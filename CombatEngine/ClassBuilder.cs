namespace CombatEngine;

/// <summary>
/// a factory that builds units by combat types and sets their battle side
/// </summary>
public static class ClassBuilder
{
   public static int Uid = 1;

   public static Spell[] WarriorSpells = 
   [
      SpellBook.CreateSwordHit(),
      SpellBook.CreateShieldBash(),
      //SpellBook.CreateHealthPotion()
   ];

   public static Spell[] MageSpells =
   [
      SpellBook.CreateFrostBolt(),
      SpellBook.CreateFireSnap(),
      //SpellBook.CreateHealthPotion()
   ];

   public static Dictionary<UnitKind, Spell[]> ClassSpells = new ()
   {
      { UnitKind.Warrior, WarriorSpells },
      { UnitKind.Mage, MageSpells}
   };

   public static Unit CreateWarrior(Side side)
   {
      return new Unit(
         uid: Uid++,
         kind: UnitKind.Warrior,
         initialHealth: 250,
         speed: 6,
         side: side,
         name: NameGenerator.GenerateName(),
         WarriorSpells);
   }

   public static Unit CreateMage(Side side)
   {
      return new Unit(
         uid: Uid++,
         kind: UnitKind.Mage,
         initialHealth: 200,
         speed: 5,
         side: side,
         name: NameGenerator.GenerateName(),
         MageSpells);
   }
}