using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatEngine;

public static class Rng
{
   public static readonly Random Random = new(1);
}

public static class NameGenerator
{
   private static readonly List<string> RandomNames;

   static NameGenerator()
   {
      RandomNames = new List<string>
      {
         "Anthony", "Bobert", "Caroline", "Daphne", "Elliott", "Francois", "George", "Herald", "Ingrid", "Jake", "Kat",
         "Leroy", "Manny", "Nia", "Opera", "Percy", "Q", "Richard", "Stewart", "Tamsin", "Ulfr", "Veronica", "Wade",
         "Xander", "Yuriy", "Zila"
      };
   }

   public static string GenerateName()
   {
      var random = Rng.Random.Next(RandomNames.Count);
      var name = RandomNames[random];
      RandomNames.Remove(name);

      return name;
   }
}

