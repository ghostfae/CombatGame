using System;
using System.Collections.Generic;
using System.Linq;
using CombatEngine;

namespace BalanceConsole;

internal class BalanceCombatLog : ICombatLog
{
   public void LogRoundBegins(int round)
   {
   }

   public void LogUpkeepBegins()
   {
   }

   public void LogUpkeepEnds()
   {
   }

   public void LogReportSides(IEnumerable<UnitState> units)
   {
   }

   public void LogTurn(UnitState unit)
   {
   }

   public void LogCastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {
   }

   public void LogTakeDamage(UnitState unit, int? amount)
   {
   }

   public void LogHealDamage(UnitState unit, int? amount)
   {
   }

   public void LogUnitDies(UnitState unit)
   {
   }

   public void LogWin(Side winningSide)
   {
   }

   public void LogWinners(IEnumerable<UnitState> winningUnits)
   {
      var text = "The winner is: ";
      var units = winningUnits.ToList();
      if (units.Count > 1)
      {
         text = "The winners are: ";
      }
      var winners = string.Empty;
      foreach (var unit in units)
      {
         winners += unit.Unit.ToString();
      }
      Console.WriteLine($"{text} {winners}");
      Console.WriteLine($"{units.First().Side}");
      Console.WriteLine();
   }

   public void LogTotalRounds(int totalRounds)
   {
      Console.WriteLine($"In a total of {totalRounds} rounds...");
   }

   public void LogCrit(Spell spell)
   {
   }
}