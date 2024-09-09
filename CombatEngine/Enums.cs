﻿namespace CombatEngine;
public enum UnitKind
{
    Warrior,
    Mage
}

public enum SpellKind
{
   // warrior spells
   SwordHit,
   ShieldBash,

   // mage spells
   FrostBolt,
   FireSnap,

   // universal spells
   HealthPotion,
   HealthDurationPotion
}

public enum Side
{ 
    Red,
    Blue
}