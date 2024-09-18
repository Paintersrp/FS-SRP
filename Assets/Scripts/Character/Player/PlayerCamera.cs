using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRP
{
    public class PlayerCamera : MonoBehaviour
    {
        private static PlayerCamera _instance;
        private static readonly object _lock = new();

        public static PlayerCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            PlayerCamera[] managers = FindObjectsOfType<PlayerCamera>();
                            if (managers.Length > 0)
                            {
                                _instance = managers[0];
                            }
                            if (managers.Length > 1)
                            {
                                Debug.LogError("There is more than one PlayerCamera in the scene!");
                            }
                            if (_instance == null)
                            {
                                GameObject obj = new() { name = typeof(PlayerCamera).Name };

                                _instance = obj.AddComponent<PlayerCamera>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        public Camera cameraObject;
        public PlayerManager player;

        [SerializeField]
        Transform cameraPivotTransform;

        [Header("Camera Settings")]
        private readonly float cameraSmoothSpeed = 1;

        [SerializeField]
        float leftAndRightRotationSpeed = 220;

        [SerializeField]
        float upAndDownRotationSpeed = 220;

        [SerializeField]
        float minimumPivot = -30;

        [SerializeField]
        float maximumPivot = 60;

        [SerializeField]
        float cameraCollisionSmoothSpeed = 0.2f;

        [SerializeField]
        float cameraCollisionRadius = 0.2f;

        [SerializeField]
        LayerMask collideWithLayers;

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition;

        [SerializeField]
        float leftAndRightLookAngle;

        [SerializeField]
        float upAndDownLookAngle;
        private float cameraZPosition;
        private float targetCameraZPosition;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(
                transform.position,
                player.transform.position,
                ref cameraVelocity,
                cameraSmoothSpeed * Time.deltaTime
            );

            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            // TODO: Needs force rotation on lock on

            leftAndRightLookAngle +=
                PlayerInputManager.Instance.cameraHorizontalInput
                * leftAndRightRotationSpeed
                * Time.deltaTime;

            upAndDownLookAngle -=
                PlayerInputManager.Instance.cameraVerticalInput
                * upAndDownRotationSpeed
                * Time.deltaTime;

            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            // Left & Right
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // Up & Down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            // Check for collison
            if (
                Physics.SphereCast(
                    cameraPivotTransform.position,
                    cameraCollisionRadius,
                    direction,
                    out hit,
                    Mathf.Abs(targetCameraZPosition),
                    collideWithLayers
                )
            )
            {
                float distanceFromHitObject = Vector3.Distance(
                    cameraPivotTransform.position,
                    hit.point
                );
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            // If the target position is within our collison radius, we subtract to snap the camera back
            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            // Apply final position to camera object
            cameraObjectPosition.z = Mathf.Lerp(
                cameraObject.transform.localPosition.z,
                targetCameraZPosition,
                cameraCollisionSmoothSpeed
            );
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}
