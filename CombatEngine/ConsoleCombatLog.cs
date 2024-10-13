namespace CombatEngine;

/// <summary>
/// logs each stage of a round and every action taken within the round
/// </summary>
public class ConsoleCombatLog : ICombatLog
{
   public void RoundBegins(int round)
   {
      Console.WriteLine();
      Console.WriteLine($"BEGIN ROUND {round}");
      Console.WriteLine();
   }

   public void UpkeepBegins() 
   {
      Console.WriteLine($"Upkeep begins.");
   }

   public void UpkeepEnds()
   {
      Console.WriteLine($"Upkeep ends.");
      Console.WriteLine();
   }

   public void ReportSides(IEnumerable<UnitState> units)
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

   public void Turn(UnitState unit)
   {
      Console.WriteLine();
      Console.WriteLine($"It is {unit.Unit}'s turn.");
   }

   public void CastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null)
   {
      //var type = new string("");

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

   public void TakeDamage(UnitState unit, int? amount)
   {
      if (amount.HasValue)
      {
         Console.WriteLine($"{unit.Unit} was hit for {amount} damage and has {unit.Health - amount} health remaining.");
      }
   }

   public void HealDamage(UnitState unit, int? amount)
   {
      if (amount.HasValue)
      {
         Console.WriteLine($"{unit.Unit} was healed for {amount} and has {unit.Health + amount} health remaining.");
      }
   }

   public void Win(Side winningSide)
   {
      Console.WriteLine($"{winningSide} wins!");
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
      Console.WriteLine($"The winners are:");
      foreach (var unit in winningUnits)
      {
         Console.WriteLine($"{unit.Unit}");
      }
   }

   public void TotalRounds(int totalRounds)
   {
   }

   public void UnitDies(UnitState unit)
   {
      Console.WriteLine($"{unit.Unit} dies.");
   }

   public void Crit(Spell spell)
   {
      Console.WriteLine($"{spell.Kind} has crit!");
   }
}