using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions
{
    /// <summary>
    /// This aura recovers mana to every ally it is applied to.
    /// </summary>
    [CreateAssetMenu(menuName = "BossRoom/Actions/Mana Regen Aura Action")]
    public class ManaRegenAuraAction : AuraAction
    {
        protected override void ApplyAuraEffectToAlly(ServerCharacter parent, IDamageable ally)
        {
            ally.ReceiveMana(parent, -Config.Amount);
        }
    }
}
