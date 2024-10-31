using System.Text;
using System.Xml.Linq;

namespace CombatEngine;

public struct Turn(UnitState target, int score, Spell? spell, CombatState newState)
{
   public UnitState Target = target;
   public int Score = score;
   public Spell? Spell = spell;
   public CombatState NewState = newState;

   public override string ToString()
   {
      return $"target = {Target}, score = {Score}, spell = {Spell}";
   }
}

public class AlliesAndEnemies
{
   private readonly IReadOnlyDictionary<bool, UnitState[]> _combatants;

   public UnitState[]? Allies => _combatants.GetValueOrDefault(true);
   public UnitState[]? Enemies=> _combatants.GetValueOrDefault(false);

   private AlliesAndEnemies(IReadOnlyDictionary<bool, UnitState[]> combatants)
   {
      _combatants = combatants;
   }

   public static AlliesAndEnemies Create(Side side, CombatState combatState)
   {
      var combatants = combatState
         .GetAliveUnits()
         .GroupBy(unit => unit.Side == side)
         .ToDictionary(g => g.Key, g => g.ToArray());
      return new AlliesAndEnemies(combatants);
   }
}

public static class CombatAI
{
   public static IEnumerable<(UnitState target, Spell spell)> GetPossibleActions(CombatState combatState, UnitState caster)
   {
      var alliesAndEnemies = AlliesAndEnemies.Create(caster.Side, combatState);

      foreach (var spell in caster.ReadySpells())
      {
         // helpful and harmful spells should be treated differently

         switch (spell.Spell.SpellEffect.IsHarm)
         {
            case true when alliesAndEnemies.Enemies is { Length: > 0 } enemies:
            {
               foreach (var target in enemies)
                  yield return (target, spell.Spell);
               break;
            }
            case false when alliesAndEnemies.Allies is { Length: > 0 } allies:
            {
               foreach (var target in allies)
                  yield return (target, spell.Spell);
               break;
            }
         }
      }
   }


   public static (UnitState target, Spell spell) SimulateCombatChain(CombatState combatState, UnitState caster)
   {
      int maxTests = 10;
      int depth = 6;
      var firstCaster = caster;
      var firstState = combatState;
      var firstTurns = new List<Turn>();


      for (int i = 0; i < maxTests; i++)
      {
         var isFirstTurn = true;
         caster = firstCaster;
         combatState = firstState;

         for (int x = 0; x < depth; x++)
         {
            var scoredTurn = CalculateFinalScores(combatState, caster);

            if (caster.Side != firstCaster.Side)
            {
               scoredTurn.Score *= -1;
            }

            if (isFirstTurn)
            {
               firstTurns.Add(scoredTurn);
               isFirstTurn = false;
            }

            caster = scoredTurn.NewState.GetNextUnit();
            combatState = scoredTurn.NewState;

            if (combatState.TryGetWinningSide != null) break;
         }
      }

      var best = firstTurns.OrderBy(t => t.Score).First();
      return (best.Target, best.Spell)!;
   }


   public static List<Turn> SimulateSingleCombat(CombatState combatState, UnitState caster)
   {
      var simulatedResults = new List<Turn>();
      var maxTests = 100;
      // todo: convert to linq
      for (int i = 0; i < maxTests; i++)
      {
         simulatedResults.Add(CalculateFinalScores(combatState, caster));
      }

      return simulatedResults;
   }

   public static int CalculateTotalScore(CombatState oldState, CombatState newState, UnitState caster)
   {
      var allies = newState.Combatants.Where(u => u.Value.Side == caster.Side).ToArray();

      var enemies = newState.Combatants.Where(u => u.Value.Side != caster.Side).ToArray();

      int allyDiff = 0;
      foreach (var ally in allies)
      {
         allyDiff += CalculateUnitScore(oldState, newState, ally.Value);
      }

      int enemyDiff = 0;
      foreach (var enemy in enemies)
      {
         enemyDiff += CalculateUnitScore(oldState, newState, enemy.Value);
      }

      return allyDiff - enemyDiff;
   }

   public static int CalculateUnitScore(CombatState oldState, CombatState newState, UnitState unit)
   {
      var oldHealth = oldState.Combatants.First(u => u.Key == unit.Unit.Uid).Value.Health;
      var newHealth = newState.Combatants.First(u => u.Key == unit.Unit.Uid).Value.Health;

      // if health has increased, diff will be positive; if health has decreased, diff will be negative
      return newHealth - oldHealth;
   }

   public static Turn CalculateFinalScores(CombatState combatState, UnitState self)
   {
      if (self.CanAct == false) return new Turn(combatState.GetNextUnit(), 0, null, combatState);

      var (newState, target, spell) = CombatRunner.PerformRandomTurn(combatState, self, new ConsoleEmptyLog());
      var score = CalculateTotalScore(combatState, newState, self);
      return new Turn(target, score, spell, newState);
   }

   public static int MiniMax(int node, int depth, bool isMaxPlayer, int alpha, int beta) // (UnitState target, int diff) [] scores
   {
      if (depth == 0) return node; // return node VALUE

      if (isMaxPlayer)
      {
         var bestVal = -1000;
         for (int i = depth; i >= 0; i--)
         {
            var val = MiniMax(node, depth - 1, false, alpha, beta);
            bestVal = Math.Max(bestVal, val);
            alpha = Math.Max(alpha, bestVal);
            if (beta <= alpha) break;
         }
         return bestVal;
      }
      else
      {
         var bestVal = 1000;
         for (int i = depth; i >= 0; i--)
         {
            var val = MiniMax(node, depth - 1, true, alpha, beta);
            bestVal = Math.Max(bestVal, val);
            alpha = Math.Max(alpha, bestVal);
            if (beta <= alpha) break;
         }
         return bestVal;
      }
   }
}