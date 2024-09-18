using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SRP
{
    public class CharacterManager : NetworkBehaviour
    {
        public CharacterController characterController;

        CharacterNetworkManager characterNetworkManager;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        protected virtual void Update()
        {
            // Update for host
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            // Update for clients connected to host
            else
            {
                transform.SetPositionAndRotation(
                    // Position
                    Vector3.SmoothDamp(
                        transform.position,
                        characterNetworkManager.networkPosition.Value,
                        ref characterNetworkManager.networkPositionVelocity,
                        characterNetworkManager.networkPositionSmoothTime
                    ),
                    // Rotation
                    Quaternion.Slerp(
                        transform.rotation,
                        characterNetworkManager.networkRotation.Value,
                        characterNetworkManager.networkRotationSmoothTime
                    )
                );
            }
        }

        protected virtual void LateUpdate() { }
    }
}
