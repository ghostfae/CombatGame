using CombatEngine;

namespace CombatEngine
{
   public interface ICombatLog
   {
      void RoundBegins(int round);

      void UpkeepBegins();

      void UpkeepEnds();

      void ReportSides(IEnumerable<Unit> units);

      void Turn(Unit unit);

      void CastSpell(Unit unit, Unit target, Spell currentSpell, int? amount = null);

      void TakeDamage(Unit unit, int? amount);

      void HealDamage(Unit unit, int? amount);

      void UnitDies(Unit unit);

      void Win(Side winningSide);

      void Winners(IEnumerable<Unit> winningUnits);

      void TotalRounds(int totalRounds);

      void Crit(Spell spell);

   }
   
}
