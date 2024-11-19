using System.Collections.Generic;
using CombatEngine;

namespace BalanceConsole;

internal class NullCombatListener : ICombatListener
{
   public void CastSpell(UnitKind caster, SpellKind spell)
   {
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
   }

   public void EndOfRound(int round)
   {
   }
}