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
   public static bool TryPerformTurn(CombatState combatState, out CombatState? newCombatState, ICombatLog log)
   {
      if (combatState.TryGetNextUnit(out var nextUnit))
      {
         newCombatState = PerformTurn(combatState, nextUnit!, log);
         return true;
      }

      newCombatState = null;
      return false;
   }

   public static CombatState PerformTurn(CombatState combatState, UnitState caster, ICombatLog log)
   {
      log.Turn(caster);

      caster = caster.Tick().ExhaustTurn();
      combatState = combatState.CloneWith(caster);

      var (target, spell) = caster.Unit.ChooseTargetAndSpell(combatState.GetAliveUnits());

      combatState = combatState.CastAndApplySpell(caster, target, spell, log);

      if (target.Health <= 0)
      {
         log.UnitDies(target);
      }

      return combatState;
   }

   public static CombatState PerformTurn2(CombatState combatState, UnitState caster, ICombatLog log)
   {
      log.Turn(caster);

      caster = caster.Tick().ExhaustTurn();
      combatState = combatState.CloneWith(caster);

      var (target, spell) = caster.Unit.ChooseTargetAndSpell(combatState.GetAliveUnits());

      combatState = combatState.CastAndApplySpell(caster, target, spell, log);

      if (target.Health <= 0)
      {
         log.UnitDies(target);
      }

      return combatState;
   }

   public static IEnumerable<UnitState> Run(CombatState combatState, ICombatLog log, ICombatListener listener)
   {
      int round = 1;

      while (true)
      {
         log.RoundBegins(round);

         log.UpkeepBegins();
         combatState = combatState.Upkeep(log);
         log.UpkeepEnds();

         log.ReportSides(combatState.GetAliveUnits());

         if (GetWin(combatState, round, log) != null)
         {
            return combatState.GetAliveUnits();
         }

         while (TryPerformTurn(combatState, out var newCombatState,  log))
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
         log.TotalRounds(round);
         log.Win(winningSide.Value);
         log.Winners(combatStateInstance.GetAliveUnits());
         return combatStateInstance.GetAliveUnits();
      }

      return null;
   }
}