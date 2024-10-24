# Combat AI & Engine

## Description
> A simple combat simulator with two teams of one or two combat units. These units fight until there is only one side remaining, using 'spells' (attack, heal) to win the battle.
> These spells can have extra effects (damage over time, freezing) and have crit chances for 4x damage.

## Purpose
> This is a personal project I have created as a fun way to build on my fundamental knowledge of C#, as well as learn how and why different patterns are used in practical code.
> It is inspired by turn-based RPGs, most notably Heroes of Might and Magic 3 as the units are automated instead of controlled. Pokémon and Final Fantasy-style games are also minor influences.
> I enjoy working with numbers and sums, so I wanted to work with calculating crit chances and the randomised damage amounts.

## Classes
#### Unit Handling
> Class Builder class:
> 
> Unit class: The initial state of the unit; how much health it starts with, its total spells known, the side it belongs to, its name, and its speed in the turn order.
> 
> UnitState class: The current state of the unit; the unit's health, spells available, and any over time effects are stored in each state as these may change.
> 
> Spell class: Any action that the unit can take; creates the cooldown, type of spell, the effect that the spell does, and the crit effect (if it exists).
> 
> SpellEffect class: The effect that any spell applies; stores if the spell harms or heals, whether it's direct or timed, the amount of max and min damage the spell does, the crit chance of the spell, and the crit multiplier.
> 
#### Combat Handling
> Combat.cs
> 
> FightBuilder.cs
> 
> TimedOverTimeEffect.cs:
> 
> TimedSpell.cs:
> 
> UnitBehaviour.cs
> 
#### Other
> Enums.cs
> 
> ICombatLog.cs
> 
> ConsoleCombatLog.cs
> 
> ConsoleCombatListener.cs
> 
> BalanceConsole project
> 



## Decisions and Changes
> Reasons for using certain patterns over others
> 
> States:
> 
> OLD: UnitStateBuilder - no longer using a builder pattern, as it is more efficient for a unit state to 
 
## Future Plans
#### TODO:
>  Apply alpha-beta pruning to decision-making instead of it being random
#### Extensions:
>  Add new classes
> 
>  Damage types?
