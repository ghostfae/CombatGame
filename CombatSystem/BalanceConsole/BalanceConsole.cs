using System;
using System.Collections.Generic;
using System.Linq;
using CombatEngine;

namespace BalanceConsole;

internal class BalanceConsole
{
   static void Main(string[] args)
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
      var combatants = FightBuilder.CreateScenario2V2(new ClassBuilder());

      var combat = new CombatState(combatants);

      return CombatRunner.Run(combat, new NullCombatLog(), new NullCombatListener());
   }
}

internal class NullCombatListener : ICombatListener
{
   public void EndOfRound(int round)
   {
   }
}

internal class NullCombatLog : ICombatLog
{
   public void RoundBegins(int round)
   {
   }

   public void UpkeepBegins()
   {
   }

   public void UpkeepEnds()
   {
   }

   public void ReportSides(IEnumerable<UnitState> units)
   {
   }

   public void Turn(UnitState unit)
   {
   }

   public void CastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {
   }

   public void TakeDamage(UnitState unit, int? amount)
   {
   }

   public void HealDamage(UnitState unit, int? amount)
   {
   }

   public void UnitDies(UnitState unit)
   {
   }

   public void Win(Side winningSide)
   {
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
      string text = "The winner is: ";
      var units = winningUnits.ToList();
      if (units.Count() > 1)
      {
         text = "The winners are: ";
      }
      string winners = "";
      foreach (var unit in units)
      {
         winners += unit.Unit.ToString();
      }
      Console.WriteLine($"{text} {winners}");
      Console.WriteLine($"{units.First().Side}");
      Console.WriteLine();
   }

   public void TotalRounds(int totalRounds)
   {
      Console.WriteLine($"In a total of {totalRounds} rounds...");
   }

   public void Crit(Spell spell)
   {
   }
}