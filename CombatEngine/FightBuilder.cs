using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace CombatEngine;

/// <summary>
/// adds units to combat, and creates battle scenarios
/// </summary>
public class FightBuilder
{
   public static UnitState AddWarrior(Side side, ClassBuilder builder)
   {
      var warrior = builder.CreateWarrior(side);

      return warrior.State;
   }

   public static UnitState AddMage(Side side, ClassBuilder builder)
   {
      var mage = builder.CreateMage(side);
      return (mage.State);
   }
   public static UnitState[] CreateScenario1V1(ClassBuilder builder) 
   {
       return [AddWarrior(Side.Red, builder), AddMage(Side.Blue, builder)];
   }
   public static UnitState[] CreateScenario1V1Warrior(ClassBuilder builder)
   {
      return [AddWarrior(Side.Red, builder), AddWarrior(Side.Blue, builder)];
   }
   public static UnitState[] CreateScenario2V2(ClassBuilder builder)
   {
      return [AddWarrior(Side.Red, builder), AddWarrior(Side.Blue, builder), AddMage(Side.Red, builder), AddMage(Side.Blue, builder)];
   }
   //public static IEnumerable<UnitState> CreateScenario1V1(ClassBuilder builder)
   //{
   //   var warrior = AddWarrior(Side.Red, builder);
   //   var mage = AddMage(Side.Blue, builder);
   //   return new Dictionary<int, UnitState>()
   //   {
   //      [warrior.Uid] = warrior.State,
   //      [mage.Uid] = mage.State,
   //   };
   //}

   //public static Dictionary<int, UnitState> CreateScenario2V2(ClassBuilder builder)
   //{
   //   var warriorRed = AddWarrior(Side.Red, builder);
   //   var warriorBlue = AddWarrior(Side.Blue, builder);
   //   var mageRed = AddMage(Side.Red, builder);
   //   var mageBlue = AddMage(Side.Blue, builder);
   //   return new Dictionary<int, UnitState>()
   //   {
   //      [warriorRed.Uid] = warriorRed.State,
   //      [warriorBlue.Uid] = warriorBlue.State,
   //      [mageRed.Uid] = mageRed.State,
   //      [mageBlue.Uid] = mageBlue.State
   //   };
   //}
}