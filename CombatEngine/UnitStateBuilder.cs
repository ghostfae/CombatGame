using static CombatEngine.UnitState;

namespace CombatEngine;

public class UnitStateBuilder
{
   private readonly List<TimedSpell> _timedSpells;
   private readonly List<TimedOverTimeEffect> _overTimeEffects; // TODO make class
   private readonly Unit _unit;
   private int _health;
   private Side _side;

   public UnitStateBuilder(UnitState state)
   {
      _unit = state.Unit;
      _health = state.Health;
      _timedSpells = state.TimedSpells.Select(spell => spell.Clone()).ToList(); // deep clone
      _side = state.Side;
      _overTimeEffects = state.OverTimeEffects.Select(effect => effect.Clone()).ToList(); // TODO deep clone
   }

   public UnitStateBuilder Hit(int effect)
   {
      _health = Math.Max(0, _health - effect);
      return this;
   }

   public UnitStateBuilder Heal(int effect)
   {
      _health = Math.Min(_unit.InitialHealth, _health + effect);
      return this;
   }

   public UnitStateBuilder Tick(int tick = 1)
   {
      foreach (var effect in _overTimeEffects)
      {
         effect.Tick(tick);
      }
      foreach (TimedSpell spell in _timedSpells)
      {
         spell.Tick(tick);
      }

      return this;
   }

   public UnitStateBuilder MarkCooldown(SpellKind spellKind)
   {
      // spellKind is unique within timedSpells
      var matchingSpell = _timedSpells.First(spell => spell.Spell.Kind == spellKind);
      matchingSpell.MarkCooldown();
      return this;
   }

   public UnitStateBuilder AttachOverTime(SpellEffect spellEffect)
   {
      _overTimeEffects.Add(new TimedOverTimeEffect(spellEffect));
      return this;
   }

   public UnitStateBuilder UpkeepOverTime()
   {
      var elementsToRemove = new List<TimedOverTimeEffect>();
      foreach (var item in _overTimeEffects)
      {
         if (item.Timer <= 0)
         {
            elementsToRemove.Add(item);
         }
         else
         {
            var amount = item.Effect.RollRandomAmount();
            if (item.Effect.IsHarm)
            {
               Hit(amount);
               Console.WriteLine($"{_unit} takes damage for {amount}.");
            }
            else
            {
               Heal(amount);
               Console.WriteLine($"{_unit} heals for {amount}.");
            }
         }
      }

      foreach (var item in elementsToRemove)
      {
         _overTimeEffects.Remove(item);
      }

      return this;
   }

   public UnitStateBuilder SetSide(Side side)
   {
      _side = side;
      return this;
   }

   public UnitState Build()
   {
      return new UnitState(_unit, _health, _timedSpells, _side, _overTimeEffects);
   }


}