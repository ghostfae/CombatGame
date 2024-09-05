namespace CombatEngine;

public class CombatLog
{
   public void RoundBegins(IEnumerable<Unit> units, int round)
   {
      Console.WriteLine();
      Console.WriteLine($"BEGIN ROUND {round}");

      var sides = units
         .GroupBy(unit => unit.Side)
         .Select(group => new
         {
            Side = group.Key,
            Units = group.OrderByDescending(unit => unit.Speed).ToList()
         });

      foreach (var side in sides)
      {
         Console.WriteLine($"Side {side.Side}:");
;
         foreach (var unit in side.Units)
            Console.WriteLine($"{unit} has speed of {unit.Speed} and {unit.CurrentHealth} health.");
      }
   }

   public void Turn(Unit unit)
   {
      Console.WriteLine();
      Console.WriteLine($"It is {unit}'s turn.");
   }

   public void CastSpell(Unit unit, Unit target, Spell currentSpell, int damage)
   {
      Console.WriteLine($"{unit} cast {currentSpell.Kind} for {damage} damage at {target}.");
   }

   public void TakeDamage(Unit unit, int damage)
   {
      Console.WriteLine($"{unit} was hit for {damage} damage and has {unit.CurrentHealth} health remaining.");
   }

   public void Win(Side winningSide)
   {
      Console.WriteLine($"{winningSide} wins!");
   }

   public void UnitDies(Unit unit)
   {
      Console.WriteLine($"{unit} dies.");
   }
}