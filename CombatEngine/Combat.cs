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
         _log.RoundBegins(GetAliveUnits(), _round);

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
      ApplySpell(target, spell, damage);

      if (target.CurrentHealth <= 0)
      {
         _log.UnitDies(target);
      }
   }

   public int CastSpell(Unit caster, Unit target, Spell spell)
   {
      // EXTENSION - have a % cast failed?
      var damage = Rng.Random.Next(spell.MinDamage, spell.MaxDamage);
      _log.CastSpell(caster, target, spell, damage);

      caster.ModifyState(builder => builder.MarkCooldown(spell.Kind));
      return damage;
   }

   public void ApplySpell(Unit target, Spell spell, int damage)
   {
      // TODO: apply defences etc
      target.ModifyState(builder => builder.Hit(damage));
      _log.TakeDamage(target, damage);
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
