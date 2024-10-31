namespace CombatEngine;

public class CombatState
{
   private readonly Dictionary<int, UnitState> _combatants;

   public CombatState(IEnumerable<UnitState> combatants)
   {
      _combatants = combatants.ToDictionary(u => u.Unit.Uid, u => u);
   }

   private CombatState(Dictionary<int, UnitState> combatants)
   {
      _combatants = combatants;
   }

   public CombatState Upkeep(ICombatLog log)
   {
      // todo: log upkeep
      var updatedUnits = GetAliveUnits().Select(aliveUnit => aliveUnit.Upkeep());
      return CloneWith(updatedUnits);
   }

   public CombatState CastAndApplySpell(UnitState caster, UnitState target, Spell spell, ICombatLog log)
   {
      var (damage, updatedCaster) = CastSpell(caster, target, spell, log);

      // EXTENSION - have a % cast failed?

      var updatedTarget = ApplySpell(target, spell, damage, log);

      return CloneWith(updatedCaster, updatedTarget);
   }

   private CombatState CloneWith(IEnumerable<UnitState> updatedUnits)
   {
      var clone = new Dictionary<int, UnitState>(_combatants);
      foreach (var unit in updatedUnits)
      {
         clone[unit.Unit.Uid] = unit;
      }

      return new CombatState(clone);
   }

   public CombatState CloneWith(params UnitState[] updatedUnits)
   {
      return CloneWith((IEnumerable<UnitState>)updatedUnits);
   }

   public (int? damageAmount, UnitState newState) CastSpell(UnitState caster, UnitState target, Spell spell, ICombatLog? log)
   {
      int? amount = null;
      if (spell.IsOverTime)
      {
         log?.CastSpell(caster, target, spell);
      }
      else
      {
         amount = spell.SpellEffect.RollRandomAmount();
         log?.CastSpell(caster, target, spell, amount);
      }

      
      return (amount, caster.MarkCooldown(spell.Kind));
   }

   public UnitState ApplySpell(UnitState target, Spell spell, int? amount, ICombatLog? log)
   {
      // TODO: apply defences etc
      if (spell.IsOverTime)
      {
         return ApplyOverTimeSpell(target, spell, log);
      }
      else
      {
         return ApplyDirectHitSpell(target, spell, amount!.Value, log);
      }
   }

   private UnitState ApplyDirectHitSpell(UnitState target, Spell spell, int amount, ICombatLog? log)
   {
      if (DetectCrit(spell, log))
      {
         amount *= spell.SpellEffect.CritModifier;
         target = ApplyCritEffect(target, spell, log); // TODO: how to add this?
      }
      return ApplyDirectHitOrHeal(target, spell, amount, log);
   }

   private UnitState ApplyCritEffect(UnitState target, Spell spell, ICombatLog? log)
   {
      if (spell.CritEffect != null)
      {
         if (spell.CritEffect.Kind == SpellEffectKind.OverTime)
         {
            return AttachOverTime(target, spell.CritEffect, log);
         }
         if (spell.CritEffect.Kind == SpellEffectKind.Freeze)
         {
            return ApplyFreezeSpell(target, spell.CritEffect, log);
         }
      }

      return target;
   }

   private UnitState ApplyDirectHitOrHeal(UnitState target, Spell spell, int amount, ICombatLog? log)
   {
      if (spell.SpellEffect.IsHarm)
      {
         return DamageUnit(target, amount, log);
      }
      else
      {
         return HealUnit(target, amount, log);
      }
   }

   private UnitState ApplyOverTimeSpell(UnitState target, Spell spell, ICombatLog? log)
   {
      return AttachOverTime(target, spell.SpellEffect, log);
   }

   private UnitState ApplyFreezeSpell(UnitState target, SpellEffect spell, ICombatLog? log)
   {
      return target.Freeze(spell.Duration!.Value);
      // todo: add log freeze
   }

   private UnitState AttachOverTime(UnitState target, SpellEffect effect, ICombatLog? log)
   {
      return target.AttachOverTime(effect);
      // todo: add log overtime
   }

   private UnitState DamageUnit(UnitState target, int amount, ICombatLog? log)
   {
      target = target.Hit(amount);
      log?.TakeDamage(target, amount);
      return target;
   }

   private UnitState HealUnit(UnitState target, int amount, ICombatLog? log)
   {
      target = target.Heal(amount);
      log?.HealDamage(target, amount);
      return target;
   }

   private bool DetectCrit(Spell spell, ICombatLog? log)
   {
      if (Rng.Random.Next(0, 100) <= spell.SpellEffect.CritChance)
      {
         log?.Crit(spell);
         return true;
      }

      return false;
   }

   public void ResetRound()
   {
      foreach (var unit in _combatants)
      {
         if (unit.Value.CanAct || unit.Value.CanActTimer == 0)
         {
            unit.Value.ResetRound();
         }
      }
   }

   public IEnumerable<UnitState> GetAliveUnits()
   {
      return _combatants
         .Values
         .Where(unit => unit.Health > 0);
   }

   public UnitState? TryGetNextUnit()
   {
      return _combatants
         .Where(unit => unit.Value is { CanAct: true, Health: > 0 })
         .OrderByDescending(unit => unit.Value.Unit.Speed)
         .Select(kvp => kvp.Value)
         .FirstOrDefault();
   }

   public Side? TryGetWinningSide()
   {
      var survivingSides = _combatants
         .Where(u => u.Value.Health > 0)
         .GroupBy(u => u.Value.Side)
         .Select(g => g.Key)
         .ToArray();

      if (survivingSides.Length == 1)
      {
         return survivingSides[0];
      }
      return null;
   }
}
