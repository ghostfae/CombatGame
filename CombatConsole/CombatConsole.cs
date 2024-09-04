using CombatEngine;

namespace CombatConsole;

internal class CombatConsole 
{
    static void Main(string[] args)
    {
        var combatants = FightBuilder.Scenario1v1();

        var combat = new Combat(new Random(1), combatants[0], combatants[1]);

        // beginning of a round
        TurnLog.PrintInit(combat);

        SingleTurn(combat);
        SingleTurn(combat);
        SingleTurn(combat);

    }

    public static void SingleTurn (Combat combat)
    { 
        // beginning of first turn
        var currentCombatant = combat.RotateCombatants();
        TurnLog.StartTurnDialogue(currentCombatant);

        var currentTurn = combat.TakeTurn(currentCombatant);
        TurnLog.TakeDamageDialogue(currentTurn.Target, currentTurn.Damage);
    }
}

public static class TurnLog
{
    public static void PrintInit(Combat combat)
    {
        foreach (var unit in combat.Initiative())
        {
            Console.WriteLine($"{(unit.Kind)} has speed of {unit.Speed}.");
        }
        Console.WriteLine();
    }
    public static void StartTurnDialogue(Unit currentCombatant) 
    {
        Console.WriteLine($"It is {currentCombatant.Kind}'s turn.\n" +
            $"{currentCombatant.Kind} has {currentCombatant.Health} health.\n");
    }

    public static void TakeDamageDialogue(Unit enemyCombatant, int damage) 
    {
        Console.WriteLine($"{enemyCombatant.Kind} was hit for {damage} damage.\n");
    }
}
