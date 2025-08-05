using System;
using System.Collections.Generic;
using System.Linq;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Main.Runtime.Equipments.Datas;
using Main.Runtime.Equipments.Scripts;
using Main.Runtime.Equipments.Scripts.Weapons;
using Main.Shared;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[assembly: ZLinq.ZLinqDropInAttribute("Main.Runtime.Agents", ZLinq.DropInGenerateTypes.Everything)]

namespace Main.Runtime.Agents
{
    public class AgentEquipmentSystem : SerializedMonoBehaviour, IAgentComponent, IAfterInitable
    {
        [SerializeField] public StatSO _powerStat, _increaseMomentumGaugeStat;
        [SerializeField] private EquipmentDatabaseSO equipmentDatabase;
        [SerializeField] private bool _startWithEquipment;

        [SerializeField, ShowIf("_startWithEquipment", true)]
        private Dictionary<Define.ESocketType, EquipmentDataSO> _startEquipmentDatas;

        [ReadOnly, SerializeField] Dictionary<Define.ESocketType, AgentSocket> _sockets = new();
        private Agent _agent;
        [SerializeField] private bool _tutorialPlayer;

        private readonly int socketTypeLength = Enum.GetValues(typeof(Define.ESocketType)).Length;

        public void Initialize(Agent agent)
        {
            _agent = agent;
            _powerStat = agent.GetCompo<AgentStat>(true).GetStat(_powerStat);
            _increaseMomentumGaugeStat = agent.GetCompo<AgentStat>(true).GetStat(_increaseMomentumGaugeStat);

            InitializeSocketCache();
            EquipmentDataSO emptyWeaponData =
                equipmentDatabase.equipmentDatas.Find(x => x.equipmentPrefab.GetType() == typeof(EmptyWeapon));
            if (!_startWithEquipment)
            {
                foreach (Define.ESocketType socketType in Enum.GetValues(typeof(Define.ESocketType)))
                {
                    InitEquipment(emptyWeaponData, socketType);
                }
            }
            else
            {
                foreach (Define.ESocketType socketType in Enum.GetValues(typeof(Define.ESocketType)))
                {
                    if (_startEquipmentDatas.ContainsKey(socketType)) continue;
                    InitEquipment(emptyWeaponData, socketType);
                }

                foreach (var pair in _startEquipmentDatas)
                {
                    InitEquipment(pair.Value, pair.Key);
                }
            }
        }

        private void InitEquipment(EquipmentDataSO data, Define.ESocketType socketType)
        {
            if (!_sockets.TryGetValue(socketType, out AgentSocket socket)) return;

            Equipment equipment = Instantiate(data.equipmentPrefab);

            if (equipment is Weapon weapon)
            {
                weapon.Equip(_agent, _powerStat, _increaseMomentumGaugeStat);
            }
            else
                equipment.Equip(_agent);

            socket.ChangeItem(equipment, data.equipmentPosition,
                data.equipmentRotation);
            if (_tutorialPlayer)
            {
                equipment.transform.localScale = Vector3.one * 100f;
            }
        }

        public void AfterInitialize()
        {
            _agent.HealthCompo.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            _agent.HealthCompo.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            foreach (var socket in _sockets.Values)
            {
                Weapon weapon = socket.GetItem<Weapon>();
                if (!weapon) continue;
                weapon.DisableDamageCollider();
            }
        }

        private void InitializeSocketCache()
        {
            _sockets.Clear();
            AgentSocket[] agentSockets = _agent.GetComponentsInChildren<AgentSocket>();
            foreach (Define.ESocketType socketType in Enum.GetValues(typeof(Define.ESocketType)))
            {
                AgentSocket socket = agentSockets.FirstOrDefault(socket => socket.socketType == socketType);
                if (socket)
                {
                    _sockets.Add(socketType, socket);
                }
            }
        }

        public AgentSocket GetSocket(Define.ESocketType socketType) => _sockets[socketType];

#if UNITY_EDITOR

