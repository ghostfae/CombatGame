﻿namespace CombatEngine;
/// <summary>
/// spell class that stores the current cooldown time on each spell
/// </summary>

public class TimedSpell
{
    public Spell Spell { get; }
    public int CooldownTimer { get; private set; }

    public TimedSpell(Spell spell)
       : this(spell, 0)
    // when we start, nothing is on cooldown
    {
    }

    private TimedSpell(Spell spell, int cooldownTimer)
    {
        Spell = spell;
        CooldownTimer = cooldownTimer;
    }

    public TimedSpell Clone() 
    {
        return new TimedSpell(Spell, CooldownTimer);
    }

    public void Tick(int ticks = 1)
    {
        CooldownTimer = Math.Max(CooldownTimer - ticks, 0); // used instead of if; always subtracts
    }

    public void MarkCooldown() 
    {
        CooldownTimer = Spell.MaxCooldown;
    }
}
