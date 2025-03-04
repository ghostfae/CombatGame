﻿namespace CombatEngine;

// todo: xml docs
public record ScoredAction(int TargetUid, Spell Spell, int Score)
{ 
}

public class ScoredActionComparer : IEqualityComparer<ScoredAction>
{
   public static ScoredActionComparer Instance = new();

   public bool Equals(ScoredAction? x, ScoredAction? y)
   {
      if (ReferenceEquals(x, y)) return true;
      if (x is null || y is null) return false;
      return x.TargetUid == y.TargetUid &&
             x.Spell.Kind == y.Spell.Kind;
   }

   public int GetHashCode(ScoredAction obj)
   {
      return obj.GetHashCode();
   }
}