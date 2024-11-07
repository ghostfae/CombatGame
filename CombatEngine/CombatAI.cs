namespace CombatEngine;

public record ScoredAction(UnitState Target, Spell Spell, int Score)
{ }

public class CombatAI : INextMoveStrategy
{
   private const int Depth = 4;

   public (UnitState target, Spell spell)? ChooseNextMove(UnitState caster, CombatState combatState)
   {
      var possibleActions = GetPossibleActions(combatState, caster);

      var allActions = new List<ScoredAction>();

      foreach (var action in possibleActions)
      {
         var score = GetScoreForChain(action, combatState, combatState, caster, Depth, caster.Side);
         var scoredAction = new ScoredAction(action.target, action.spell, score);
         allActions.Add(scoredAction);
         Console.WriteLine($"Target: {scoredAction.Target.Unit.Name}, Spell: {scoredAction.Spell.Kind}, Score: {score}\n");
      }

      var best = allActions
         .OrderByDescending(a => a.Score)
         .FirstOrDefault();

      return (best.Target, best.Spell);
   }

   //private void 

   private int GetScoreForChain(
      (UnitState target, Spell spell) action,
      CombatState startCombatState,
      CombatState newCombatState,
      UnitState caster,
      int depth,
      Side initSide)
   {
      newCombatState = ApplyTurn(action, newCombatState, caster, ConsoleEmptyLog.Instance);

      if (depth == 0)
      {
         Console.WriteLine("End of current chain");
         // calculate the score and return
         return CalculateTotalScoreForSide(newCombatState, initSide);
      }

      DebugLogCurrentScore(action, caster, depth);

      caster = newCombatState.GetNextUnit(caster);
      var newAction = caster.Unit.ChooseTargetAndSpell(newCombatState.GetAliveUnits());

      return GetScoreForChain(
         newAction, 
         startCombatState, 
         newCombatState, 
         caster, 
         depth - 1,
         initSide);
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