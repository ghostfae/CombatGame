using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      //Rng.ReplaceSeed(new Random().Next());

      var combatants = FightBuilder.CreateScenario1V1();
      var combatState = new CombatState(combatants);
      var combatRunner = new CombatRunner(new ConsoleCombatLog());
      combatRunner.Run(combatState, new ConsoleCombatListener());
   }
}

