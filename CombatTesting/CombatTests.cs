using CombatEngine;

namespace CombatTesting;

public class Tests
{
   [SetUp]
   public void Setup()
   {
   }

   [Test]
   public void TestDamageWorks()
   {
      var combatants = FightBuilder.CreateScenario1v1();
      var combat = new Combat(new Random(1), combatants[0], combatants[1]);
      Assert.Pass();
   }
}
