namespace CombatEngine;
/// <summary>
/// generates a random name from a list and avoids repeats;
/// stores the random used for this game
/// </summary>
public static class Rng
{
   public static Random Random = new(4);

   public static void ReplaceSeed(int seed)
   {
      Random = new Random(seed);
   }
}

public static class NameGenerator
{
   private static readonly List<string> AllRandomNames;
   private static readonly List<string> CurrentRandomNames;

   static NameGenerator()
   {
      AllRandomNames = new List<string>
      {
         "Anthony", "Bobert", "Caroline", "Daphne", "Elliott", "Francois", "George", "Herald", "Ingrid", "Jake", "Kat",
         "Leroy", "Manny", "Nia", "Opera", "Percy", "Q", "Richard", "Stewart", "Tamsin", "Ulfr", "Veronica", "Wade",
         "Xander", "Yuriy", "Zila"
      };

      CurrentRandomNames = new List<string>
      {
         "Anthony", "Bobert", "Caroline", "Daphne", "Elliott", "Francois", "George", "Herald", "Ingrid", "Jake", "Kat",
         "Leroy", "Manny", "Nia", "Opera", "Percy", "Q", "Richard", "Stewart", "Tamsin", "Ulfr", "Veronica", "Wade",
         "Xander", "Yuriy", "Zila"
      };
   }

   public static string GenerateName()
   {
      ResetRandomNames();
      var random = Rng.Random.Next(CurrentRandomNames.Count);
      var name = CurrentRandomNames[random];
      CurrentRandomNames.Remove(name);

      return name;
   }

   public static void ResetRandomNames()
   {
      if (CurrentRandomNames.Count <= 0)
      {
         CurrentRandomNames.Clear();
         CurrentRandomNames.AddRange(AllRandomNames);
      }
   }
}

