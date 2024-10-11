using System.Text;

namespace CombatEngine;

/// <summary>
/// engine that deals with all the actions taken within combat,
/// handles unit turns and rounds
/// </summary>
///
public static class CombatRunner
{

   public static IEnumerable<UnitState> Run(Combat combatInstance)
   {
      int round = 1;

      while (true)
      {
         combatInstance.CombatLog.RoundBegins(round);
         combatInstance.CombatLog.UpkeepBegins();
         foreach (var aliveUnit in combatInstance.GetAliveUnits())
         {
            aliveUnit.Upkeep();
         }

         combatInstance.CombatLog.UpkeepEnds();

         combatInstance.CombatLog.ReportSides(combatInstance.GetAliveUnits());
         if (GetWin(combatInstance, round) != null)
         {
            return combatInstance.GetAliveUnits();
         }

         var unit = combatInstance.TryGetNextUnit();
         while (unit != null)
         {
            combatInstance.PerformTurn(unit);
            unit = combatInstance.TryGetNextUnit();

            if (GetWin(combatInstance, round) != null)
            {
               return combatInstance.GetAliveUnits();
            }
         }

         combatInstance.CombatListener.EndOfRound(round);
         combatInstance.ResetRound();
         round++;
      }
   }

   public static IEnumerable<UnitState>? GetWin(Combat combatInstance, int round)
   {
      var winningSide = combatInstance.TryGetWinningSide();
      if (winningSide != null)
      {
         combatInstance.CombatLog.TotalRounds(round);
         combatInstance.CombatLog.Win(winningSide.Value);
         combatInstance.CombatLog.Winners(combatInstance.GetAliveUnits());
         return combatInstance.GetAliveUnits();
      }

      return null;
   }
}