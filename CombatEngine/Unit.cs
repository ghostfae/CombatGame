namespace CombatEngine;

public class TimedAction
{
    public CombatAction Action { get; }
    public int CooldownTimer { get; private set; }
    public TimedAction(CombatAction action)
    {
        Action = action;
        CooldownTimer = 0; // when we start, nothing is on cooldown
    }

    public void Tick(int ticks = 1) 
    {
        CooldownTimer = Math.Max(CooldownTimer - ticks, 0); // used instead of if; always subtracts
    }
}
public class UnitState
{
    public Unit Unit { get; }
    public int Health { get; private set; }

    public IEnumerable<TimedAction> ReadyActions
    {
        get
        {
            foreach (var action in _timedActions)
                if (action.CooldownTimer == 0)
                    yield return action;
        }
    }

    private List<TimedAction> _timedActions;
}

public class Unit
{
    public UnitKind Kind { get; }
    public int Health { get; }
    public int Speed { get; }
    public List<CombatAction> AllActions { get; }

    public Unit(UnitKind kind, int health, int speed, List<CombatAction> allActions) 
    {
        Kind = kind;
        Health = health;
        Speed = speed;
        AllActions = allActions;
    }
}

