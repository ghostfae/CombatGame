﻿using System;
using CombatEngine;
using System.Collections.Generic;
using System.Linq;

namespace BalanceConsole;

internal class BalanceConsole : ICombatListener
{
   private readonly Dictionary<UnitKind, ClassStats> _stats = new();

   public void Run()
   {
      var combatants = FightBuilder.CreateScenario1V1();

      var combat = new CombatState(combatants);

      for (int i = 0; i < 50; i++)
      {
         CombatRunner.Run(combat, new ConsoleEmptyLog(), this); // callback
      }

      Console.WriteLine($"Mage has won {_stats[UnitKind.Mage].Wins} times");
      Console.WriteLine($"Spells cast are:");
      foreach (var (spellKind, times) in _stats[UnitKind.Mage].SpellCount)
      {
         Console.WriteLine($"{spellKind} has been cast {times} times");
      }

      Console.WriteLine($"Warrior has won {_stats[UnitKind.Warrior].Wins} times");
      Console.WriteLine($"Spells cast are:");
      foreach (var (spellKind, times) in _stats[UnitKind.Warrior].SpellCount)
      {
         Console.WriteLine($"{spellKind} has been cast {times} times");
      }
   }

   public void CastSpell(UnitKind caster, SpellKind spell)
   {
      GetOrCreateStatsForCaster(caster)
         .SpellCount[spell]++;
   }

   public void Winners(IEnumerable<UnitState> winners)
   {
      foreach (var winner in winners)
      {
         GetOrCreateStatsForCaster(winner.Unit.Kind)
            .Wins++;
      }
   }

   public void EndOfRound(int round)
   {
   }

   private ClassStats GetOrCreateStatsForCaster(UnitKind caster)
   {
      return _stats.GetOrCreate(caster, () => CreateStatsForClass(caster));
   }

   private static ClassStats CreateStatsForClass(UnitKind caster)
   {
      var spells = ClassBuilder.ClassSpells[caster].Select(s => s.Kind);
      return new ClassStats(spells);
   }
}