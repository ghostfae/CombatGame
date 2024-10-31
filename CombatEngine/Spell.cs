namespace CombatEngine;
/// <summary>
/// creates spells that are used by the units
/// </summary>
public class Spell
{
   public SpellKind Kind { get; }
   public int MaxCooldown { get; }
   public SpellEffect SpellEffect { get; }
   public SpellEffect? CritEffect { get; }

   public bool IsOverTime => SpellEffect.Kind == SpellEffectKind.OverTime;

   public Spell(SpellKind spellKind, int maxCooldown, SpellEffect spellEffect, SpellEffect? critEffect = null)
   {
      Kind = spellKind;
      MaxCooldown = maxCooldown;
      SpellEffect = spellEffect;
      CritEffect = critEffect;
   }

   public override string ToString()
   {
      return
         $"{nameof(Kind)}: {Kind}, {nameof(MaxCooldown)}: {MaxCooldown}";
   }
}

public static class SpellBook
{
   private static Spell CreateSpell(SpellKind spellKind, int maxCooldown, SpellEffect spellEffect, SpellEffect? critEffect = null)
   {
      return new Spell(spellKind, maxCooldown, spellEffect, critEffect);
   }

   public static Spell CreateSwordHit()
   {
      return CreateSpell(SpellKind.SwordHit, 0, 
         SpellEffect.CreateDirectDamage(30, 30, 5, 4),
         SpellEffect.CreateOverTimeDamage(5, 10, 3));
   }

   public static Spell CreateShieldBash()
   {
      return CreateSpell(SpellKind.ShieldBash, 3, 
         SpellEffect.CreateDirectDamage(15, 30, 5, 2));
   }

   public static Spell CreateFrostBolt()
   {
      return CreateSpell(SpellKind.FrostBolt, 0,
         SpellEffect.CreateDirectDamage(30, 30, 5, 3),
         SpellEffect.CreateSkipTurn());
   }
   public static Spell CreateFireSnap()
   {
      return CreateSpell(SpellKind.FireSnap, 3, 
         SpellEffect.CreateOverTimeDamage(30, 45, 2));
   }

   public static Spell CreateHealthPotion()
   {
      return CreateSpell(SpellKind.HealthPotion, 2, 
         SpellEffect.CreateDirectHeal(8, 10, 10, 2));
   }
   public static Spell CreateHealthDurationPotion()
   {
      return CreateSpell(SpellKind.HealthDurationPotion, 3, 
         SpellEffect.CreateOverTimeHeal(15, 20, 2));
   }
}











