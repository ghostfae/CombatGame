namespace CombatEngine
{
   public interface ICombatLog
   {
      void LogRoundBegins(int round);

      void LogUpkeepBegins();
      void LogUpkeepEnds();

      void LogReportSides(IEnumerable<UnitState> units);

      void LogTurn(UnitState unit);

      void LogCastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null);

      void LogTakeDamage(UnitState unit, int? amount);

      void LogHealDamage(UnitState unit, int? amount);

      void LogUnitDies(UnitState unit);

      void LogWin(Side winningSide);

      void LogWinners(IEnumerable<UnitState> winningUnits);

      void LogTotalRounds(int totalRounds);

      void LogCrit(Spell spell);
   }
}
