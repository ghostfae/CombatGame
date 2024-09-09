namespace CombatEngine;

public class Combat
{
   private readonly CombatLog _log;
   private readonly List<Unit> _units;

   private int _round;

   public Combat(params Unit[] combatants)
   {
      _log = new CombatLog();
      _units = combatants.ToList();

      _round = 1;
   }

   public void Run()
   {
      while (true)
      {
         _log.RoundBegins(_round);
         _log.UpkeepBegins();
         foreach (var aliveUnit in GetAliveUnits())
         {
            aliveUnit.Upkeep();
         }
         _log.UpkeepEnds();

         _log.ReportSides(GetAliveUnits());
         var winningSide = TryGetWinningSide();
         if (winningSide != null)
         {
            _log.Win(winningSide.Value);
            return;
         }

         var unit = TryGetNextUnit();
         while (unit != null)
         {
            PerformTurn(unit);
            unit = TryGetNextUnit();
         }

         ResetRound();
         _round++;
         // DEBUG
         Console.ReadLine();
      }
   }

   /// <returns>true if the combat should continue, false if one side wins</returns>
   private void PerformTurn(Unit unit)
   {
      _log.Turn(unit);

      unit.UpdateTick();
      unit.MarkAsTakenTurn();

      // 1. apply dots or hots if any
      // TODO:

      var (target, spell) = unit.ChooseTargetAndSpell(GetAliveUnits());

      var damage = CastSpell(unit, target, spell);
      // EXTENSION - have a % cast failed?
      ApplySpell(target, spell, damage);

      if (target.CurrentHealth <= 0)
      {
         _log.UnitDies(target);
      }
   }

   public int? CastSpell(Unit caster, Unit target, Spell spell)
   {
      int? amount = null;
      if (spell.IsOverTime)
      {
         _log.CastSpell(caster, target, spell);
      }
      else
      {
         amount = spell.SpellEffect.RollRandomAmount();
         _log.CastSpell(caster, target, spell, amount);
      }

      caster.ModifyState(builder => builder.MarkCooldown(spell.Kind));
      return amount;
   }

   public void ApplySpell(Unit target, Spell spell, int? amount)
   {
      // TODO: apply defences etc
      if (spell.IsOverTime)
      {
         target.ModifyState(builder => builder.AttachOverTime(spell.SpellEffect));
      }
      else
      {
         if (spell.SpellEffect.IsHarm) // amount is never null for direct spell
         {
            target.ModifyState(builder => builder.Hit(amount!.Value));
            _log.TakeDamage(target, amount);
         }
         else
         {
            target.ModifyState(builder => builder.Heal(amount!.Value));
            _log.HealDamage(target, amount);
         }
      }
   }


   public void ResetRound()
   {
      foreach (var unit in _units)
      {
         unit.ResetRound();
      }
   }

   private IEnumerable<Unit> GetAliveUnits()
   {
      return _units.Where(unit => unit.CurrentHealth > 0);
   }

   public Unit? TryGetNextUnit()
   {
      return _units
         .Where(unit => unit.CanAct() && unit.CurrentHealth > 0)
         .MaxBy(unit => unit.Speed);
   }

   public Side? TryGetWinningSide()
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
