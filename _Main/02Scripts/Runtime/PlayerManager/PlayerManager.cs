using System;
using Main.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Runtime.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        public event Action<int> OnChangedHeartCount;
        private static PlayerManager _instance;
        private int _currentHeartCount = 3;

        public int CurrentHeartCount
        {
            get => _currentHeartCount;
            set
            {
                _currentHeartCount = Mathf.Max(value, 0);
                OnChangedHeartCount?.Invoke(value);
            }
        }

        public static PlayerManager Instance;

        private IPlayer _player;

        public IPlayer Player
        {
            get
            {
                if (_player == null)
                {
                    _player = GameObject.FindGameObjectWithTag("Player").GetComponent<IPlayer>();
                }

                return _player;
            }
        }

        private ICamera _playerCamera;

        public ICamera PlayerCamera
        {
            get
            {
                if (_playerCamera == null)
                {
                    _playerCamera = GameObject.FindGameObjectWithTag("MainPlayerCamera").GetComponent<ICamera>();
                }

                return _playerCamera;
            }
        }

        private void Awake()
        {
            PlayerManager[] instances = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);

            if (instances.Length > 1)
                Destroy(gameObject);
            else
            {
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += HandleSceneLoaded;
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<IPlayer>();
            _playerCamera = GameObject.FindGameObjectWithTag("MainPlayerCamera").GetComponent<ICamera>();
        }
    }
}