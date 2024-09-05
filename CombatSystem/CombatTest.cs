namespace CombatEngine;

[TestClass]
public class CombatTest
{
    [TestMethod]
    public void TestDamageWorks()
    {
       Combatant hero = CreateHero();
       Combatant punchBag = CreatePunchBag();

       Combat combat = new Combat(hero, punchBag, new Random(1));
       int heroHealth = hero.Health;
       int punchHealth = punchBag.Health;

       combat.Turn();
       Assert.That(hero.Health, Is.EqualTo(heroHealth));
       Assert.That(punchBag.Health, Is.LessThan(punchHealth));
   }

    [Test]
    public void Test2()
    {
       Combatant hero = CreateHero();
       Combatant punchBag = CreatePunchBag();

       Combat combat = new Combat(hero, punchBag, new Random(1));
       int heroHealth = hero.Health;

       while (combat.Turn().CanCombatContinue)
       {
       }

       Assert.That(hero.Health, Is.EqualTo(heroHealth));
       Assert.That(punchBag.Health, Is.LessThanOrEqualTo(0));
    }
}