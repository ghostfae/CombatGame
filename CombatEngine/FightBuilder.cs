namespace CombatEngine;
/// <summary>
/// adds units to combat, and creates battle scenarios
/// </summary>
public class FightBuilder
{
    public static UnitState AddWarrior(Side side) 
    {
        var warrior = ClassBuilder.CreateWarrior(side);

        return warrior.State;
    }

    public static UnitState AddMage(Side side)
    {
        var mage = ClassBuilder.CreateMage(side);
        
        return mage.State;
    }

    public static UnitState[] CreateScenario1V1() 
    {
        return [AddWarrior(Side.Red), AddMage(Side.Blue)];
    }
    public static UnitState[] CreateScenario2V2()
    {
       return [AddWarrior(Side.Red), AddWarrior(Side.Blue), AddMage(Side.Red), AddMage(Side.Blue)];
    }
}