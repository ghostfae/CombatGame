using System.Collections.Immutable;

namespace CombatEngine;
/// <summary>
/// state that updates whenever a unit casts or reacts to a spell;
/// a mutable version of the unit
/// </summary>
public class UnitState
{
   public Unit Unit { get; }
   public int Health { get; private set; }
   public Side Side { get; set; }
   public bool CanAct { get; set; }
   public int CanActTimer { get; set; }
   public List<TimedOverTimeEffect> OverTimeEffects { get; set; }
   public List<TimedSpell> TimedSpells { get; set; }
   public bool ResetRound() => CanAct = true;
   public static UnitState InitialCreate(Unit unit, Side side, int health)
   {
      return new UnitState(unit, side, health);
   }

   private UnitState(Unit unit, Side side, int health)
   {
      Unit = unit;
      Side = side;
      Health = health;
      TimedSpells = unit.AllSpells.Select(action => new TimedSpell(action)).ToList();
      CanAct = IsAlive();
      CanActTimer = 0;
      OverTimeEffects = [];
   }

   // only used by the builder
   internal UnitState(Unit unit, int health, List<TimedSpell> timedSpells,
      Side side, List<TimedOverTimeEffect> overTimeEffects, bool canAct, int canActTimer)
   {
      Unit = unit;
      Health = health;
      TimedSpells = timedSpells;
      Side = side;
      OverTimeEffects = overTimeEffects;
      CanAct = canAct;
      CanActTimer = canActTimer;
   }

   public bool IsAlive()
   {
      return Health > 0;
   }

   /// <summary>
   /// Updates cooldowns of the timed spells and ticks over time effects.
   /// </summary>
   public UnitState UpdateTick(int tick = 1)
   {
      return new UnitState(
         Unit, 
         Health,
         TimedSpells.Select(ote => ote.Tick()).ToList(),
         Side,
         OverTimeEffects.Select(ote => ote.Tick()).ToList(), 
         false, 
         CanActTimer);
   }

   public UnitState Upkeep()
   {
      var updatedState = UpkeepOverTime();
      var (canActTimer, canAct) = UpkeepCanAct();

      updatedState.CanActTimer = canActTimer;
      updatedState.CanAct = canAct;

      return updatedState;
   }

   public UnitState Hit(int effect)
   {
      var health = Math.Max(0, Health - effect);
      return new UnitState(Unit, health, TimedSpells,
         Side, OverTimeEffects, CanAct, CanActTimer);
   }

   public UnitState Heal(int effect)
   {
      var health = Math.Min(Unit.InitialHealth, Health + effect);
      return new UnitState(Unit, health, TimedSpells,
         Side, OverTimeEffects, CanAct, CanActTimer);
   }

   public UnitState MarkCooldown(SpellKind spellKind)
   {
      var clonedTimedSpells = new List<TimedSpell>(TimedSpells);
      // spellKind is unique within timedSpells
      var matchingSpell = TimedSpells.First(spell => spell.Spell.Kind == spellKind);

      clonedTimedSpells.Remove(matchingSpell);

      clonedTimedSpells.Add(matchingSpell.MarkCooldown());

      return new UnitState(Unit, Health, clonedTimedSpells,
         Side, OverTimeEffects, CanAct, CanActTimer);
   }

   public UnitState AttachOverTime(SpellEffect spellEffect)
   {
      var clone = new List<TimedOverTimeEffect>(OverTimeEffects)
         { new(spellEffect) };

      return new UnitState(Unit, Health, TimedSpells,
         Side, clone, CanAct, CanActTimer);
   }

   public UnitState UpkeepOverTime()
   {
      if (!OverTimeEffects.Any())
         return this;

      var elementsToRemove = new List<TimedOverTimeEffect>();

      var updatedState = new UnitState(Unit, Health, TimedSpells,
         Side, OverTimeEffects, CanAct, CanActTimer);

      foreach (var item in OverTimeEffects)
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
               updatedState = Hit(amount);
            }
            else
            {
               updatedState = Heal(amount);
            }
         }
      }

      foreach (var item in elementsToRemove)
      {
         updatedState.OverTimeEffects.Remove(item);
      }

      return updatedState;
   }

   public (int, bool) UpkeepCanAct()
   {
      var canActTimer = CanActTimer;
      bool canAct;

      if (canActTimer > 0)
      {
         canActTimer--;
         canAct = false;
      }
      else
      {
         canAct = true;
      }

      return (canActTimer, canAct);
   }

   public UnitState Freeze(int duration)
   {
      CanAct = false;
      CanActTimer = duration;
      return new UnitState(Unit, Health, TimedSpells,
         Side, OverTimeEffects, CanAct, CanActTimer);
   }

   public override string ToString()
   {
      return
         $"{nameof(Unit.Name)}: {Unit.Name}, {nameof(Health)}: {Health}," +
         $" {nameof(Side)}: {Side}, {nameof(CanAct)}: {CanAct}, {nameof(CanActTimer)}: {CanActTimer}";
   }

}
