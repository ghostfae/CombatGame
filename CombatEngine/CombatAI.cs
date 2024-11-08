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
         ? (bestAction.Target, bestAction.Spell)
         : null;
   }

   public (UnitState target, Spell spell)? ChooseNextMove2(UnitState caster, CombatState combatState)
   {
      var allActions = new List<ScoredAction>();

      // simulate scored actions
      for(int i = 0; i < MaxSimulations; i++)
      {
         var scoredAction = EvaluateChain(combatState, caster, Depth, caster.Side);
         allActions.Add(scoredAction);
         Console.WriteLine($"Target: {scoredAction.Target.Unit.Name}, Spell: {scoredAction.Spell.Kind}, Score: {scoredAction.Score}\n");
      }

      // get average of each, find best

      // TODO: if empty, return null

      var best = GetActionAverages(allActions)
         .MaxBy(a => a.Score);

      return best != null ? (best.Target, best.Spell) : null;
   }

   private IEnumerable<ScoredAction> GetActionAverages(IEnumerable<ScoredAction> allActions)
   {
      var groupedActions = allActions
         .GroupBy(a => (a.Spell, a.Target));
         
      var actionAverages = new List<ScoredAction>();

      foreach (var grouping in groupedActions)
      {
         var average = GetAverage(grouping.Select(a => a.Score).ToArray());
         var action = new ScoredAction(grouping.Key.Target, grouping.Key.Spell, average);
         Console.WriteLine($"casting {action.Spell.Kind} at {action.Target.Unit.Name} has average score of {average}");
         actionAverages.Add(action);
      }

      return actionAverages;
   }

   private int GetAverage(int[] scores)
   {
      return scores.Sum() / scores.Length;
   }

   private ScoredAction EvaluateChain(
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

         DebugLogCurrentScore(currentAction, currentCaster, depth);
         currentCaster = combatState.GetNextTurnUnit(currentCaster);

         if (GetWin(combatState.Combatants.Values, currentCaster))
            break;

         currentAction = ChooseRandomTargetAndSpell(currentCaster, combatState.GetAliveUnits());
         depth--;
      }

      Console.WriteLine("End of current chain");
      // calculate the score and return
      return new ScoredAction(firstCasterAction.target, firstCasterAction.spell,
         CalculateTotalScoreForSide(combatState, initSide));
   }

   private static bool GetWin(IEnumerable<UnitState> combatants, UnitState caster)
   {
      return combatants
         .Where(u => u.Health > 0)
         .All(u => u.Side == caster.Side);
   }

   //private static bool TryChooseRandomTargetAndSpell(UnitState caster, IEnumerable<UnitState> availableTargets, out UnitState target, out Spell spell)
   //{
   //   var selectedSpell = UnitBehaviour.SelectRandomSpell(caster);
   //   var allTargets = new List<UnitState>();

   //   foreach (var state in availableTargets)
   //   {
   //      allTargets.Add(state);
   //   }

   //   var selectedTarget = selectedSpell.SpellEffect.IsHarm ? // if operator
   //      UnitBehaviour.SelectRandomEnemy(allTargets, caster)
   //      : UnitBehaviour.SelectRandomAlly(allTargets, caster);

   //   return (selectedTarget, selectedSpell);
   //}

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
            combatState = combatState.ResetRound();

         return combatState;
      }

      combatState = combatState.ExhaustTurn(combatState, caster, log);

      combatState = combatState.CastAndApplySpell(caster, action.target, action.spell, log);

      if (action.target.Health <= 0)
      {
         log.UnitDies(action.target);
      }

      return combatState;
   }

   private IEnumerable<(UnitState target, Spell spell)> GetPossibleActions(CombatState combatState, UnitState caster)
   {
      var allies = combatState
         .GetAliveUnits()
         .Where(u => u.Side == caster.Side).ToArray();

      var enemies = combatState
         .GetAliveUnits()
         .Where(u => u.Side != caster.Side).ToArray();

      foreach (var spell in caster.TimedSpells
                  .Where(spell => spell.CooldownTimer == 0)
                  .ToList())
      {
         switch (spell.Spell.SpellEffect.IsHarm)
         {
            case true when enemies.Length > 0:
            {
               foreach (var target in enemies)
                  yield return (target, spell.Spell);
               break;
            }
            case false when allies.Length > 0:
            {
               foreach (var target in allies)
                  yield return (target, spell.Spell);
               break;
            }
         }
      }
   }
}