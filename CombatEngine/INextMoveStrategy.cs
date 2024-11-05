namespace CombatEngine;

public interface INextMoveStrategy
{
   (UnitState target, Spell spell) ChooseNextMove(UnitState caster, CombatState combatState);
}

public class RandomMoveStrategy : INextMoveStrategy
{
   // TODO: consider all spells on CD so there's no next move at all (skip turn)
   public (UnitState target, Spell spell) ChooseNextMove(UnitState caster, CombatState combatState)
   {
      var selectedSpell = UnitBehaviour.SelectRandomSpell(caster);
      var allTargets = combatState.GetAliveUnits();

      var selectedTarget = selectedSpell.SpellEffect.IsHarm ? // if operator
         UnitBehaviour.SelectRandomEnemy(allTargets, caster)
         : UnitBehaviour.SelectRandomAlly(allTargets, caster);

      return (selectedTarget, selectedSpell);
   }
}