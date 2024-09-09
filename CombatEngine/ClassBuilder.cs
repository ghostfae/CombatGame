namespace CombatEngine;

public static class ClassBuilder 
{
    public static Unit CreateWarrior(Side side)
    {
       return new Unit(
          kind: UnitKind.Warrior,
          initialHealth: 100,
          speed: 6,
          side: side,
          name: NameGenerator.GenerateName(),
            SpellBook.CreateSwordHit(),
          SpellBook.CreateShieldBash(),
          SpellBook.CreateHealthPotion());
    }

    public static Unit CreateMage(Side side)
    {
        return new Unit(
            kind: UnitKind.Mage,
            initialHealth: 80,
            speed: 5,
            side: side,
            name: NameGenerator.GenerateName(),
            SpellBook.CreateFrostBolt(),
            SpellBook.CreateFireSnap(), 
            SpellBook.CreateHealthPotion(),
            SpellBook.CreateHealthDurationPotion());
   }
}