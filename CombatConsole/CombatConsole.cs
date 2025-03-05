using Microsoft.Extensions.DependencyInjection;
using CombatEngine;
using System;

namespace CombatConsole;

internal class CombatConsole 
{
   static void Main(string[] args)
   {
      var services = new ServiceCollection();

      services.AddSingleton(_ => FightBuilder.CreateScenario1V1());
      services.AddTransient<CombatState>();
      services.AddSingleton<CombatRunner>();
      services.AddSingleton<INextMoveStrategy, CombatAi>();
      services.AddSingleton<ICombatLog, ConsoleCombatLog>();
      services.AddSingleton<ICombatListener, ConsoleCombatListener>();

      var serviceProvider = services.BuildServiceProvider();

      // Resolve dependencies
      var combatRunner = serviceProvider.GetRequiredService<CombatRunner>();

      // Run the combat simulation
      combatRunner.Run();
   }
}

