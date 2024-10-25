using System.Text;

namespace CombatEngine;

/// <summary>
/// engine that deals with all the actions taken within combat,
/// handles unit turns and rounds
/// </summary>
///
public static class CombatRunner
{
   public static CombatState PerformTurn(CombatState combatState, UnitState caster, ICombatLog log)
   {
      log.Turn(caster);

      caster = caster.Tick().ExhaustTurn();
      combatState = combatState.CloneWith(caster);

      var (target, spell) = caster.Unit.SelectBestMove(combatState, caster);

      combatState = combatState.CastAndApplySpell(caster, target, spell, log);

      if (target.Health <= 0)
      {
         log.UnitDies(target);
      }

      return combatState;
   }

   public static (CombatState combatState, UnitState target, Spell spell) PerformRandomTurn(CombatState combatState, UnitState caster, ICombatLog log)
   {
      log.Turn(caster);

      caster = caster.Tick().ExhaustTurn();
      combatState = combatState.CloneWith(caster);

      var (target, spell) = caster.Unit.ChooseRandomTargetAndSpell(combatState.GetAliveUnits());

      combatState = combatState.CastAndApplySpell(caster, target, spell, log);

      if (target.Health <= 0)
      {
         log.UnitDies(target);
      }

      return (combatState, target, spell);
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

         var unit = combatState.TryGetNextUnit();
         while (unit != null)
         {
            combatState = PerformTurn(combatState, unit, log);
            unit = combatState.TryGetNextUnit();

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

   public static List<(UnitState target, int diff, Spell spell)> SimulateCombat(CombatState combatState, UnitState caster)
   {
      var simulatedResults = new List<(UnitState target, int diff, Spell spell)>();
      var maxTests = 100;
      for (int i = 0; i < maxTests; i++)
      {
         simulatedResults.Add(CalculateScores(combatState, caster));
      }

      return simulatedResults;
   }

   public static int CalculateTotalDiff(CombatState oldState, CombatState newState, UnitState caster)
   {
      var allies = newState.Combatants.Where(u => u.Value.Side == caster.Side).ToArray();

      var enemies = newState.Combatants.Where(u => u.Value.Side != caster.Side).ToArray();

      int allyDiff = 0;
      foreach (var ally in allies)
      {
         allyDiff += CalculateUnitDiff(oldState, newState, ally.Value);
      }

      int enemyDiff = 0;
      foreach (var enemy in enemies)
      {
         enemyDiff += CalculateUnitDiff(oldState, newState, enemy.Value);
      }

      return allyDiff - enemyDiff;
   }

   public static int CalculateUnitDiff(CombatState oldState, CombatState newState, UnitState unit)
   {
      var oldHealth = oldState.Combatants.First(u => u.Key == unit.Unit.Uid).Value.Health;
      var newHealth = newState.Combatants.First(u => u.Key == unit.Unit.Uid).Value.Health;

      // if health has increased, diff will be positive; if health has decreased, diff will be negative
      return newHealth - oldHealth;
   }

   public static (UnitState target, int diff, Spell spell) CalculateScores(CombatState combatState, UnitState self)
   {
      var (newState, target, spell) = PerformRandomTurn(combatState, self, new ConsoleEmptyLog());
      var diff = CalculateTotalDiff(combatState, newState, self);
      return (target, diff, spell);
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