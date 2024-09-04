namespace CombatEngine;

public class Turn
{
    public Unit? Attacker { get; set; }
    public Unit? Target { get; set; } 
    public int Damage { get; set; }
    public Turn(Unit? attacker = null, Unit? target = null, int damage = 0)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
    }

}