using System;
using System.Collections.Generic;
using System.Linq;
using CombatEngine;

namespace BalanceConsole;

internal class BalanceConsole : ICombatListener
{
   Dictionary<UnitKind, ClassStats1v1> _stats = new();

   public void Run()
   {
      var combatants = FightBuilder.CreateScenario1V1();

      var combat = new CombatState(combatants);

      CombatRunner.Run(combat, new NullCombatLog(), this); // callback
   }

   // todo:
   // create an extension method for Dictionary<TKey, TValue> GetOrCreate()
   // 1. where should it live? how to call the class where it lives?
   // 2. what should you pass?
   // 3, when done, replace lines 29 -34 with this new method
   
   public void CastSpell(UnitKind caster, SpellKind spell)
   {
      if (!_stats.TryGetValue(caster, out var classStats))
      {
         var spells = ClassBuilder.ClassSpells[caster].Select(s => s.Kind);
         classStats = new ClassStats1v1(spells);
         _stats.Add(caster, classStats);
      }

      classStats.SpellCount[spell]++;
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
      var winner = winningUnits.First().Unit.Kind;
      _stats[winner].Wins++;
   }

   public void EndOfRound(int round)
   {
   }
}