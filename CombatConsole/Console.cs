using CombatEngine;

namespace CombatConsole;

internal class Console
{
    static void Main(string[] args)
    {
        // create warrior
        var weaponStrike = new CombatAction(Actions.Hit, 0, 20, 10);
        var healthPotion = new CombatAction(Actions.Heal, 3, 10, 10);
        var warriorActions = new List<CombatAction>();

        warriorActions.Add(weaponStrike);
        warriorActions.Add(healthPotion);

        var warrior = new Unit(UnitKind.Warrior, 100, 6, warriorActions);

        // create mage
        var arcaneBolt = new CombatAction(Actions.Hit, 1, 40, 20);
        var mageArmour = new CombatAction(Actions.ApplyBuff, 3);
        var mageActions = new List<CombatAction>();

        mageActions.Add(arcaneBolt);
        mageActions.Add(mageArmour);

        var mage = new Unit(UnitKind.Mage, 80, 7, mageActions);
    }
}
