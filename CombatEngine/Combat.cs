namespace CombatEngine;

public class Combat
{
   //public readonly ICombatLog CombatLog;
   //public readonly ICombatListener CombatListener;
   private readonly List<UnitState> _units;

   public Combat( params UnitState[] combatants)
   {
      //CombatListener = combatListener;
      //CombatLog = log;
      _units = combatants.ToList();
   }

   /// <returns>true if the combat should continue, false if one side wins</returns>
   

   public int? CastSpell(UnitState caster, UnitState target, Spell spell, ICombatLog? log)
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

      caster.ModifyState(builder => builder.MarkCooldown(spell.Kind));
      return amount;
   }

   public void ApplySpell(UnitState target, Spell spell, int? amount, ICombatLog? log)
   {
      // TODO: apply defences etc
      if (spell.IsOverTime)
      {
         ApplyOverTimeSpell(target, spell, log);
      }
      else
      {
         ApplyDirectHitSpell(target, spell, amount!.Value, log);
      }
   }

   private void ApplyDirectHitSpell(UnitState target, Spell spell, int amount, ICombatLog? log)
   {
      if (DetectCrit(spell, log))
      {
         amount *= spell.SpellEffect.CritModifier;
         ApplyCritEffect(target, spell, log);
      }
      ApplyDirectHitOrHeal(target, spell, amount, log);
   }

   private void ApplyCritEffect(UnitState target, Spell spell, ICombatLog? log)
   {
      if (spell.CritEffect != null)
      {
         if (spell.CritEffect.Kind == SpellEffectKind.OverTime)
         {
            AttachOverTime(target, spell.CritEffect, log);
         }
         else if (spell.CritEffect.Kind == SpellEffectKind.Freeze)
         {
            ApplyFreezeSpell(target, spell.CritEffect, log);
         }
      }
   }

   private void ApplyDirectHitOrHeal(UnitState target, Spell spell, int amount, ICombatLog? log)
   {
      if (spell.SpellEffect.IsHarm)
      {
         DamageUnit(target, amount, log);
      }
      else
      {
         HealUnit(target, amount, log);
      }
   }

   private void ApplyOverTimeSpell(UnitState target, Spell spell, ICombatLog? log)
   {
      AttachOverTime(target, spell.SpellEffect, log);
   }

   private void ApplyFreezeSpell(UnitState target, SpellEffect spell, ICombatLog? log)
   {
      target.ModifyState(builder => builder.Freeze(spell.Duration!.Value));
      // todo: add log freeze
   }

   private void AttachOverTime(UnitState target, SpellEffect effect, ICombatLog? log)
   {
      target.ModifyState(builder => builder.AttachOverTime(effect));
      // todo: add log overtime
   }

   private void DamageUnit(UnitState target, int amount, ICombatLog? log)
   {
      target.ModifyState(builder => builder.Hit(amount));
      log?.TakeDamage(target, amount);
   }

   private void HealUnit(UnitState target, int amount, ICombatLog? log)
   {
      target.ModifyState(builder => builder.Heal(amount));
      log?.HealDamage(target, amount);
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
