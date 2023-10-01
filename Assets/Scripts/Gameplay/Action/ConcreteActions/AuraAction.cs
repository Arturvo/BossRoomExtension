using System.Collections.Generic;
using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions
{
    /// <summary>
    /// The Aura Action continuously applies an effect to the caster and allies in a set radius around the caster.
    /// This class is abstract to allow different aura effects. To create a usable aura, create an inheriting class and implement ApplyAuraToAlly().
    /// </summary>
    public abstract partial class AuraAction : Action
    {
        private bool m_AuraStarted;
        private float m_LastApplyTime;

        public override bool OnStart(ServerCharacter serverCharacter)
        {
            // broadcasting to all players including myself.
            Data.TargetIds = new ulong[0];
            serverCharacter.serverAnimationHandler.NetworkAnimator.SetTrigger(Config.Anim);
            serverCharacter.clientCharacter.RecvDoActionClientRPC(Data);
            return ActionConclusion.Continue;
        }

        public override void Reset()
        {
            base.Reset();
            m_AuraStarted = false;
            m_ClientAuraStarted = false;
        }

        public override bool OnUpdate(ServerCharacter serverCharacter)
        {
            if (!m_AuraStarted && TimeRunning >= Config.ExecTimeSeconds)
            {
                // begin applying the aura effect after execution completed.
                m_AuraStarted = true;
                m_LastApplyTime = Time.time;
            }
            else if (m_AuraStarted && Time.time - m_LastApplyTime >= Config.EffectRepeatSeconds)
            {
                // every Config.EffectRepeatSeconds after aura started apply aura effect.
                ApplyAuraEffect(serverCharacter);
            }

            return ActionConclusion.Continue;
        }

        private void ApplyAuraEffect(ServerCharacter caster)
        {
            // aura effect is applied to all allies in a set radius around the caster.
            m_LastApplyTime = Time.time;
            var alliesInRadius = FindAllAlliesInRadius(caster.NetworkObjectId);

            foreach (var ally in  alliesInRadius)
            {
                ApplyAuraEffectToAlly(caster, ally);
            }
        }

        private List<IDamageable> FindAllAlliesInRadius(ulong casterNetworkObjectId)
        {
            List<IDamageable> alliesInRadius = new List<IDamageable>();

            if (PhysicsWrapper.TryGetPhysicsWrapper(casterNetworkObjectId, out var physicsWrapper))
            {
                var colliders = Physics.OverlapSphere(physicsWrapper.transform.position, Config.Radius, LayerMask.GetMask("PCs"));
                for (var i = 0; i < colliders.Length; i++)
                {
                    var ally = colliders[i].GetComponent<IDamageable>();
                    if (ally != null)
                    {
                        alliesInRadius.Add(ally);
                    }
                }
            }

            return alliesInRadius;
        }

        /// <summary>
        /// Define what happens when aura effect is applied to an ally.
        /// </summary>
        /// <param name="caster">Character casting the aura. </param>
        /// <param name="ally">Character receiving the aura effect.  </param>
        protected abstract void ApplyAuraEffectToAlly(ServerCharacter parent, IDamageable ally);
    }
}
