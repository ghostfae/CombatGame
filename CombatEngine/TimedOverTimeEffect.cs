namespace CombatEngine;

/// <summary>
/// over time effect that tracks the duration left
/// </summary>
public class TimedOverTimeEffect
{
   public SpellEffect Effect { get; }
   public int Timer { get; private set; }

   public TimedOverTimeEffect(SpellEffect effect)
      : this(effect, effect.Duration!.Value)
   {
   }

   private TimedOverTimeEffect(SpellEffect effect, int timer)
   {
      Effect = effect;
      Timer = timer;
   }

   public TimedOverTimeEffect Clone()
   {
      return new TimedOverTimeEffect(Effect, Timer);
   }

   public TimedOverTimeEffect Tick(int ticks = 1)
   {
      return new TimedOverTimeEffect(Effect, Math.Max(Timer - ticks, 0)); // used instead of if; always subtracts
   }
}