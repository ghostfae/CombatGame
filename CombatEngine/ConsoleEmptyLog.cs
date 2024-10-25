namespace CombatEngine;

/// <summary>
/// blank console log
/// </summary>
public class ConsoleEmptyLog : ICombatLog
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

   public void Win(Side winningSide)
   {
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
   }

   public void TotalRounds(int totalRounds)
   {
   }

   public void UnitDies(UnitState unit)
   {
   }

   public void Crit(Spell spell)
   {
   }
}