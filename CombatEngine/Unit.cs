namespace CombatEngine;

public class RoundState
{
   public bool CanAct { get; set; } = true;
}

public class Unit
{
   public UnitKind Kind { get; }
   public int InitialHealth { get; }
   public int Speed { get; }
   public IReadOnlyCollection<Spell> AllSpells { get; }
   public UnitState State { get; private set; }

   public RoundState RoundState { get; private set; }

   public Side Side => State.Side;
   public int CurrentHealth => State.Health;
   public bool IsAlive() => State.IsAlive();

   public bool CanAct() => RoundState.CanAct;

   public bool ResetRound() => RoundState.CanAct = true;

   public bool MarkAsTakenTurn() => RoundState.CanAct = false;

   public string Name { get; }
   public IReadOnlyCollection<TimedSpell> TimedSpells => State.TimedSpells;

   private Unit(UnitKind kind, int initialHealth, int speed, Side side, string name, List<Spell> allSpells)
   {
      Kind = kind;
      InitialHealth = initialHealth;
      Speed = speed;
      AllSpells = allSpells;
      Name = name;
      State = UnitState.Create(this, side, initialHealth);
      RoundState = new RoundState();
   }

   public Unit(UnitKind kind, int initialHealth, int speed, Side side, string name, params Spell[] allSpells)
      : this(kind, initialHealth, speed, side, name, allSpells.ToList())
   {
   }

   public void UpdateState(UnitState state)
   {
      State = state;
   }

   public void UpdateTick()
   {
      this.ModifyState(builder => builder.Tick());
   }

   public (Unit target, Spell spell) ChooseTargetAndSpell(IEnumerable<Unit> availableTargets)
   {
      var selectedSpell = UnitBehaviour.SelectSpell(this);
      var selectedTarget = UnitBehaviour.SelectEnemy(availableTargets, this);
      return (selectedTarget, selectedSpell);
   }

   public override string ToString()
   {
      return $"{Kind} {Name}";
   }
}