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
   public void TestBestTurn()
   {
      var classBuilder = new ClassBuilder();

      var combatants = FightBuilder.CreateScenario1V1(classBuilder);
      var combat = new CombatState(combatants);
      var list = CombatAI.SimulateSingleCombat(combat, combatants.First());
      var bestCombo = list.MaxBy(v => v.Score);
      Console.WriteLine(bestCombo);
   }

   [Test]
   public void TestCombatAI()
   {
      var classBuilder = new ClassBuilder();

      var combatants = FightBuilder.CreateScenario1V1(classBuilder);
      var combat = new CombatState(combatants);
      var best = CombatAI.SimulateCombatChain(combat, combatants.First());
      Console.WriteLine($"{best.target.Unit}, {best.spell.Kind}");
   }
}
