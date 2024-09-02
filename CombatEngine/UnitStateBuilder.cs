namespace CombatEngine;

// builder for mutating an immutable unit state
// using fluent syntax

public class UnitStateBuilder 
{
    private readonly Unit _unit;
    private int _health;
    private List<TimedSpell> _timedSpells;

    public UnitStateBuilder(UnitState state)
    {
        _unit = state.Unit;
        _health = state.Health;
        _timedSpells = state.TimedSpells.Select(spell => spell.Clone()).ToList(); // deep clone
    }

    public UnitStateBuilder Hit(int damage)
    {
        _health = Math.Max(0, _health - damage);
        return this;
    }

    public UnitStateBuilder Tick(int tick = 1)
    {
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

    public UnitState Build() 
    {
        return new UnitState(_unit, _health, _timedSpells);
    }
}