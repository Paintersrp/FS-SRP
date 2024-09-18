using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRP
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        private static WorldSaveGameManager _instance;
        private static readonly object _lock = new();

        public static WorldSaveGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            WorldSaveGameManager[] managers =
                                FindObjectsOfType<WorldSaveGameManager>();
                            if (managers.Length > 0)
                            {
                                _instance = managers[0];
                            }
                            if (managers.Length > 1)
                            {
                                Debug.LogError(
                                    "There is more than one WorldSaveGameManager  in the scene!"
                                );
                            }
                            if (_instance == null)
                            {
                                GameObject obj = new() { name = typeof(WorldSaveGameManager).Name };

                                _instance = obj.AddComponent<WorldSaveGameManager>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        [SerializeField]
        int worldSceneIndex = 1;

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
        }

        public IEnumerator LoadNewGame()
        {
            //
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}
