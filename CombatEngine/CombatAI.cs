namespace CombatEngine;

public class CombatAI : INextMoveStrategy
{
   private const int Depth = 4;
   private const int MaxSimulations = 100;
   private const int TopSimulationsToAnalyse = MaxSimulations / 10;

   public (UnitState target, Spell spell)? ChooseNextMove(UnitState caster, CombatState combatState)
   {
      // TODO: if no targets or no spells, return null
      var scoredActions = Enumerable
         .Range(0, MaxSimulations)
         .Select(_ => EvaluateChain(combatState, caster, Depth, caster.Side))
         .OrderByDescending(scoredAction => scoredAction.Score)
         .ToArray(); //todo: remove, only used for debugging


      var bestAction = scoredActions
         .Take(TopSimulationsToAnalyse)
         .GroupBy(scoredAction => scoredAction, ScoredActionComparer.Instance)
         .MaxBy(grouping => grouping.Count())
         ?.First();

      return bestAction != null
         ? (combatState.Combatants[bestAction.TargetUid], bestAction.Spell)
         : null;
   }

   public int GetAverage(int[] scores)
   {
      return scores.Sum() / scores.Length;
   }

   public ScoredAction EvaluateChain(
      CombatState combatState,
      UnitState caster,
      int depth,
      Side initSide)
   {
      var firstCasterAction = ChooseRandomTargetAndSpell(caster, combatState.GetAliveUnits());

      var currentAction = firstCasterAction;
      var currentCaster = caster;

      while (depth > 0)
      {
         combatState = ApplyTurn(currentAction, combatState, currentCaster, ConsoleEmptyLog.Instance);

         //DebugLogCurrentScore(currentAction, currentCaster, depth);
         currentCaster = combatState.GetNextTurnUnit(currentCaster);

         if (GetWin(combatState.Combatants.Values, currentCaster))
            break;

         currentAction = ChooseRandomTargetAndSpell(currentCaster, combatState.GetAliveUnits());
         depth--;
      }

      //Console.WriteLine("End of current chain");
      // calculate the score and return
      return new ScoredAction(firstCasterAction.target.Unit.Uid, firstCasterAction.spell,
         CalculateTotalScoreForSide(combatState, initSide));
   }

   private static bool GetWin(IEnumerable<UnitState> combatants, UnitState caster)
   {
      return combatants
         .Where(u => u.Health > 0)
         .All(u => u.Side == caster.Side);
   }

   private static (UnitState target, Spell spell) ChooseRandomTargetAndSpell(UnitState caster, IEnumerable<UnitState> availableTargets)
   {
      var selectedSpell = UnitBehaviour.SelectRandomSpell(caster);
      var allTargets = new List<UnitState>();

      foreach (var state in availableTargets)
      {
         allTargets.Add(state);
      }

      var selectedTarget = selectedSpell.SpellEffect.IsHarm ? // if operator
         UnitBehaviour.SelectRandomEnemy(allTargets, caster)
         : UnitBehaviour.SelectRandomAlly(allTargets, caster);

      return (selectedTarget, selectedSpell);
   }

   private static void DebugLogCurrentScore((UnitState target, Spell spell) action,
      UnitState caster, int depth)
   {
      Console.WriteLine(
         $"at depth {depth}: caster is {caster.Unit.Name}, target is {action.target.Unit.Name}, spell is {action.spell.Kind}");
   }


   private static int CalculateTotalScoreForSide(CombatState state, Side side)
   {
      // for now, we only take the sum of ally health - sum of enemy health
      return state.Combatants.Values.Sum(unit => unit.Health * (unit.Side == side ? 1 : -1));
   }

   public CombatState ApplyTurn((UnitState target, Spell spell) action, CombatState combatState, UnitState caster,
      ICombatLog log)
   {
      if (caster.CanAct != true)
      {
         if(!combatState.TryGetNextUnit(out var unit))
            combatState = ResetRound(combatState);

         return combatState;
      }

      combatState = combatState.ExhaustTurn(combatState, caster, log);
      caster = combatState.Combatants[caster.Unit.Uid];

      combatState = CastAndApplySpell(combatState, caster, action.target, action.spell, log);

      if (action.target.Health <= 0)
      {
         log.LogUnitDies(action.target);
      }

      return combatState;
   }
   public CombatState CastAndApplySpell(CombatState state, UnitState caster, UnitState target, Spell spell, ICombatLog log)
   {
      var (damage, updatedCaster) = state.CastSpell(caster, target, spell, log);

      // EXTENSION - have a % cast failed?

      var updatedTarget = state.ApplySpell(target, spell, damage, log);

      return state.CloneWith(updatedCaster, updatedTarget);
   }

   public CombatState ResetRound(CombatState state)
   {
      return state.CloneWith(ResetCombatants(state));
   }

   public IEnumerable<UnitState> ResetCombatants(CombatState state)
   {
      foreach (var unit in state.Combatants.Values)
      {
         if (unit.IsAlive() && (unit.CanAct || unit.CanActTimer == 0))
         {
            yield return unit.ResetRound();
         }
      }
   }

}