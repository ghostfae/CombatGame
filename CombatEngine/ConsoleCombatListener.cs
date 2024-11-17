namespace CombatEngine;

public interface ICombatListener
{
   void RoundBegins(CombatState combatState, int round);

   void UpkeepBegins(CombatState combatState);

   void UpkeepEnds(CombatState combatState);

   void ReportSides(CombatState combatState, IEnumerable<UnitState> units);

   void Turn(CombatState combatState, UnitState unit);

   void CastSpell(CombatState combatState, UnitState unit, UnitState target, Spell currentSpell, int? amount = null);

   void TakeDamage(CombatState combatState, UnitState unit, int? amount);

   void HealDamage(CombatState combatState, UnitState unit, int? amount);

   void UnitDies(CombatState combatState, UnitState unit);

   void Win(CombatState combatState, Side winningSide);

   void Winners(CombatState combatState, IEnumerable<UnitState> winningUnits);

   void TotalRounds(CombatState combatState, int totalRounds);

   void Crit(CombatState combatState, Spell spell);
   void EndOfRound(CombatState combatState, int round);
}

public class ConsoleCombatListener(ICombatLog log) : ICombatListener
{
   private ICombatLog _log = log;

   public void RoundBegins(CombatState combatState, int round)
   {
      _log.LogRoundBegins(round);
   }

   public void UpkeepBegins(CombatState combatState)
   {
      _log.LogUpkeepBegins();
   }

   public void UpkeepEnds(CombatState combatState)
   {
      _log.LogUpkeepEnds();
   }

   public void ReportSides(CombatState combatState, IEnumerable<UnitState> units)
   {
      _log.LogReportSides(units);
   }

   public void Turn(CombatState combatState, UnitState unit)
   {
      _log.LogTurn(unit);
   }

   public void CastSpell(CombatState combatState, UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {
      _log.LogCastSpell(unit, target, currentSpell, amount);
   }

   public void TakeDamage(CombatState combatState, UnitState unit, int? amount)
   {
      _log.LogTakeDamage(unit, amount);
   }

   public void HealDamage(CombatState combatState, UnitState unit, int? amount)
   {
      _log.LogHealDamage(unit, amount);
   }

   public void UnitDies(CombatState combatState, UnitState unit)
   {
      _log.LogUnitDies(unit);
   }

   public void Win(CombatState combatState, Side winningSide)
   {
      _log.LogWin(winningSide);
   }

   public void Winners(CombatState combatState, IEnumerable<UnitState> winningUnits)
   {
      _log.LogWinners(winningUnits);
   }

   public void TotalRounds(CombatState combatState, int totalRounds)
   {
      _log.LogTotalRounds(totalRounds);
   }

   public void Crit(CombatState combatState, Spell spell)
   {
      _log.LogCrit(spell);
   }

   public void EndOfRound(CombatState combatState, int round)
   {
      Console.ReadLine();
   }
}