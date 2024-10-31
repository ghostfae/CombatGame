namespace CombatEngine;

/// <summary>
/// AI that decides what actions to take
/// </summary>
public static class UnitBehaviour
{
   public static Spell SelectRandomSpell(UnitState unit) 
   {
      var readySpells = unit.TimedSpells
         .Where(spell => spell.CooldownTimer == 0)
         .ToList();
      var max = readySpells.Count - 1;
      var randomSelect = Rng.Random.Next(Math.Max(0, max));
      return readySpells[randomSelect].Spell;
   }

   public static UnitState SelectRandomEnemy(IEnumerable<UnitState> availableTargets, UnitState self)
   {
      var enemies = availableTargets.Where(t => t.Side != self.Side).ToArray();
      return enemies[Rng.Random.Next(enemies.Length)];
   }

   public static UnitState SelectRandomAlly(IEnumerable<UnitState> availableTargets, UnitState self)
   {
      var allies = availableTargets.Where(t => t.Side == self.Side).ToArray();
      return allies[new Random(1).Next(allies.Length)];
   }
}
