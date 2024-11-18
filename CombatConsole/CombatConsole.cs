using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      //Rng.ReplaceSeed(new Random().Next());

      var combatants = FightBuilder.CreateScenario1V1();
      var combat = new CombatState(combatants);
      CombatRunner.Run(combat, new ConsoleCombatLog(), new ConsoleCombatListener());
   }
}

