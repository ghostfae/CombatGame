using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      //Rng.ReplaceSeed(new Random().Next());

      var combatants = FightBuilder.CreateScenario2V2();
      var combat = new Combat(new ConsoleCombatListener(), new ConsoleCombatLog(), combatants);
      CombatRunner.Run(combat);
   }
}
