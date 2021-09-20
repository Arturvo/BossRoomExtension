using System;
using Unity.Netcode;
using UnityEngine;

namespace BossRoom.Client
{
    public class ClientPlayerAvatar : NetworkBehaviour
    {
        [SerializeField]
        ClientPlayerAvatarRuntimeCollection m_PlayerAvatars;

        public static event Action<ClientPlayerAvatar> LocalClientSpawned;

        public static event Action LocalClientDespawned;

        public override void OnNetworkSpawn()
        {
            name = "PlayerAvatar" + OwnerClientId;

            if (IsClient && IsOwner)
            {
                LocalClientSpawned?.Invoke(this);
            }

            if (m_PlayerAvatars)
            {
                m_PlayerAvatars.Add(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient && IsOwner)
            {
                LocalClientDespawned?.Invoke();
            }

            RemoveNetworkCharacter();
        }

        void OnDestroy()
        {
            RemoveNetworkCharacter();
        }

        void RemoveNetworkCharacter()
        {
            if (m_PlayerAvatars)
            {
                m_PlayerAvatars.Remove(this);
            }
        }
    }
}