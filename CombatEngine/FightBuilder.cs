using CombatEngine;

namespace CombatConsole;

public class FightBuilder
{
    public static Unit AddWarrior(Side side) 
    {
        var warrior = ClassBuilder.CreateWarrior();
        warrior.State.AttackSide = side;

        return warrior;
    }

    public static Unit AddMage(Side side)
    {
        var mage = ClassBuilder.CreateMage();
        mage.State.AttackSide = side;

        return mage;
    }

    public static Unit[] Scenario1v1() 
    {
        return new Unit[] { AddWarrior(Side.sideA), AddMage(Side.sideB) };
    }
}