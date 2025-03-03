namespace CombatEngine;

/// <summary>
/// AI that decides what actions to take
/// </summary>
public static class UnitBehaviour
{
   public static TimedSpell[] ReadySpells(this UnitState unit)
   {
      return unit.TimedSpells
         .Where(spell => spell.CooldownTimer == 0)
         .ToArray();
   }

   public static Spell SelectRandomSpell(UnitState unit)
   {
      var readySpells = unit.ReadySpells();
      var randomSelect = Rng.Random.Next(Math.Max(0, readySpells.Length - 1));
      return readySpells[randomSelect].Spell;
   }

   public static UnitState SelectRandomEnemy(IEnumerable<UnitState> availableTargets, UnitState self)
   {
      var enemies = availableTargets.Where(t => t.Side != self.Side).ToArray();
      return enemies[Rng.Random.Next(enemies.Length)];
   }

   public static UnitState SelectRandomAlly(IEnumerable<UnitState> availableTargets, UnitState self)
   {
      var allies = availableTargets.Where(t => t.Side == self.Side).Append(self).ToArray();
      return allies[Rng.Random.Next(allies.Length)];
   }
}
