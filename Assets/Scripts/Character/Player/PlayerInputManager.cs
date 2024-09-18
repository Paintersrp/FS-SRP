using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRP
{
    public class PlayerInputManager : MonoBehaviour
    {
        private static PlayerInputManager _instance;
        private static readonly object _lock = new();

        public static PlayerInputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            PlayerInputManager[] managers = FindObjectsOfType<PlayerInputManager>();
                            if (managers.Length > 0)
                            {
                                _instance = managers[0];
                            }
                            if (managers.Length > 1)
                            {
                                Debug.LogError(
                                    "There is more than one PlayerInputManager in the scene!"
                                );
                            }
                            if (_instance == null)
                            {
                                GameObject obj = new() { name = typeof(PlayerInputManager).Name };

                                _instance = obj.AddComponent<PlayerInputManager>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        PlayerControls playerControls;

        [Header("MOVEMENT INPUT")]
        [SerializeField]
        private Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("CAMERA INPUT")]
        [SerializeField]
        private Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChange;
            Instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Only enable input when in the world scene
            if (newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())
            {
                Instance.enabled = true;
            }
            else
            {
                Instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i =>
                    movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i =>
                    cameraInput = i.ReadValue<Vector2>();
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        // Stops input when window isn't focused
        // Avoids multi client input when testing
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
        }

        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
    }
}
