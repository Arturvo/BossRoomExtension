# Boss Room Extension
### An extension to the Boss Room Unity sample project, adding a mana regeneration aura ability to the mage character class with the following functionality:
The mage now has a third ability, which, after use, regenerates the mana of nearby allies over time (including the mage). The aura lasts for a specified number of seconds, applies every specified number of seconds, and works in a set radius around the caster. The aura fires a specified casting animation at the start and creates specified VFX on every character every time it is applied to them.

### Changes made to the original project:
1. Added the ability for the characters to gain and lose mana, consistent with how they do HP:
   - Created a NetworkManaState class similar to NetworkHealthState.
   - Added the NetManaState variable to the ServerCharacter class, similar to NetHealthState.
   - Changed the BaseMana variable in CharacterClass from int to IntVariable.
   - Added InitializeMana() and ReceiveMana() methods to the ServerCharacter class. Also modified the ITargetable and IDamageable interfaces accordingly, together with the DamageReceiver, Breakable and AIBrain classes.
2. Implemented aura logic:
   - Added the EffectRepeatSeconds variable into the ActionConfig class to specify a new possible action property characteristic for the aura.
   - Added a new generic parial action class, AuraAction, to specify aura logic on client and server. Decided on the generic approach to leave the door open for other aura effects.
   - Added Aura to the ActionLogic enum and set the ShouldClose parameter to false for it in the ClientInputSender.PopulateSkillRequest() method to allow casting the aura irrespective of the distance from the selected target.
   - Implemented the ManaRegenAuraAction class based on the generic class AuraAction.
4. Created a scriptable object for the new ManaRegenAuraAction ability and plugged it into the mage character scriptable object and GameDataSource prefab. The mana regeneration aura has the same icon and VFX as the healing ability as a placeholder.

### Testing:
In order to test the new functionality, you can, for example, hook into the Mana.OnValueChanged event in the NetworkManaState class. Note that for it to fire, the mana value needs to change, which it won't if it is already at its maximum value and is being increased. To make the event fire, either set the starting mana to 0 in ServerCharacter.InitializeMana() or set the Amount variable to positive in the MageManaRegen scriptable object (which will cause the aura to decrease the mana over time instead of increasing).
