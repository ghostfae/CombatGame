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

    public static Unit[] CreateScenario1V1() 
    {
        return new Unit[] { AddWarrior(Side.Red), AddMage(Side.Blue) };
    }
    public static Unit[] CreateScenario2V2()
    {
       return new Unit[] { AddWarrior(Side.Red), AddWarrior(Side.Red), AddMage(Side.Blue), AddMage(Side.Blue) };
    }
}