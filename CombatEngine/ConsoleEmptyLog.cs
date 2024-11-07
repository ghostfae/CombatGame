namespace CombatEngine;

public class ConsoleEmptyLog : ICombatLog
{
   public static ConsoleEmptyLog Instance = new ();

   public void RoundBegins(int round)
   {
   }

   public void UpkeepBegins()
   {
   }

   public void UpkeepEnds()
   {
   }

   public void ReportSides(IEnumerable<UnitState> units)
   {
   }

   public void Turn(UnitState unit)
   {
   }

   public void CastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {
   }

   public void TakeDamage(UnitState unit, int? amount)
   {
   }

   public void HealDamage(UnitState unit, int? amount)
   {
   }

   public void UnitDies(UnitState unit)
   {
   }

   public void Win(Side winningSide)
   {
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
   }

   public void TotalRounds(int totalRounds)
   {
   }

   public void Crit(Spell spell)
   {
   }
}