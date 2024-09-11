namespace CombatEngine;
/// <summary>
/// creates the effect that a given spell would have on the target
/// </summary>
public enum SpellEffectKind
{
   Direct,
   OverTime,
   Freeze
}
public class SpellEffect
{
   public SpellEffectKind Kind { get; }
   public bool IsHarm { get; }
   public int MinEffect { get; }
   public int MaxEffect { get; }

   /// <summary>
   /// Overtime effect duration, if applicable
   /// </summary>
   public int? Duration { get; }

   /// <summary>
   /// Crit chance in percents
   /// </summary>
   public int CritChance { get; }
   public int CritModifier { get; }

   private SpellEffect(SpellEffectKind kind, bool isHarm, int minEffect, int maxEffect,
      int? duration, int critChance, int critModifier)
   {
      Kind = kind;
      IsHarm = isHarm;
      MinEffect = minEffect;
      MaxEffect = maxEffect;
      Duration = duration;
      CritChance = critChance;
      CritModifier = critModifier;
   }

   public static SpellEffect CreateDirectDamage(int minEffect, int maxEffect, int critChance, int critModifier)
   {
      return new SpellEffect
         (SpellEffectKind.Direct, true, minEffect, maxEffect, null, critChance, critModifier);
   }

   public static SpellEffect CreateDirectHeal(int minEffect, int maxEffect, int critChance, int critModifier)
   {
      return new SpellEffect
         (SpellEffectKind.Direct, false, minEffect, maxEffect, null, critChance, critModifier);
   }


   public static SpellEffect CreateOverTimeDamage(int minEffect, int maxEffect, int duration)
   {
      return new SpellEffect
         (SpellEffectKind.OverTime, true, minEffect, maxEffect, duration, 0, 0);
   }

   public static SpellEffect CreateOverTimeHeal(int minEffect, int maxEffect, int duration)
   {
      return new SpellEffect
         (SpellEffectKind.OverTime, false, minEffect, maxEffect, duration, 0, 0);
   }

   public static SpellEffect CreateSkipTurn()
   {
      return new SpellEffect
         (SpellEffectKind.Freeze, false, 0, 0, 2, 0, 0);
   }

   public int RollRandomAmount()
   {
      return Rng.Random.Next(MinEffect, MaxEffect);
   }

   public override string ToString()
   {
      return
         $"{nameof(Kind)}: {Kind}, {nameof(MinEffect)}: {MinEffect}, {nameof(MaxEffect)}: {MaxEffect}, " +
         $"{nameof(Duration)}: {Duration}, {nameof(CritChance)}: {CritChance}";
   }
}