namespace CombatEngine;

public class Combat
{
   private readonly Random _random;
   private readonly CombatLog _log;
   private readonly List<Unit> _combatants;

   public Combat(Random random, params Unit[] combatants)
   {
      _random = random;
      _log = new CombatLog();
      _combatants = combatants.ToList();
   }

   public void Run()
   {
      var order = _combatants
         .OrderByDescending(combatant => combatant.Speed)
         .ToList();

      _log.Init(order);

      while (true)
      {
         foreach (var unit in order)
         {
            if (!PerformTurn(unit))
            {
               // someone dead, lets check if we should continue
               var winningSide = TryGetWinningSide(order);
               if (winningSide != null)
               {
                  _log.Win(winningSide.Value);
                  return;
               }
            }
         }
      }
   }

   private Side? TryGetWinningSide(IEnumerable<Unit> order)
   {
      var survivingSides = order
         .Where(u => u.CurrentHealth > 0)
         .GroupBy(u => u.Side)
         .Select(g => g.Key)
         .ToArray();

      if (survivingSides.Length == 1)
      {
         return survivingSides[0];
      }
      return null;
   }

   /// <returns>true if the combat should continue, false if one side wins</returns>
   private bool PerformTurn(Unit unit)
   {
      _log.StartTurn(unit);
      // 1. apply dots or hots if any
      // TODO:

      // 2. choose target(s) and spells
      var (target, spell) = unit.ChooseTargetAndSpell(_combatants);

      // 3. cast
      var damage = CastSpell(unit, spell);
      ApplySpell(target, spell, damage);

      var isTargetAlive = target.IsAlive();
      if (!isTargetAlive)
      {
         _log.UnitDies(target);
      }
      return isTargetAlive;
   }

   public int CastSpell(Unit caster, Spell spell)
   {
      // EXTENSION - have a % cast failed?
      var damage = _random.Next(spell.MinDamage, spell.MaxDamage);
      _log.CastSpell(caster, spell, damage);

      caster.ModifyState(builder => builder.MarkCooldown(spell.Kind));
      return damage;
   }

   public void ApplySpell(Unit target, Spell spell, int damage)
   {
      // TODO: apply defences etc
      _log.TakeDamage(target, damage);
      target.ModifyState(builder => builder.Hit(damage));
   }
}
