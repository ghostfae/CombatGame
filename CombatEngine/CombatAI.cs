using System.Diagnostics;
using System.Drawing;

namespace CombatEngine;

public class CombatAI : INextMoveStrategy
{
   private const int depth = 4;
   public (UnitState target, Spell spell) ChooseNextMove(UnitState caster, CombatState combatState)
   {
      var possibleActions = GetPossibleActions(combatState, caster);
      var allActions = new Dictionary<(UnitState, Spell), int>();

      foreach (var action in possibleActions)
      {
         allActions.Add(action, 0);
         allActions[action] = GetScoreForChain(action, combatState, combatState, caster, depth, caster.Side);
         Debug.WriteLine($"targeting {action.target.Unit.Name} with {action.spell.Kind} has a score of {allActions[action]} \n");
      }
      
      var best = allActions
         .OrderByDescending(a => a.Value)
         .FirstOrDefault();
      return best.Key;
   }

   private int GetScoreForChain((UnitState target, Spell spell) action, CombatState startCombatState,
      CombatState newCombatState, UnitState caster, int depth, Side initSide, int currentScore = 0)
   {
      var oldTurnState = newCombatState;
      newCombatState = ApplyTurn(action, newCombatState, caster, new ConsoleEmptyLog());
      if (depth-- > 0)
      {
         currentScore = CalculateTotalScore(oldTurnState, newCombatState, initSide);

         Debug.WriteLine($"caster is {caster.Unit.Name}, target is {action.target.Unit.Name}, spell is {action.spell.Kind}," +
                           $"current score is {currentScore}");
         caster = newCombatState.GetNextUnit(caster);
         var newAction = caster.Unit.ChooseTargetAndSpell(newCombatState.GetAliveUnits());
         return GetScoreForChain(newAction, startCombatState, newCombatState, caster, depth, initSide, currentScore);
      }

      Debug.WriteLine("End of current chain \n");
      // calculate the score and return
      return CalculateTotalScore(startCombatState, newCombatState, initSide);
   }

   private static int CalculateTotalScore(CombatState oldState, CombatState newState, Side initSide)
   {
      var allies = newState.Combatants.Where(u => u.Value.Side == initSide).ToArray();

      var enemies = newState.Combatants.Where(u => u.Value.Side != initSide).ToArray();

      int allyScore = 0;
      foreach (var ally in allies)
      {
         allyScore += CalculateUnitScore(oldState, newState, ally.Value);
      }

      int enemyScore = 0;
      foreach (var enemy in enemies)
      {
         enemyScore += CalculateUnitScore(oldState, newState, enemy.Value);
      }

      return allyScore - enemyScore;
   }

   private static int CalculateUnitScore(CombatState oldState, CombatState newState, UnitState unit)
   {
      var oldHealth = oldState.Combatants.First(u => u.Key == unit.Unit.Uid).Value.Health;
      var newHealth = newState.Combatants.First(u => u.Key == unit.Unit.Uid).Value.Health;

      // if health has increased, diff will be positive; if health has decreased, diff will be negative
      return newHealth - oldHealth;
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