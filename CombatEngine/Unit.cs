namespace CombatEngine;
/// <summary>
/// creates the combat unit and the initial state it has before the game starts
/// </summary>

public class Unit
{
   public UnitKind Kind { get; }
   public int InitialHealth { get; }
   public int Speed { get; }
   public IReadOnlyCollection<Spell> AllSpells { get; }
   public UnitState State { get; private set; }

   public string Name { get; }

   private Unit(UnitKind kind, int initialHealth, int speed, Side side, string name, List<Spell> allSpells)
   {
      Kind = kind;
      InitialHealth = initialHealth;
      Speed = speed;
      AllSpells = allSpells;
      Name = name;
      State = UnitState.InitialCreate(this, side, initialHealth);
   }

   public Unit(UnitKind kind, int initialHealth, int speed, Side side, string name, params Spell[] allSpells)
      : this(kind, initialHealth, speed, side, name, allSpells.ToList())
   {
   }

   public void UpdateState(UnitState state)
   {
      State = state;
   }

   public (UnitState target, Spell spell) ChooseTargetAndSpell(IEnumerable<UnitState> availableTargets)
   {
      var selectedSpell = UnitBehaviour.SelectSpell(State);

      var selectedTarget = selectedSpell.SpellEffect.IsHarm ? // if operator
         UnitBehaviour.SelectEnemy(availableTargets, State) 
         : UnitBehaviour.SelectAlly(availableTargets, State);

      return (selectedTarget, selectedSpell);
   }
   
   public override string ToString()
   {
      return $"{Kind} {Name}";
   }
}