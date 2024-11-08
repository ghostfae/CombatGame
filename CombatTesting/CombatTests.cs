using CombatEngine;

namespace CombatTesting;

public class Tests
{
   [Test]
   public void TestCombatAIDefault()
   {
      var classBuilder = new ClassBuilder();

      var combatants = FightBuilder.CreateScenario1V1(classBuilder);
      var combat = new CombatState(combatants);

      // default depth = 4, maxActions = 100
      var ai = new CombatAI();

      var bestTurn = ai.ChooseNextMove(combatants[0], combat);
      Console.WriteLine($"best turn is to cast {bestTurn!.Value.spell.Kind} at {bestTurn.Value.target.Unit.Name}");

      //best turn is ShieldBash on Zila
      Assert.That(bestTurn.Value.target.Unit.Name == "Zila" && bestTurn.Value.spell.Kind == SpellKind.ShieldBash);
   }
}
