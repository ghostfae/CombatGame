namespace CombatEngine;



public class Unit
{
    public UnitKind Kind { get; }
    public int Health { get; }
    public int Speed { get; }
    public List<Spell> AllSpells { get; }
    public UnitState State { get; }

    private Unit(UnitKind kind, int health, int speed, List<Spell> allSpells) 
    {
        Kind = kind;
        Health = health;
        Speed = speed;
        AllSpells = allSpells;

        State = UnitState.Create(this);
    }

    public Unit(UnitKind kind, int health, int speed, params Spell[] allSpells) 
        : this(kind, health, speed, allSpells.ToList())
    {
    }
}

