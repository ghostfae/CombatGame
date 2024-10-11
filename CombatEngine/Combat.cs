namespace CombatEngine;

public class Combat
{
   public readonly ICombatLog CombatLog;
   public readonly ICombatListener CombatListener;
   private readonly List<UnitState> _units;

   public Combat(ICombatListener combatListener, ICombatLog log, params UnitState[] combatants)
   {
      CombatListener = combatListener;
      CombatLog = log;
      _units = combatants.ToList();
   }

   /// <returns>true if the combat should continue, false if one side wins</returns>
   public void PerformTurn(UnitState unit)
   {
      CombatLog.Turn(unit);

      unit.UpdateTick();

      var (target, spell) = unit.Unit.ChooseTargetAndSpell(GetAliveUnits());

      var damage = CastSpell(unit, target, spell);
      // EXTENSION - have a % cast failed?
      ApplySpell(target, spell, damage);

      if (target.Health <= 0)
      {
         CombatLog.UnitDies(target);
      }
   }

   public int? CastSpell(UnitState caster, UnitState target, Spell spell)
   {
      int? amount = null;
      if (spell.IsOverTime)
      {
         CombatLog.CastSpell(caster, target, spell);
      }
      else
      {
         amount = spell.SpellEffect.RollRandomAmount();
         CombatLog.CastSpell(caster, target, spell, amount);
      }

      caster.ModifyState(builder => builder.MarkCooldown(spell.Kind));
      return amount;
   }

   private void ApplySpell(UnitState target, Spell spell, int? amount)
   {
      // TODO: apply defences etc
      if (spell.IsOverTime)
      {
         ApplyOverTimeSpell(target, spell);
      }
      else
      {
         ApplyDirectHitSpell(target, spell, amount!.Value);
      }
   }

   private void ApplyDirectHitSpell(UnitState target, Spell spell, int amount)
   {
      if (DetectCrit(spell))
      {
         amount *= spell.SpellEffect.CritModifier;
         ApplyCritEffect(target, spell);
      }
      ApplyDirectHitOrHeal(target, spell, amount);
   }

   private void ApplyCritEffect(UnitState target, Spell spell)
   {
      if (spell.CritEffect != null)
      {
         if (spell.CritEffect.Kind == SpellEffectKind.OverTime)
         {
            AttachOverTime(target, spell.CritEffect);
         }
         else if (spell.CritEffect.Kind == SpellEffectKind.Freeze)
         {
            ApplyFreezeSpell(target, spell.CritEffect);
         }
      }
   }

   private void ApplyDirectHitOrHeal(UnitState target, Spell spell, int amount)
   {
      if (spell.SpellEffect.IsHarm)
      {
         DamageUnit(target, amount);
      }
      else
      {
         HealUnit(target, amount);
      }
   }

   private void ApplyOverTimeSpell(UnitState target, Spell spell)
   {
      AttachOverTime(target, spell.SpellEffect);
   }

   private void ApplyFreezeSpell(UnitState target, SpellEffect spell)
   {
      target.ModifyState(builder => builder.Freeze(spell.Duration!.Value));
   }

   private void AttachOverTime(UnitState target, SpellEffect effect)
   {
      target.ModifyState(builder => builder.AttachOverTime(effect));
   }

   private void DamageUnit(UnitState target, int amount)
   {
      target.ModifyState(builder => builder.Hit(amount));
      CombatLog.TakeDamage(target, amount);
   }

   private void HealUnit(UnitState target, int amount)
   {
      target.ModifyState(builder => builder.Heal(amount));
      CombatLog.HealDamage(target, amount);
   }

   private bool DetectCrit(Spell spell)
   {
      if (Rng.Random.Next(0, 100) <= spell.SpellEffect.CritChance)
      {
         CombatLog.Crit(spell);
         return true;
      }

      return false;
   }

   public void ResetRound()
   {
      foreach (var unit in _units)
      {
         if (unit.CanAct || unit.CanActTimer == 0)
         {
            unit.ResetRound();
         }
      }
   }

   public IEnumerable<UnitState> GetAliveUnits()
   {
      return _units.Where(unit => unit.Health > 0);
   }

   public UnitState? TryGetNextUnit()
   {
      return _units
         .Where(unit => unit is { CanAct: true, Health: > 0 })
         .MaxBy(unit => unit.Unit.Speed);
   }

   public Side? TryGetWinningSide()
   {
      var survivingSides = _units
         .Where(u => u.Health > 0)
         .GroupBy(u => u.Side)
         .Select(g => g.Key)
         .ToArray();

      if (survivingSides.Length == 1)
      {
         return survivingSides[0];
      }
      return null;
   }
}
