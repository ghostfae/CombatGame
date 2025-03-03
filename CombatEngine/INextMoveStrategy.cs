namespace CombatEngine;

public interface INextMoveStrategy
{
   (UnitState target, Spell spell)? ChooseNextMove(UnitState caster, CombatState combatState);

   int GetAverage(int[] scores);

   ScoredAction EvaluateChain(CombatState combatState, UnitState caster, int depth, Side initSide);

   CombatState ApplyTurn((UnitState target, Spell spell) action, CombatState combatState, UnitState caster, ICombatLog log);
}