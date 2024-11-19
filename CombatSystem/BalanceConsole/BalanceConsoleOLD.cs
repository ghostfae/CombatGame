using System;
using System.Collections.Generic;
using System.Linq;
using CombatEngine;

namespace BalanceConsole;

internal class BalanceConsoleOLD
{
   static void MainOLD(string[] args)
   {
      int mageWins = 0;
      int warriorWins = 0;
      int bothWin = 0;
      int redWin = 0;
      int blueWin = 0;

      for (int i = 0; i < 50; i++)
      {
         var aliveUnits = RunGameInstance();
         var enumerable = aliveUnits.ToList();

         if (enumerable.Count() == 1)
         {
            if (enumerable.First().Unit.Kind == UnitKind.Mage)
            {
               mageWins++;
            }
            else
            {
               warriorWins++;
            }
         }
         else
         {
            bothWin++;
         }

         if (enumerable.First().Side == Side.Blue)
         {
            blueWin++;
         }
         else
         {
            redWin++;
         }
      }

      Console.WriteLine($"Red win {redWin} times and Blue win {blueWin} times.");
      Console.WriteLine($"Mages have won {mageWins} times.");
      Console.WriteLine($"Warriors have won {warriorWins} times.");
      if (bothWin > 0)
      {
         Console.WriteLine($"Both have won {bothWin} times.");
      }
   }


static IEnumerable<UnitState> RunGameInstance()
   {
      //Rng.ReplaceSeed(new Random().Next());
      var combatants = FightBuilder.CreateScenario1V1();

      var combat = new CombatState(combatants);

      return CombatRunner.Run(combat, new BalanceCombatLog(), new NullCombatListener());
   }
}