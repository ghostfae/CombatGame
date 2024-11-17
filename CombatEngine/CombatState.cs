namespace CombatEngine;

public class CombatState
{
   /// <summary>
   /// Combatants by Unit ID including dead Units
   /// </summary>
   public readonly Dictionary<int, UnitState> Combatants;

   public CombatState(IEnumerable<UnitState> combatants)
   {
      Combatants = combatants.ToDictionary(u => u.Unit.Uid, u => u);
   }

   private CombatState(Dictionary<int, UnitState> combatants)
   {
      Combatants = combatants;
   }

   public CombatState Upkeep(ICombatLog log)
   {
      // todo: log upkeep
      var updatedUnits = GetAliveUnits().Select(aliveUnit => aliveUnit.Upkeep());
      return CloneWith(updatedUnits);
   }

   public CombatState ExhaustTurn(CombatState combatState, UnitState caster, ICombatLog log)
   {
      log.LogTurn(caster);

      caster = caster.Tick().ExhaustTurn();
      return combatState.CloneWith(caster);
   }

   public CombatState CastAndApplySpell(UnitState caster, UnitState target, Spell spell, ICombatLog log)
   {
      var (damage, updatedCaster) = CastSpell(caster, target, spell, log);

      // EXTENSION - have a % cast failed?

      var updatedTarget = ApplySpell(target, spell, damage, log);

      return CloneWith(updatedCaster, updatedTarget);
   }

   public CombatState CloneWith(IEnumerable<UnitState> updatedUnits)
   {
      var clone = new Dictionary<int, UnitState>(Combatants);
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
         log?.LogCastSpell(caster, target, spell);
      }
      else
      {
         amount = spell.SpellEffect.RollRandomAmount();
         log?.LogCastSpell(caster, target, spell, amount);
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
      log?.LogTakeDamage(target, amount);
      return target;
   }

   private UnitState HealUnit(UnitState target, int amount, ICombatLog? log)
   {
      target = target.Heal(amount);
      log?.LogHealDamage(target, amount);
      return target;
   }

   private bool DetectCrit(Spell spell, ICombatLog? log)
   {
      if (Rng.Random.Next(0, 100) <= spell.SpellEffect.CritChance)
      {
         log?.LogCrit(spell);
         return true;
      }

      return false;
   }

   public CombatState ResetRound()
   {
      return CloneWith(ResetCombatants());
   }

   public IEnumerable<UnitState> ResetCombatants()
   {
      foreach (var unit in Combatants.Values)
      {
         if (unit is { Health: > 0, CanActTimer: 0 })
         {
            yield return unit.ResetRound();
         }
      }
   }

   public IEnumerable<UnitState> GetAliveUnits()
   {
      return Combatants
         .Values
         .Where(unit => unit.Health > 0);
   }

   public bool TryGetNextUnit(out UnitState? nextUnit)
   {
      nextUnit = GetNextUnitOrDefault();
      return nextUnit != null;
   }

   /// <summary>
   /// returns null when round ends
   /// </summary>
   public UnitState? GetNextUnitOrDefault()
   {
      return Combatants
         .Where(unit => unit.Value is { CanAct: true, Health: > 0 })
         .OrderByDescending(unit => unit.Value.Unit.Speed)
         .Select(kvp => kvp.Value)
         .FirstOrDefault();
   }

   public UnitState GetNextTurnUnit(UnitState lastUnit)
   {
      return Combatants
         .Where(unit => unit.Value is { CanAct: true, Health: > 0 })
         .OrderByDescending(unit => unit.Value.Unit.Speed)
         .Where(u => u.Key != lastUnit.Unit.Uid)
         .Select(kvp => kvp.Value)
         .FirstOrDefault(lastUnit);
   }
   public Side? TryGetWinningSide()
   {
      var survivingSides = Combatants
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
