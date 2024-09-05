using System.Data;

namespace CombatEngine;

public class Unit
{
   public UnitKind Kind { get; }
   public int InitialHealth { get; }
   public int Speed { get; }
   public IReadOnlyCollection<Spell> AllSpells { get; }
   public UnitState State { get; private set; }

   public Side Side => State.Side;
   public int CurrentHealth => State.Health;
   public bool IsAlive() => State.IsAlive();

   public IReadOnlyCollection<TimedSpell> TimedSpells => State.TimedSpells;

   private Unit(UnitKind kind, int initialHealth, int speed, Side side, List<Spell> allSpells)
   {
      Kind = kind;
      InitialHealth = initialHealth;
      Speed = speed;
      AllSpells = allSpells;

      State = UnitState.Create(this, side, initialHealth);
   }

   public Unit(UnitKind kind, int initialHealth, int speed, Side side, params Spell[] allSpells)
      : this(kind, initialHealth, speed, side, allSpells.ToList())
   {
   }

   public void UpdateState(UnitState state)
   {
      State = state;
   }

   public (Unit target, Spell spell) ChooseTargetAndSpell(IReadOnlyCollection<Unit> availableTargets)
   {
      var selectedSpell = UnitBehaviour.SelectSpell(this);
      var selectedTarget = UnitBehaviour.SelectEnemy(availableTargets, this);
      return (selectedTarget, selectedSpell);
   }

   public override string ToString()
   {
      return $"{nameof(Kind)}: {Kind}";
   }
}