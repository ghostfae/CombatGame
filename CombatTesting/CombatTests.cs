using CombatEngine;

namespace CombatTesting;

public class Tests
{
   [SetUp]
   public void Setup()
   {
   }

   [Test]
   public void TestCombatAI()
   {
      var classBuilder = new ClassBuilder();

      var combatants = FightBuilder.CreateScenario1V1(classBuilder);
      var combat = new CombatState(combatants);
      var ai = new CombatAI();

      var bestTurn = ai.ChooseNextMove(combatants[0], combat);
      Console.WriteLine($"best turn is to cast {bestTurn.spell.Kind} at {bestTurn.target.Unit.Name}");
   }
}
