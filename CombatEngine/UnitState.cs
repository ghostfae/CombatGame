using System.Collections.Immutable;

namespace CombatEngine;
/// <summary>
/// state that updates whenever a unit casts or reacts to a spell;
/// a mutable version of the unit
/// </summary>
public class UnitState
{
   public Unit Unit { get; }
   public Side Side { get; set; } // unused now, but will allow "possession" spells etc
   public int Health { get; set; }
   public bool CanAct { get; set; }
   public int CanActTimer { get; set; }
   public List<TimedOverTimeEffect> OverTimeEffects { get; set; }
   public List<TimedSpell> TimedSpells { get; set; }
   public bool ResetRound() => CanAct = true;
   public static UnitState InitialCreate(Unit unit, Side side, int health)
   {
      return new UnitState(unit, side, health);
   }

   private UnitState(Unit unit, Side side, int health,
      List<TimedSpell> timedSpells,
      List<TimedOverTimeEffect> overTimeEffects, 
      bool canAct, 
      int canActTimer)
   {
      Unit = unit;
      Side = side;
      Health = health;
      TimedSpells = timedSpells;
      OverTimeEffects = overTimeEffects;
      CanAct = canAct;
      CanActTimer = canActTimer;
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

   private UnitState CloneWithHealth(int health)
   {
      return new UnitState(Unit, Side, health, TimedSpells, OverTimeEffects, CanAct, CanActTimer);
   }

   public bool IsAlive()
   {
      return Health > 0;
   }

   /// <summary>
   /// Updates cooldowns of the timed spells and ticks over time effects.
   /// </summary>
   public UnitState Tick(int tick = 1)
   {
      return new UnitState(
         Unit,
         Side,
         Health,
         TimedSpells.Select(ote => ote.Tick()).ToList(),
         OverTimeEffects.Select(ote => ote.Tick()).ToList(), 
         CanAct, 
         CanActTimer);
   }

   /// <summary>
   /// Mark unit as made its turn
   /// </summary>
   /// <returns></returns>
   public UnitState ExhaustTurn()
   {
      return new UnitState(
         Unit,
         Side,
         Health,
         TimedSpells,
         OverTimeEffects,
         false,
         CanActTimer);
   }

   public UnitState Upkeep()
   {
      return UpkeepOverTime().UpkeepCanAct();
   }

   public UnitState Hit(int effect)
   {
      var health = Math.Max(0, Health - effect);
      return CloneWithHealth(health);
   }

   public UnitState Heal(int effect)
   {
      var health = Math.Min(Unit.InitialHealth, Health + effect);
      return CloneWithHealth(health);
   }

   public UnitState MarkCooldown(SpellKind spellKind)
   {
      var clonedTimedSpells = new List<TimedSpell>(TimedSpells);
      // spellKind is unique within timedSpells
      var matchingSpell = TimedSpells.First(spell => spell.Spell.Kind == spellKind);

      clonedTimedSpells.Remove(matchingSpell);

      clonedTimedSpells.Add(matchingSpell.MarkCooldown());

      return new UnitState(Unit, Side, Health, clonedTimedSpells,
         OverTimeEffects, CanAct, CanActTimer);
   }

   public UnitState AttachOverTime(SpellEffect spellEffect)
   {
      return new UnitState(Unit, Side, Health, TimedSpells,
         new List<TimedOverTimeEffect>(OverTimeEffects)
            { new(spellEffect) }, 
         CanAct, CanActTimer);
   }

   /// <summary>
   /// Applies over time effect and removes expired effect
   /// </summary>
   public UnitState UpkeepOverTime()
   {
      if (!OverTimeEffects.Any())
         return this;

      var elementsToRemove = new List<TimedOverTimeEffect>();

      var updatedState = new UnitState(Unit, Side, Health, TimedSpells,
         OverTimeEffects, CanAct, CanActTimer);

      foreach (var item in OverTimeEffects)
      {
         if (item.Timer <= 0)
         {
            elementsToRemove.Add(item);
         }
         else
         {
            // TODO: log?
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

      var overTimeEffects
         = updatedState.OverTimeEffects.Except(elementsToRemove);

      return updatedState;
   }

   /// <summary>
   /// 
   /// </summary>
   public UnitState UpkeepCanAct()
   {
      return new UnitState(Unit, Side, Health, TimedSpells, OverTimeEffects, 
         CanActTimer == 0,
         Math.Max(0, CanActTimer - 1));
   }

   public UnitState Freeze(int duration)
   {
      CanAct = false;
      CanActTimer = duration;
      return new UnitState(Unit, Side, Health, TimedSpells, OverTimeEffects, CanAct, CanActTimer);
   }

   public override string ToString()
   {
      return
         $"{nameof(Unit.Name)}: {Unit.Name}, {nameof(Side)}: {Side}, {nameof(Health)}: {Health}," +
         $"  {nameof(CanAct)}: {CanAct}, {nameof(CanActTimer)}: {CanActTimer}";
   }

}
