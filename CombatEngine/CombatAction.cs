namespace CombatEngine;

public class CombatAction
{
    Actions ActionType { get; }
    int MaxCooldown { get; }
    //int? HitChance { get; }
    int[] MinMaxDamage { get; }

    public CombatAction(Actions actionType, int maxCooldown, int maxDamage = 0, int minDamage = 0) 
    { 
        ActionType = actionType;
        MaxCooldown = maxCooldown;
        MinMaxDamage = [minDamage, maxDamage];

    }
}

// TODO : Have action types be separate? as Buffs would have a duration, whilst Hits wouldn't
// TODO: have a target? or not here














