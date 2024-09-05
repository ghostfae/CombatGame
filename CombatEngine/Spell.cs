namespace CombatEngine;

public class Spell
{
    public SpellKind Kind { get; }
    public int MaxCooldown { get; }
    public int MinDamage { get; }
    public int MaxDamage { get; }

    public Spell(SpellKind spellKind, int maxCooldown, int minDamage = 0, int maxDamage = 0) 
    {
        Kind = spellKind;
        MaxCooldown = maxCooldown;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
    }

    public override string ToString()
    {
       return
          $"{nameof(Kind)}: {Kind}, {nameof(MaxCooldown)}: {MaxCooldown}, {nameof(MinDamage)}: {MinDamage}, {nameof(MaxDamage)}: {MaxDamage}";
    }
}

// TODO : Have action types be separate? as Buffs would have a duration, whilst Hits wouldn't













