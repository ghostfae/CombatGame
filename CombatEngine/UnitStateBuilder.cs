namespace CombatEngine;
/// <summary>
/// builds a new unit state whenever the current unit state needs to be modified
/// </summary>

public class UnitStateBuilder
{
   private readonly List<TimedSpell> _timedSpells;
   private readonly List<TimedOverTimeEffect> _overTimeEffects;
   private readonly Unit _unit;
   private int _health;
   private Side _side;
   private bool _canAct;
   private int _canActTimer;

   public UnitStateBuilder(UnitState state)
   {
      _unit = state.Unit;
      _health = state.Health;
      _timedSpells = state.TimedSpells.Select(spell => spell.Clone()).ToList(); // deep clone
      _side = state.Side;
      _overTimeEffects = state.OverTimeEffects.Select(effect => effect.Clone()).ToList(); // TODO deep clone
      _canAct = state.CanAct;
      _canActTimer = state.CanActTimer;
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
            }
            else
            {
               Heal(amount);
            }
         }
      }

      foreach (var item in elementsToRemove)
      {
         _overTimeEffects.Remove(item);
      }

      return this;
   }

   public UnitStateBuilder UpkeepCanAct()
   {
      if (_canActTimer > 0)
      {
         _canActTimer--;
         _canAct = false;
      }
      else
      {
         _canAct = true;
      }

      return this;
   }

   public UnitStateBuilder Freeze(int duration)
   {
      _canAct = false;
      _canActTimer = duration;
      return this;
   }

   public UnitStateBuilder SetSide(Side side)
   {
      _side = side;
      return this;
   }

   public UnitState Build()
   {
      var state = new UnitState(_unit, _health, _timedSpells,
         _side, _overTimeEffects, _canAct, _canActTimer); 
      //Console.WriteLine($"Built {state}");
      return state;
   }


}