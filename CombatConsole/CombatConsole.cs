using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      var combatants = FightBuilder.CreateScenario1v1();

      var combat = new Combat(new Random(1), combatants);

      combat.Run();

   }
}
