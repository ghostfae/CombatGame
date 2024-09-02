namespace CombatEngine;

public class Spell
{
    public SpellKind Kind { get; }
    public int MaxCooldown { get; }
    //int? HitChance { get; }
    public int[] MinMaxDamage { get; }

    public Spell(SpellKind spellKind, int maxCooldown, int maxDamage = 0, int minDamage = 0) 
    {
        Kind = spellKind;
        MaxCooldown = maxCooldown;
        MinMaxDamage = [minDamage, maxDamage];
    }


}

// TODO : Have action types be separate? as Buffs would have a duration, whilst Hits wouldn't
// TODO: have a target? or not here














