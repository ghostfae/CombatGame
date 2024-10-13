using System.Drawing;

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
   public bool MarkAsTakenTurn() => CanAct = false;
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
      OverTimeEffects = new List<TimedOverTimeEffect>();
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
   public void UpdateTick()
   {
      ModifyState(builder => builder.Tick());
      MarkAsTakenTurn();
   }
   public void Upkeep()
   {
      ModifyState(builder => builder
         .UpkeepOverTime()
         .UpkeepCanAct());
   }
   public override string ToString()
   {
      return
         $"{nameof(Unit.Name)}: {Unit.Name}, {nameof(Health)}: {Health}," +
         $" {nameof(Side)}: {Side}, {nameof(CanAct)}: {CanAct}, {nameof(CanActTimer)}: {CanActTimer}";
   }

   public UnitState ModifyState(Action<UnitStateBuilder> modifyStateAction)
   {
      var builder = new UnitStateBuilder(this);
      modifyStateAction(builder);
      Unit.UpdateState(builder.Build());
      return Unit.State;
   }

   public void ModifySelf(UnitState newState)
   {
      Health = newState.Health;
      TimedSpells = newState.TimedSpells;
      Side = newState.Side;
      OverTimeEffects = newState.OverTimeEffects;
      CanAct = newState.CanAct;
      CanActTimer = newState.CanActTimer;
   }
}
