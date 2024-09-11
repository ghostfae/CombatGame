namespace CombatEngine;
/// <summary>
/// adds units to combat, and creates battle scenarios
/// </summary>
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
        return [AddWarrior(Side.Red), AddMage(Side.Blue)];
    }
    public static Unit[] CreateScenario2V2()
    {
       return [AddWarrior(Side.Red), AddWarrior(Side.Blue), AddMage(Side.Red), AddMage(Side.Blue)];
    }
}