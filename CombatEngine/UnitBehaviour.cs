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
      return readySpells
         .OrderByDescending(spell => spell.Spell.MaxDamage)
         .First()
         .Spell;
      //EXTENSION: add healing spells?
   }

   public static Unit SelectEnemy(IEnumerable<Unit> availableTargets, Unit self)
   {
      var enemies = availableTargets.Where(t => t.Side != self.Side).ToArray();
      return enemies.First();
   }
}
