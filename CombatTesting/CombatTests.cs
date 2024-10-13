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
      var combatants = FightBuilder.CreateScenario1V1();
      //var combat = new CombatState(combatants[0], combatants[1]);
      Assert.Pass();
   }
}
