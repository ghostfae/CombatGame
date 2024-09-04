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
    public Side AttackSide { get; set; }
    public bool CanAct { get; set; }

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
        CanAct = IsAlive();
    }

    // only used by the builder
    internal UnitState(Unit unit, int health, List<TimedSpell> timedSpells)
    {
        Unit = unit;
        Health = health;
        TimedSpells = timedSpells;
        CanAct = true;
    }

    public bool IsAlive()
    {
        if(Health <= 0) return false;
        return true;
    }

}

public static class UnitExtensions
{
    public static UnitStateBuilder ModifyState(this Unit unit)
    {
        return new UnitStateBuilder(unit.State);
    }
}
