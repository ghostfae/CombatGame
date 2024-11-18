namespace CombatEngine;

public class ClassStats1v1
{
   public int Wins { get; set; }
   public Dictionary<SpellKind, int> SpellCount { get; set; }

   public ClassStats1v1(IEnumerable<SpellKind> availableSpells)
   {
      Wins = 0;
      SpellCount = availableSpells.ToDictionary(k => k, _ => 0);
   }
}