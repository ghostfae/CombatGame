namespace CombatEngine;

public static class ClassBuilder 
{
    // spell list
    private static Spell SwordHit = new Spell(SpellKind.SwordHit, 0, 10, 10);
    private static Spell ShieldBash = new Spell(SpellKind.ShieldBash, 3, 5, 30);
    private static Spell FrostBolt = new Spell(SpellKind.FrostBolt, 0, 10, 10);
    private static Spell FireSnap = new Spell(SpellKind.FireSnap, 2, 15, 20);

    public static Unit CreateWarrior(Side side)
    {
       return new Unit(
          kind: UnitKind.Warrior,
          initialHealth: 100,
          speed: 6,
          side: side,
          name: NameGenerator.GenerateName(),
            SwordHit,
            ShieldBash);
    }

    public static Unit CreateMage(Side side)
    {
        return new Unit(
            kind: UnitKind.Mage,
            initialHealth: 80,
            speed: 5,
            side: side,
            name: NameGenerator.GenerateName(),
            FrostBolt,
            FireSnap);
    }
}