        [Flags, EnumPaging]
        enum SelectSocketType
        {
            LeftHand = 1 << 0,
            RightHand = 1 << 1,
            LeftLowerArm = 1 << 2,
            RightLowerArm = 1 << 3,
            LeftFoot = 1 << 4,
            RightFoot = 1 << 5,
            LeftLowerLeg = 1 << 6,
            RightLowerLeg = 1 << 7,
        }

        [GUIColor(0, 1, 0)]
        [HideIf("CheckExistsAllSocket")]
        [Button(ButtonStyle.FoldoutButton)]
        private void AddSocketInBone(SelectSocketType selectSocketType)
        {
            AgentSocket[] sockets = transform.parent.GetComponentsInChildren<AgentSocket>();
            Animator animator = transform.parent.GetComponentInChildren<Animator>();
            foreach (SelectSocketType socketType in Enum.GetValues(typeof(SelectSocketType)))
            {
                if (!selectSocketType.HasFlag(socketType)) continue;
                if (sockets.Any(socket => socket.socketType.ToString() == socketType.ToString()))
                {
                    Debug.LogError($"Socket_{socketType} already exists.");
                    continue;
                }

                Define.ESocketType type = Enum.Parse<Define.ESocketType>(socketType.ToString());
                CreateSocket(animator, type);
            }

            Debug.Log($"<b><color=yellow>Successfully added socket.</color></b>");
        }

        [GUIColor(1, 0, 0)]
        [ShowIf("@this.transform.parent.GetComponentInChildren<AgentSocket>() != null")]
        [Button(ButtonStyle.FoldoutButton)]
        private void RemoveSocketInBone(SelectSocketType removeSocketType)
        {
            AgentSocket[] sockets = transform.parent.GetComponentsInChildren<AgentSocket>();

            foreach (SelectSocketType socketType in Enum.GetValues(typeof(SelectSocketType)))
            {
                if (!removeSocketType.HasFlag(socketType)) continue;
                AgentSocket socket = sockets.FirstOrDefault(socket =>
                    socket.socketType.ToString() == socketType.ToString());
                if (socket == null)
                {
                    Debug.LogError($"Socket_{socketType} does not exist.");
                    return;
                }

                DestroyImmediate(socket.gameObject);
            }

            Debug.Log($"<color=yellow>Successfully removed socket.</color>");
        }

        private void CreateSocket(Animator animator, Define.ESocketType socketType)
        {
            HumanBodyBones bodyType = Enum.Parse<HumanBodyBones>(socketType.ToString());
            Transform parent = animator.GetBoneTransform(bodyType);
            GameObject socket = new GameObject();
            socket.AddComponent<AgentSocket>().ChangeSocketType(socketType);
            socket.transform.SetParent(parent);
            socket.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            socket.transform.localScale = Vector3.one;
        }

        private bool CheckExistsAllSocket()
        {
            AgentSocket[] sockets = transform.parent.GetComponentsInChildren<AgentSocket>();
            int checkCount = 0;
            foreach (Define.ESocketType socketType in Enum.GetValues(typeof(Define.ESocketType)))
            {
                foreach (AgentSocket socket in sockets)
                {
                    if (socket.socketType == socketType)
                    {
                        checkCount++;
                        break;
                    }
                }
            }

            return checkCount == socketTypeLength;
        }

        [GUIColor(0, 1, 0)]
        [ShowIf("@this.transform.parent.GetComponentInChildren<AgentSocket>() != null")]
        [Button(ButtonStyle.FoldoutButton)]
        private void SelectAgentSocket(Define.ESocketType socketType)
        {
            AgentSocket socket = transform.parent.GetComponentsInChildren<AgentSocket>()
                .FirstOrDefault(socket => socket.socketType == socketType);

            if (socket == null)
            {
                Debug.LogError($"Could not find \"Socket_{socketType}\"");
                return;
            }

            Selection.activeGameObject = socket.gameObject;
        }
#endif
    }
}