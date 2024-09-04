namespace CombatEngine;

/// <summary>
/// AI that decided what actions to take
/// </summary>
public class UnitBehaviour
{
    public Unit Self { get; }
    //public Unit? Target { get; set; }
    public UnitBehaviour(Unit self)
    {
        Self = self;
        //Target = null;
    }

    private IReadOnlyCollection<TimedSpell> GetReadySpells()
    {
        return Self
            .TimedSpells
            .Where(spell => spell.CooldownTimer == 0)
            .ToList();
    }

    public Spell SelectSpell() 
    {
        return GetReadySpells()
            .OrderByDescending(spell => spell.Spell.MaxDamage)
            .First()
            .Spell;
        //EXTENSION: add healing spells?
    }

    public Unit SelectEnemyTarget(Unit[] availableTargets) 
    {
        return availableTargets
            .Where(t => t.AttackSide != Self.AttackSide)
            .OrderBy(t => t.CurrentHealth)
            .First();
    }
}

