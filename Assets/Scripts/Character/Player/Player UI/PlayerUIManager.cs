using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SRP
{
    public class PlayerUIManager : MonoBehaviour
    {
        private static PlayerUIManager _instance;
        private static readonly object _lock = new();

        public static PlayerUIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            PlayerUIManager[] managers = FindObjectsOfType<PlayerUIManager>();
                            if (managers.Length > 0)
                            {
                                _instance = managers[0];
                            }
                            if (managers.Length > 1)
                            {
                                Debug.LogError(
                                    "There is more than one PlayerUIManager in the scene!"
                                );
                            }
                            if (_instance == null)
                            {
                                GameObject obj = new() { name = typeof(PlayerUIManager).Name };

                                _instance = obj.AddComponent<PlayerUIManager>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        [Header("NETWORK JOIN")]
        [SerializeField]
        bool startGameAsClient;

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

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;

                // Shutdown as host such that we can connect as a client
                NetworkManager.Singleton.Shutdown();

                // Restart as client
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
