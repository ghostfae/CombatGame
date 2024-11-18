namespace CombatEngine;

public interface ICombatListener
{
   void CastSpell(UnitKind caster, SpellKind spell);
   
   void Winners(IEnumerable<UnitState> winningUnits);

   void EndOfRound(int round);
}

public class ConsoleCombatListener() : ICombatListener
{
   public void CastSpell(UnitKind caster, SpellKind spell)
   {
   }

   public void Winners(IEnumerable<UnitState> winningUnits)
   {
   }

   public void EndOfRound(int round)
   {
      Console.ReadLine();
   }
}