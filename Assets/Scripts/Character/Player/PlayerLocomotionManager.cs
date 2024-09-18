using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRP
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;

        [SerializeField]
        float walkingSpeed = 2;

        [SerializeField]
        float runningSpeed = 5;

        [SerializeField]
        float rotationSpeed = 15;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {
            HandleGroundMovement();
            HandleRotationMovement();
            // Aerial
            // Fall
        }

        private void GetVerticalAndHorizontalInputs()
        {
            verticalMovement = PlayerInputManager.Instance.verticalInput;
            horizontalMovement = PlayerInputManager.Instance.horizontalInput;

            // Clamp movements for animations
        }

        private void HandleGroundMovement()
        {
            GetVerticalAndHorizontalInputs();

            // Movement is based on camera persective and user input
            moveDirection = PlayerCamera.Instance.transform.forward * verticalMovement;
            moveDirection += PlayerCamera.Instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (PlayerInputManager.Instance.moveAmount > 0.5f)
            {
                // Move at run speed
                player.characterController.Move(runningSpeed * Time.deltaTime * moveDirection);
            }
            else if (PlayerInputManager.Instance.moveAmount <= 0.5f)
            {
                // Move at walk speed
                player.characterController.Move(walkingSpeed * Time.deltaTime * moveDirection);
            }
        }

        private void HandleRotationMovement()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection =
                PlayerCamera.Instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection +=
                PlayerCamera.Instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(
                transform.rotation,
                newRotation,
                rotationSpeed * Time.deltaTime
            );
            transform.rotation = targetRotation;
        }

        private void HandleAerialMovement() { }
    }
}
