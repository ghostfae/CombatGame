using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatEngine;
public class UnitState
{
    public Unit Unit { get; }
    public int Health { get; private set; }

    public IEnumerable<TimedSpell> ReadyActions
    {
        get
        {
            foreach (var action in TimedSpells)
                if (action.CooldownTimer == 0)
                    yield return action;
        }
    }

    public List<TimedSpell> TimedSpells { get; }

    public static UnitState Create(Unit unit) 
    {
        return new UnitState(unit);
    }

    private UnitState(Unit unit)
    {
        Unit = unit;
        Health = unit.Health;
        TimedSpells = unit.AllSpells.Select(action => new TimedSpell(action)).ToList();
    }

    // only used by the builder
    internal UnitState(Unit unit, int health, List<TimedSpell> timedSpells)
    {
        Unit = unit;
        Health = health;
        TimedSpells = timedSpells;
    }

}
