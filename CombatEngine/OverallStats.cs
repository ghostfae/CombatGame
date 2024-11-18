namespace CombatEngine;

public class ClassStats
{
   public int Wins { get; set; }
   public Dictionary<SpellKind, int> SpellCount { get; set; }

   public ClassStats(IEnumerable<SpellKind> availableSpells)
   {
      Wins = 0;
      SpellCount = availableSpells.ToDictionary(k => k, _ => 0);
   }
}