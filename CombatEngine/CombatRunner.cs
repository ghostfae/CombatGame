using System.Text;

namespace CombatEngine;

/// <summary>
/// engine that deals with all the actions taken within combat,
/// handles unit turns and rounds
/// </summary>
///
public static class CombatRunner
{
   // returns false when end of round
   public static bool TryPerformTurn(
      CombatState combatState, 
      out CombatState? newCombatState,
      INextMoveStrategy strategy,
      ICombatLog log)
   {
      if (combatState.TryGetNextUnit(out var nextUnit))
      {
         newCombatState = PerformTurn(combatState, nextUnit!, strategy, log);
         return true;
      }

      newCombatState = null;
      return false;
   }

   public static CombatState PerformTurn(CombatState combatState, UnitState caster, INextMoveStrategy strategy, ICombatLog log)
   {
      if (!caster.CanAct)
         return combatState;

      combatState = combatState.ExhaustTurn(combatState, caster, log);
      caster = combatState.Combatants[caster.Unit.Uid];

      var nextMove = strategy.ChooseNextMove(caster, combatState);
      
      if (nextMove == null) 
         return combatState;
      
      var (target, spell) = nextMove.Value;
      combatState = combatState.CastAndApplySpell(caster, target, spell, log);

      if (target.Health <= 0)
      {
         log.LogUnitDies(target);
      }
      return combatState;
   }

   public static IEnumerable<UnitState> Run(CombatState combatState, ICombatLog log, ICombatListener listener)
   {
      var strategy = new CombatAI();
      int round = 1;

      while (true)
      {
         log.LogRoundBegins(round);

         log.LogUpkeepBegins();
         combatState = combatState.Upkeep(log);
         log.LogUpkeepEnds();

         log.LogReportSides(combatState.GetAliveUnits());

         if (GetWin(combatState, round, log) != null)
         {
            return combatState.GetAliveUnits();
         }

         while (TryPerformTurn(combatState, out var newCombatState, strategy, log))
         {
            combatState = newCombatState!;

            if (GetWin(combatState, round, log) != null)
            {
               return combatState.GetAliveUnits();
            }
         }

         listener.EndOfRound(round);
         combatState.ResetRound();
         round++;
      }
   }

   public static IEnumerable<UnitState>? GetWin(CombatState combatStateInstance, int round, ICombatLog log)
   {
      var winningSide = combatStateInstance.TryGetWinningSide();
      if (winningSide != null)
      {
         log.LogTotalRounds(round);
         log.LogWin(winningSide.Value);
         log.LogWinners(combatStateInstance.GetAliveUnits());
         return combatStateInstance.GetAliveUnits();
      }

      return null;
   }
}