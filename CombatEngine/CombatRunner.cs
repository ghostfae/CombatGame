using System.Text;

namespace CombatEngine;

/// <summary>
/// engine that deals with all the actions taken within combat,
/// handles unit turns and rounds
/// </summary>
///
public class CombatRunner(ICombatLog log)
{
   // returns false when end of round
   public bool TryPerformTurn(
      CombatState combatState, 
      out CombatState? newCombatState,
      INextMoveStrategy strategy,
      ICombatListener listener)
   {
      if (combatState.TryGetNextUnit(out var nextUnit))
      {
         newCombatState = PerformTurn(combatState, nextUnit!, strategy, listener);
         return true;
      }

      newCombatState = null;
      return false;
   }

   public CombatState PerformTurn(CombatState combatState, UnitState caster, INextMoveStrategy strategy, ICombatListener listener)
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
      listener.CastSpell(caster.Unit.Kind, spell.Kind);

      if (target.Health <= 0)
      {
         log.LogUnitDies(target);
      }
      return combatState;
   }

   public IEnumerable<UnitState> Run(CombatState combatState, ICombatListener listener)
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

         if (TryGetWinners(combatState, round, out var winners))
         {
            listener.Winners(winners);
            return winners;
         }

         while (TryPerformTurn(combatState, out var newCombatState, strategy, listener))
         {
            combatState = newCombatState!;

            if (TryGetWinners(combatState, round, out winners))
            {
               listener.Winners(winners);
               // todo: use listener instead of return value
               return winners;
            }
         }

         listener.EndOfRound(round);
         combatState.ResetRound();
         round++;
      }
   }

   public bool TryGetWinners(CombatState combatState, int round, out IReadOnlyCollection<UnitState> winners)
   {
      if (combatState.TryGetWinningSide(out var winningSide))
      {
         log.LogTotalRounds(round);
         log.LogWin(winningSide);

         winners = combatState.GetAliveUnits().ToArray();
         log.LogWinners(winners);
         return true;
      }

      winners = Array.Empty<UnitState>();
      return false;
   }
}