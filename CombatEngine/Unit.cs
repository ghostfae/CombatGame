﻿namespace CombatEngine;

/// <summary>
/// creates the combat unit and the initial state it has before the game starts
/// </summary>
public class Unit
{
   public int Uid { get; }
   public UnitKind Kind { get; }
   public int InitialHealth { get; }
   public int Speed { get; }
   public IReadOnlyCollection<Spell> AllSpells { get; }
   public UnitState State { get; private set; }

   public string Name { get; }

   private Unit(int uid, UnitKind kind, int initialHealth, int speed, Side side, string name, List<Spell> allSpells)
   {
      Uid = uid;
      Kind = kind;
      InitialHealth = initialHealth;
      Speed = speed;
      AllSpells = allSpells;
      Name = name;
      State = UnitState.InitialCreate(this, side, initialHealth);
   }

   public Unit(int uid, UnitKind kind, int initialHealth, int speed, Side side, string name, params Spell[] allSpells)
      : this( uid, kind, initialHealth, speed, side, name, allSpells.ToList())
   {
   }

   public override string ToString()
   {
      return $"{Kind} {Name}";
   }
}