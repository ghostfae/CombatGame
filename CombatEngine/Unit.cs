namespace CombatEngine;

public class Unit
{
    public UnitKind Kind { get; }
    public int Health { get; }
    public int Speed { get; }
    public IReadOnlyCollection<Spell> AllSpells { get; }
    public UnitState State { get; private set; }
    public UnitBehaviour Behaviour { get; }
    public Side AttackSide => State.AttackSide;
    public int CurrentHealth => State.Health;
    public IReadOnlyCollection<TimedSpell> TimedSpells => State.TimedSpells;

    private Unit(UnitKind kind, int health, int speed, List<Spell> allSpells) 
    {
        Kind = kind;
        Health = health;
        Speed = speed;
        AllSpells = allSpells;

        State = UnitState.Create(this);
        Behaviour = new UnitBehaviour(this);
    }

    public Unit(UnitKind kind, int health, int speed, params Spell[] allSpells) 
        : this(kind, health, speed, allSpells.ToList())
    {
    }

    public void UpdateState(UnitState state)
    {
        State = state;
    }
}