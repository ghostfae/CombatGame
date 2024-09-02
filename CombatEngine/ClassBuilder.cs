namespace CombatEngine;

public static class ClassBuilder 
{
    // spell list
    private static Spell SwordHit = new Spell(SpellKind.WSwordHit, 0, 10, 10);
    private static Spell ShieldBash = new Spell(SpellKind.WShieldBash, 3, 10, 10);
    private static Spell FrostBolt = new Spell(SpellKind.MFrostBolt, 0, 10, 10);
    private static Spell FireSnap = new Spell(SpellKind.MFireSnap, 2, 5, 5);

    public static Unit CreateWarrior() 
    {
        return new Unit(
            kind: UnitKind.Warrior, 
            health: 100, 
            speed: 6, 
            SwordHit,
            ShieldBash);
    }

    public static Unit CreateMage()
    {
        return new Unit(
            kind: UnitKind.Mage,
            health: 80,
            speed: 5,
            FrostBolt,
            FireSnap);
    }
}