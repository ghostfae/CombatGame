using CombatEngine;
using System.Diagnostics;

namespace CombatConsole;

public class CombatLog
{

}

/// <summary>
/// An instance is created of each round.
/// Units that made their move are removed.
/// </summary>
public class CombatOrder
{
    public CombatOrder()
    {
        
    }
}

/// <summary>
/// An instance of combat is created at the start of the battle.
/// It is then prodded until one side is dead.
/// </summary>
public class Combat
{
    private Random _random;
    private CombatLog _log;
    private Unit[] _combatants;

    public Combat(Random random, CombatLog log, params Unit[] combatants)
    {
        _random = random;
        _log = log;
        _combatants = combatants;
    }

    public List<Unit> UnitsByInitiative()
    {
        return _combatants
            .OrderByDescending(combatant => combatant.Speed)
            .ToList();
    }

    public Unit RotateCombatants() 
    {        
        var currentCombatant = _combatants[0];
        NewTurn.Attacker = currentCombatant;
        _combatants = MoveInitiative();
        return currentCombatant;
    }

    public Unit GetLastCombatant() 
    {
        return _combatants[_combatants.Length - 1];
    }

    private UnitState CastSpell(Unit caster, Spell spell, Unit target) 
    {
        // EXTENSION - have a % cast failed?
        ApplySpell(spell, target);
        NewTurn.Target = target;
        return caster.ModifyState().MarkCooldown(spell.Kind).Build();        
    }

    private UnitState ApplySpell(Spell spell, Unit target) 
    {
        var damage = _random.Next(spell.MinDamage, spell.MaxDamage);
        return target.ModifyState().Hit(damage).Build();
    }

    public Turn TakeTurn(Unit turnTaker) 
    {
        CastSpell(turnTaker, turnTaker.Behaviour.SelectSpell(), turnTaker.Behaviour.SelectEnemyTarget(_combatants));
        return NewTurn;
    
    }
}
