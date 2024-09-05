namespace CombatEngine;

public class FightBuilder
{
    public static Unit AddWarrior(Side side) 
    {
        var warrior = ClassBuilder.CreateWarrior(side);

        return warrior;
    }

    public static Unit AddMage(Side side)
    {
        var mage = ClassBuilder.CreateMage(side);
        
        return mage;
    }

    public static Unit[] CreateScenario1v1() 
    {
        return new Unit[] { AddWarrior(Side.Red), AddMage(Side.Blue) };
    }
}