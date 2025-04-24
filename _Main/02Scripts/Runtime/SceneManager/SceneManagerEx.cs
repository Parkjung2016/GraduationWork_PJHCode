using System;
using BIS.Manager;
using Main.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Runtime.Manager
{
    public class SceneManagerEx : MonoBehaviour
    {
        public static SceneManagerEx Instance;
        public IScene CurrentScene { get; private set; }

        private bool _isActiveEyeEffect;

        private void Awake()
        {
            SceneManagerEx[] managers = FindObjectsByType<SceneManagerEx>(FindObjectsSortMode.None);
            if (managers.Length > 1)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                GetCurrentScene();
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GetCurrentScene();
            if (_isActiveEyeEffect)
            {
                Managers.VolumeManager.SetBlink(1, 0);
                Managers.VolumeManager.SetBlink(0, 1);
                _isActiveEyeEffect = false;
            }
        }

        private void GetCurrentScene()
        {
            CurrentScene = GameObject.FindWithTag("Scene")?.GetComponent<IScene>();
        }

        public static void LoadScene(string sceneName, bool isActiveEyeEffect = false)
        {
            Instance._isActiveEyeEffect = isActiveEyeEffect;
            SceneControlManager.LoadScene(sceneName);
        }
    }
}