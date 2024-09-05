using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      var combatants = FightBuilder.CreateScenario2V2();

      var combat = new Combat( combatants);

      combat.Run();
   }
}
