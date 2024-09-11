namespace CombatEngine;

public interface ICombatListener
{
   void EndOfRound(int round);
}

public class ConsoleCombatListener : ICombatListener
{
   public void EndOfRound(int round)
   {
      Console.ReadLine();
   }
}