namespace CombatEngine;

/// <summary>
/// logs each stage of a round and every action taken within the round
/// </summary>
public class ConsoleCombatLog : ICombatLog
{
   public void LogRoundBegins(int round)
   {
      Console.WriteLine();
      Console.WriteLine($"BEGIN ROUND {round}");
      Console.WriteLine();
   }

   public void LogUpkeepBegins() 
   {
      Console.WriteLine($"Upkeep begins.");
   }

   public void LogUpkeepEnds()
   {
      Console.WriteLine($"Upkeep ends.");
      Console.WriteLine();
   }

   public void LogReportSides(IEnumerable<UnitState> units)
   {
      var sides = units
         .GroupBy(unit => unit.Side)
         .Select(group => new
         {
            Side = group.Key,
            Units = group.OrderByDescending(unit => unit.Unit.Speed).ToList()
         });

      foreach (var side in sides)
      {
         Console.WriteLine($"Side {side.Side}:");
         foreach (var unit in side.Units)
         {
            string canAct = unit.CanAct ? "can act" : "can't act";
            Console.WriteLine($"{unit.Unit} has speed of {unit.Unit.Speed} and {unit.Health} health. They {canAct}.");
         }
      }
   }

   public void LogTurn(UnitState unit)
   {
      Console.WriteLine();
      Console.WriteLine($"It is {unit.Unit}'s turn.");
   }

   public void LogCastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {

      if (amount.HasValue && currentSpell.SpellEffect.IsHarm)
      {
         Console.WriteLine($"{unit.Unit} cast {currentSpell.Kind} for {amount} damage at {target.Unit}.");
      }
      else if (amount.HasValue && !currentSpell.SpellEffect.IsHarm)
      {
         Console.WriteLine($"{unit.Unit} cast {currentSpell.Kind} for {amount} healing at {target.Unit}.");
      }
      else
      {
         Console.WriteLine($"{unit.Unit} cast {currentSpell.Kind} at {target.Unit}.");
      }
   }

   public void LogTakeDamage(UnitState unit, int? amount)
   {
      if (amount.HasValue)
      {
         Console.WriteLine($"{unit.Unit} was hit for {amount} damage and has {unit.Health} health remaining.");
      }
   }

   public void LogHealDamage(UnitState unit, int? amount)
   {
      if (amount.HasValue)
      {
         Console.WriteLine($"{unit.Unit} was healed for {amount} and has {unit.Health} health remaining.");
      }
   }

   public void LogWin(Side winningSide)
   {
      Console.WriteLine($"{winningSide} wins!");
   }

   public void LogWinners(IEnumerable<UnitState> winningUnits)
   {
      Console.WriteLine($"The winners are:");
      foreach (var unit in winningUnits)
      {
         Console.WriteLine($"{unit.Unit}");
      }
   }

   public void LogTotalRounds(int totalRounds)
   {
   }

   public void LogUnitDies(UnitState unit)
   {
      Console.WriteLine($"{unit.Unit} dies.");
   }

   public void LogCrit(Spell spell)
   {
      Console.WriteLine($"{spell.Kind} has crit!");
   }
}