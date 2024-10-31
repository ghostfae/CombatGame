using CombatEngine;
using System.Collections.Generic;

namespace CombatEngine
{
   public interface ICombatLog
   {
      void RoundBegins(int round);

      void UpkeepBegins();
      void UpkeepEnds();

      void ReportSides(IEnumerable<UnitState> units);

      void Turn(UnitState unit);

      void CastSpell(UnitState unit, UnitState target, Spell currentSpell, int? amount = null);

      void TakeDamage(UnitState unit, int? amount);

      void HealDamage(UnitState unit, int? amount);

      void UnitDies(UnitState unit);

      void Win(Side winningSide);

      void Winners(IEnumerable<UnitState> winningUnits);

      void TotalRounds(int totalRounds);

      void Crit(Spell spell);

   }
   
}
