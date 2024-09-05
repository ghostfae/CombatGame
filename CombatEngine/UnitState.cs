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
    public Side Side { get; set; }
    public bool CanAct { get; set; }

    public List<TimedSpell> TimedSpells { get; }

    public static UnitState Create(Unit unit, Side side, int health) 
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
    }

    // only used by the builder
    internal UnitState(Unit unit, int health, List<TimedSpell> timedSpells, Side side)
    {
        Unit = unit;
        Health = health;
        TimedSpells = timedSpells;
        Side = side;
        CanAct = true;
    }

    public bool IsAlive()
    {
       return Health > 0;
    }
}

public static class UnitExtensions
{
   public static void ModifyState(this Unit unit, Action<UnitStateBuilder> modifyStateAction)
   {
      var builder = new UnitStateBuilder(unit.State);
      modifyStateAction(builder);
      unit.UpdateState(builder.Build());
   }
}
