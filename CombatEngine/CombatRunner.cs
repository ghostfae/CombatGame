using System.Text;

namespace CombatEngine;

/// <summary>
/// engine that deals with all the actions taken within combat,
/// handles unit turns and rounds
/// </summary>
///
public static class CombatRunner
{
   public static void PerformTurn(Combat combat, UnitState unit, ICombatLog log)
   {
      log.Turn(unit);

      unit.UpdateTick();

      var (target, spell) = unit.Unit.ChooseTargetAndSpell(combat.GetAliveUnits());

      var (damage, newState) = combat.CastSpell(unit, target, spell, log);
      // TODO: update cooldown with new state
      unit.ModifySelf(newState);
      // EXTENSION - have a % cast failed?

      target.ModifySelf(combat.ApplySpell(target, spell, damage, log)); // todo: apply to target 

      if (target.Health <= 0)
      {
         log.UnitDies(target);
      }
   }

   public static IEnumerable<UnitState> Run(Combat combat, ICombatLog log, ICombatListener listener)
   {
      int round = 1;

      while (true)
      {
         log.RoundBegins(round);
         log.UpkeepBegins();
         foreach (var aliveUnit in combat.GetAliveUnits())
         {
            aliveUnit.Upkeep();
         }

         log.UpkeepEnds();

         log.ReportSides(combat.GetAliveUnits());
         if (GetWin(combat, round, log) != null)
         {
            return combat.GetAliveUnits();
         }

         var unit = combat.TryGetNextUnit();
         while (unit != null)
         {
            PerformTurn(combat, unit, log);
            unit = combat.TryGetNextUnit();

            if (GetWin(combat, round, log) != null)
            {
               return combat.GetAliveUnits();
            }
         }

         listener.EndOfRound(round);
         combat.ResetRound();
         round++;
      }
   }

   public static IEnumerable<UnitState>? GetWin(Combat combatInstance, int round, ICombatLog log)
   {
      var winningSide = combatInstance.TryGetWinningSide();
      if (winningSide != null)
      {
         log.TotalRounds(round);
         log.Win(winningSide.Value);
         log.Winners(combatInstance.GetAliveUnits());
         return combatInstance.GetAliveUnits();
      }

      return null;
   }
}