namespace CombatEngine;

/// <summary>
/// AI that decided what actions to take
/// </summary>
public static class UnitBehaviour
{
   public static Spell SelectSpell(Unit unit) 
   {
      var readySpells = unit.TimedSpells
         .Where(spell => spell.CooldownTimer == 0)
         .ToList();
      /*return readySpells
         .OrderByDescending(spell => spell.Spell.MaxDamage)
         .First()
         .Spell;*/
      var max = readySpells.Count - 1;
      var randomSelect = Rng.Random.Next(Math.Max(0, max));
      return readySpells[randomSelect].Spell;
   }

   public static Unit SelectEnemy(IEnumerable<Unit> availableTargets, Unit self)
   {
      var enemies = availableTargets.Where(t => t.Side != self.Side).ToArray();
      return enemies[new Random(1).Next(enemies.Length - 1)];
   }

   public static Unit SelectAlly(IEnumerable<Unit> availableTargets, Unit self)
   {
      var allies = availableTargets.Where(t => t.Side == self.Side).ToArray();
      return allies[new Random(1).Next(allies.Length - 1)];
   }
}
