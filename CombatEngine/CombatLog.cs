namespace CombatEngine;

public class CombatLog
{
   public void Init(IEnumerable<Unit> combatants)
   {
      foreach (var unit in combatants)
      {
         Console.WriteLine($"{(unit.Kind)} has speed of {unit.Speed}.");
      }

      Console.WriteLine();
   }

   public void StartTurn(Unit currentCombatant)
   {
      Console.WriteLine();
      Console.WriteLine($"It is {currentCombatant.Kind}'s turn.");
      Console.WriteLine($"{currentCombatant.Kind} has {currentCombatant.CurrentHealth} health.");
   }

   public void CastSpell(Unit currentCombatant, Spell currentSpell, int damage)
   {
      Console.WriteLine($"{currentCombatant.Kind} cast {currentSpell.Kind} for {damage} damage.");
   }

   public void TakeDamage(Unit enemyCombatant, int damage)
   {
      Console.WriteLine($"{enemyCombatant.Kind} was hit for {damage} damage.");
   }

   public void Win(Side winningSide)
   {
      Console.WriteLine($"{winningSide} wins!");
   }

   public void UnitDies(Unit target)
   {
      Console.WriteLine($"{target.Kind} dies.");
   }
}