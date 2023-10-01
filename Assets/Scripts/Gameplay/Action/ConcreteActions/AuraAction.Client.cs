using System.Collections.Generic;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using Unity.BossRoom.VisualEffects;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions
{
    public partial class AuraAction
    {
        private bool m_ClientAuraStarted;
        private float m_ClientLastApplyTime;

        /// <summary>
        /// List of active special graphics playing on all aura targets.
        /// </summary>
        private List<SpecialFXGraphic> m_SpawnedGraphics = null;

        public override bool OnUpdateClient(ClientCharacter clientCharacter)
        {
            if (!m_ClientAuraStarted && TimeRunning >= Config.ExecTimeSeconds)
            {
                // begin visualising the aura effect after execution completed.
                m_ClientAuraStarted = true;
                m_ClientLastApplyTime = Time.time;
            }
            else if (m_ClientAuraStarted && Time.time - m_ClientLastApplyTime >= Config.EffectRepeatSeconds)
            {
                // every Config.EffectRepeatSeconds after aura started visualise aura effect.
                VisualiseAuraEffect(clientCharacter);
            }

            return ActionConclusion.Continue;
        }

        public override void CancelClient(ClientCharacter clientCharacter)
        {
            // if we had any special target graphics, tell them we're done.
            if (m_SpawnedGraphics != null)
            {
                foreach (var spawnedGraphic in m_SpawnedGraphics)
                {
                    if (spawnedGraphic)
                    {
                        spawnedGraphic.Shutdown();
                    }
                }
            }
        }

        private void VisualiseAuraEffect(ClientCharacter caster)
        {
            // aura effect is visualised on all allies in a set radius around the caster.
            m_ClientLastApplyTime = Time.time;
            m_SpawnedGraphics = new List<SpecialFXGraphic>();

            var alliesInRadius = FindAllAlliesInRadius(caster.NetworkObjectId);

            foreach (var ally in alliesInRadius)
            {
                if (PhysicsWrapper.TryGetPhysicsWrapper(ally.NetworkObjectId, out var physicsWrapper))
                {
                    InstantiateSpecialFXGraphics(physicsWrapper.Transform, true).ForEach(graphic => m_SpawnedGraphics.Add(graphic));
                }
            }
        }
    }
}
