namespace CombatEngine;

public class ConsoleEmptyLog : ICombatLog
{
   public static ConsoleEmptyLog Instance = new ();

   public void LogRoundBegins(int round)
   {
   }

   public void LogUpkeepBegins()
   {
   }

   public void LogUpkeepEnds()
   {
   }

   public void LogReportSides(IEnumerable<UnitState> units)
   {
   }

   public void LogTurn(UnitState unit)
   {
   }

   public void LogCastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {
   }

   public void LogTakeDamage(UnitState unit, int? amount)
   {
   }

   public void LogHealDamage(UnitState unit, int? amount)
   {
   }

   public void LogUnitDies(UnitState unit)
   {
   }

   public void LogWin(Side winningSide)
   {
   }

   public void LogWinners(IEnumerable<UnitState> winningUnits)
   {
   }

   public void LogTotalRounds(int totalRounds)
   {
   }

   public void LogCrit(Spell spell)
   {
   }
}