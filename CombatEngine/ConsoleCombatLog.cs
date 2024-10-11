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

   public void ReportSides(IEnumerable<Unit> units)
   {
      var sides = units
         .GroupBy(unit => unit.Side)
         .Select(group => new
         {
            Side = group.Key,
            Units = group.OrderByDescending(unit => unit.Speed).ToList()
         });

      foreach (var side in sides)
      {
         Console.WriteLine($"Side {side.Side}:");
         foreach (var unit in side.Units)
         {
            string canAct = unit.CanAct() ? "can act" : "can't act";
            Console.WriteLine($"{unit} has speed of {unit.Speed} and {unit.CurrentHealth} health. They {canAct}.");
         }
      }
   }

   public void Turn(Unit unit)
   {
      Console.WriteLine();
      Console.WriteLine($"It is {unit}'s turn.");
   }

   public void CastSpell(Unit unit, Unit target, Spell currentSpell, int? amount = null)
   {
      //var type = new string("");

      if (amount.HasValue && currentSpell.SpellEffect.IsHarm)
      {
         Console.WriteLine($"{unit} cast {currentSpell.Kind} for {amount} damage at {target}.");
      }
      else if (amount.HasValue && !currentSpell.SpellEffect.IsHarm)
      {
         Console.WriteLine($"{unit} cast {currentSpell.Kind} for {amount} healing at {target}.");
      }
      else
      {
         Console.WriteLine($"{unit} cast {currentSpell.Kind} at {target}.");
      }
   }

   public void TakeDamage(Unit unit, int? amount)
   {
      if (amount.HasValue)
      {
         Console.WriteLine($"{unit} was hit for {amount} damage and has {unit.CurrentHealth} health remaining.");
      }
   }

   public void HealDamage(Unit unit, int? amount)
   {
      if (amount.HasValue)
      {
         Console.WriteLine($"{unit} was healed for {amount} and has {unit.CurrentHealth} health remaining.");
      }
   }

   public void Win(Side winningSide)
   {
      Console.WriteLine($"{winningSide} wins!");
   }

   public void Winners(IEnumerable<Unit> winningUnits)
   {
      Console.WriteLine($"The winners are:");
      foreach (var unit in winningUnits)
      {
         Console.WriteLine($"{unit}");
      }
   }

   public void TotalRounds(int totalRounds)
   {
   }

   public void UnitDies(Unit unit)
   {
      Console.WriteLine($"{unit} dies.");
   }

   public void Crit(Spell spell)
   {
      Console.WriteLine($"{spell.Kind} has crit!");
   }
}