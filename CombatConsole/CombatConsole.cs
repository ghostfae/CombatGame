using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      var combatants = FightBuilder.CreateScenario1V1();
      var combatState = new CombatState(combatants);
      var combatRunner = new CombatRunner(new CombatAi(), new ConsoleCombatLog(), new ConsoleCombatListener());

      combatRunner.Run(combatState);
   }
}

