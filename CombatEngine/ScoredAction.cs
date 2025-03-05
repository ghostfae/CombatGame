namespace CombatEngine;

/// <summary>
/// creates a version of the action that has a 'score' assigned to it
/// used for ai, to be able to compare the 'best' course of action based on scoring
/// </summary>

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