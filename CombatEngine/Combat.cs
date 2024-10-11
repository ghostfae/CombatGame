namespace CombatEngine;
/// <summary>
/// engine that deals with all the actions taken within combat,
/// handles unit turns and rounds
/// </summary>
public class Combat
{
   private readonly ICombatLog _combatLog;
   private readonly ICombatListener _combatListener;
   private readonly List<Unit> _units;

   private int _round;

   public Combat(ICombatListener combatListener, ICombatLog log, params Unit[] combatants)
   {
      _combatListener = combatListener;
      _combatLog = log;
      _units = combatants.ToList();

      _round = 1;
   }

   public IEnumerable<Unit> Run()
   {
      while (true)
      {
         _combatLog.RoundBegins(_round);
         _combatLog.UpkeepBegins();
         foreach (var aliveUnit in GetAliveUnits())
         {
            aliveUnit.Upkeep();
         }
         _combatLog.UpkeepEnds();

         _combatLog.ReportSides(GetAliveUnits());
         var winningSide = TryGetWinningSide();
         if (winningSide != null)
         {
            _combatLog.TotalRounds(_round);
            _combatLog.Win(winningSide.Value);
            _combatLog.Winners(GetAliveUnits());
            return GetAliveUnits();
         }

         var unit = TryGetNextUnit();
         while (unit != null)
         {
            PerformTurn(unit);
            unit = TryGetNextUnit();

            // TODO: extract function
            winningSide = TryGetWinningSide();
            if (winningSide != null)
            {
               _combatLog.TotalRounds(_round);
               _combatLog.Win(winningSide.Value);
               _combatLog.Winners(GetAliveUnits());
               return GetAliveUnits();
            }
         }

         _combatListener.EndOfRound(_round);
         ResetRound();
         _round++;
      }
   }

   /// <returns>true if the combat should continue, false if one side wins</returns>
   private void PerformTurn(Unit unit)
   {
      _combatLog.Turn(unit);

      unit.UpdateTick();

      var (target, spell) = unit.ChooseTargetAndSpell(GetAliveUnits());

      var damage = CastSpell(unit, target, spell);
      // EXTENSION - have a % cast failed?
      ApplySpell(target, spell, damage);

      if (target.CurrentHealth <= 0)
      {
         _combatLog.UnitDies(target);
      }
   }

   private int? CastSpell(Unit caster, Unit target, Spell spell)
   {
      int? amount = null;
      if (spell.IsOverTime)
      {
         _combatLog.CastSpell(caster, target, spell);
      }
      else
      {
         amount = spell.SpellEffect.RollRandomAmount();
         _combatLog.CastSpell(caster, target, spell, amount);
      }

      caster.ModifyState(builder => builder.MarkCooldown(spell.Kind));
      return amount;
   }
   private void ApplySpell(Unit target, Spell spell, int? amount)
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
   private void ApplyDirectHitSpell(Unit target, Spell spell, int amount)
   {
      if (DetectCrit(spell))
      {
         amount *= spell.SpellEffect.CritModifier;
         ApplyCritEffect(target, spell);
      }
      ApplyDirectHitOrHeal(target, spell, amount);
   }

   private void ApplyCritEffect(Unit target, Spell spell)
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

   private void ApplyDirectHitOrHeal(Unit target, Spell spell, int amount)
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

   private void ApplyOverTimeSpell(Unit target, Spell spell)
   {
      AttachOverTime(target, spell.SpellEffect);
   }

   private void ApplyFreezeSpell(Unit target, SpellEffect spell)
   {
      target.ModifyState(builder => builder.Freeze(spell.Duration!.Value));
   }

   private void AttachOverTime(Unit target, SpellEffect effect)
   {
      target.ModifyState(builder => builder.AttachOverTime(effect));
   }

   private void DamageUnit(Unit target, int amount)
   {
      target.ModifyState(builder => builder.Hit(amount));
      _combatLog.TakeDamage(target, amount);
   }

   private void HealUnit(Unit target, int amount)
   {
      target.ModifyState(builder => builder.Heal(amount));
      _combatLog.HealDamage(target, amount);
   }

   private bool DetectCrit(Spell spell)
   {
      if (Rng.Random.Next(0, 100) <= spell.SpellEffect.CritChance)
      {
         _combatLog.Crit(spell);
         return true;
      }

      return false;
   }

   private void ResetRound()
   {
      foreach (var unit in _units)
      {
         if (unit.State.CanAct || unit.State.CanActTimer == 0)
         {
            unit.ResetRound();
         }
      }
   }

   private IEnumerable<Unit> GetAliveUnits()
   {
      return _units.Where(unit => unit.CurrentHealth > 0);
   }

   private Unit? TryGetNextUnit()
   {
      return _units
         .Where(unit => unit.CanAct() && unit.CurrentHealth > 0)
         .MaxBy(unit => unit.Speed);
   }

   private Side? TryGetWinningSide()
   {
      var survivingSides = _units
         .Where(u => u.CurrentHealth > 0)
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
