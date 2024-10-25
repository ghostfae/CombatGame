using CombatEngine;

namespace CombatTesting;

public class Tests
{
   [SetUp]
   public void Setup()
   {
   }

   [Test]
   public void TestFight()
   {
      var classBuilder = new ClassBuilder();

      var combatants = FightBuilder.CreateScenario1V1(classBuilder);
      var combat = new CombatState(combatants);
      CombatRunner.Run(combat, new ConsoleCombatLog(), new ConsoleCombatListener());
   }

   [Test]
   public void TestAttempt()
   {
      var classBuilder = new ClassBuilder();

      var combatants = FightBuilder.CreateScenario1V1(classBuilder);
      var combat = new CombatState(combatants);
      var list = CombatRunner.SimulateCombat(combat, combatants.First());
      var bestCombo = list.MaxBy(v => v.diff);
      Console.WriteLine(bestCombo);
   }
}
