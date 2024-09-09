namespace CombatEngine;

public class Spell
{
   public SpellKind Kind { get; }
   public int MaxCooldown { get; }
   public SpellEffect SpellEffect { get; }

   public bool IsOverTime => SpellEffect.Kind == SpellEffectKind.OverTime;

   public Spell(SpellKind spellKind, int maxCooldown, SpellEffect spellEffect)
   {
      Kind = spellKind;
      MaxCooldown = maxCooldown;
      SpellEffect = spellEffect;
   }

   public override string ToString()
   {
      return
         $"{nameof(Kind)}: {Kind}, {nameof(MaxCooldown)}: {MaxCooldown}";
   }
}

public static class SpellBook
{
   private static Spell CreateSpell(SpellKind spellKind, int maxCooldown, SpellEffect spellEffect)
   {
      return new Spell(spellKind, maxCooldown, spellEffect);
   }

   public static Spell CreateSwordHit()
   {
      return CreateSpell(SpellKind.SwordHit, 0, SpellEffect.CreateDirectDamage(10, 10, 5));
   }

   public static Spell CreateShieldBash()
   {
      return CreateSpell(SpellKind.ShieldBash, 3, SpellEffect.CreateDirectDamage(5, 30, 5));
   }

   public static Spell CreateFrostBolt()
   {
      return CreateSpell(SpellKind.FrostBolt, 0, SpellEffect.CreateDirectDamage(10, 10, 5));
   }
   public static Spell CreateFireSnap()
   {
      return CreateSpell(SpellKind.FireSnap, 3, SpellEffect.CreateOverTimeDamage(10, 15, 2));
   }

   public static Spell CreateHealthPotion()
   {
      return CreateSpell(SpellKind.HealthPotion, 2, SpellEffect.CreateDirectHeal(8, 10, 10));
   }
   public static Spell CreateHealthDurationPotion()
   {
      return CreateSpell(SpellKind.HealthDurationPotion, 3, SpellEffect.CreateOverTimeHeal(15, 20, 2));
   }
}











