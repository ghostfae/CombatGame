using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
    static void Main(string[] args)
    {
        var warrior = ClassBuilder.CreateWarrior();
        var mage = ClassBuilder.CreateMage();

        var combat = new Combat(warrior, mage);
        var initiative = combat.Initiative().ToString();

        combat.PrintInit();
    }
}

public class TurnLog 
{
}
public class Combat 
{
    private Unit[] Combatants;
    public Combat(params Unit[] combatants) 
    {
        Combatants = combatants;
    }

    public Unit[] Initiative()
    {
        return Combatants.OrderByDescending(combatant => combatant.Speed).ToArray();
    }

    public void PrintInit() 
    {
        foreach (var unit in Initiative()) 
        {
            Console.WriteLine($"{(unit.Kind)} has speed of {unit.Speed}");
        }
    }
    // for all combatants that are still alive:
    // sort by speed
    // faster one attacks
    // apply damage to target
    // set cooldown for attacker skill
    // update state
}
