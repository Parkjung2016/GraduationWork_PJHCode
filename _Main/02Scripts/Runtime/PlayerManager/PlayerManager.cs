using System;
using Main.Runtime.Agents;
using PJH.Runtime.Core.PlayerCamera;
using UnityEngine;

namespace Main.Runtime.Manager
{
    [DefaultExecutionOrder(-10000)]
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;

        public static PlayerManager Instance;

        private Agent _player;

        public Agent Player
        {
            get
            {
                if (_player == null)
                {
                    _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Agent>();
                }

                return _player;
            }
        }

        private PlayerCamera _playerCamera;

        public PlayerCamera PlayerCamera
        {
            get
            {
                if (_playerCamera == null)
                {
                    _playerCamera = GameObject.FindGameObjectWithTag("MainPlayerCamera")?.GetComponent<PlayerCamera>();
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
                Instance = this;
            }
        }
    }
}