using KHJ.Shared;
using Main.Runtime.Agents;
using Main.Shared;
using PJH.Shared;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Main.Runtime.Combat;
using Main.Runtime.Core.StatSystem;
using UnityEngine;
using Main.Core;
using Main.Runtime.Core.Events;
using Debug = Main.Core.Debug;


namespace PJH.Runtime.PlayerPassive
{
    public enum PassiveRankType
    {
        Low,
        Middle,
        High
    }

    public abstract class PassiveSO : PlayerPieceSO
    {
        private Dictionary<ModifierStatType, Action<AgentStat, StatSO, object>> RemoveModifierStatActions = new()
        {
            {
                ModifierStatType.IncreaseValue, (stat, s, key) => { stat.RemoveValueModifier(s, key); }
            },
            {
                ModifierStatType.DecreaseValue, (stat, s, key) => stat.RemoveValueModifier(s, key)
            },
            {
                ModifierStatType.IncreaseValuePercent,
                (stat, s, key) => stat.RemoveValuePercentModifier(s, key)
            },
            {
                ModifierStatType.DecreaseValuePercent,
                (stat, s, key) => { stat.RemoveValuePercentModifier(s, key); }
            },
        };

        private Dictionary<ModifierStatType, Action<AgentStat, StatSO, object, float>> AddModifierStatActions = new()
        {
            { ModifierStatType.IncreaseValue, (stat, s, key, val) => stat.AddValueModifier(s, key, val) },
            { ModifierStatType.DecreaseValue, (stat, s, key, val) => stat.AddValueModifier(s, key, -val) },
            {
                ModifierStatType.IncreaseValuePercent,
                (stat, s, key, val) => stat.AddValuePercentModifier(s, key, val)
            },
            {
                ModifierStatType.DecreaseValuePercent,
                (stat, s, key, val) => stat.AddValuePercentModifier(s, key, -val)
            },
        };

        private Dictionary<ModifierShieldType, Action<Health, float>> RemoveModifierShieldActions = new()
        {
            {
                ModifierShieldType.Flat, (health, value) => { health.CurrentShield -= value; }
            },
            {
                ModifierShieldType.MaxHpPercent,
                (health, value) => { health.CurrentShield -= health.MaxHealth * (value * .01f); }
            },
        };

        private Dictionary<ModifierShieldType, Action<Health, float>> AddModifierShieldActions = new()
        {
            {
                ModifierShieldType.Flat, (health, value) => { health.CurrentShield += value; }
            },
            {
                ModifierShieldType.MaxHpPercent,
                (health, value) => { health.CurrentShield += health.MaxHealth * (value * .01f); }
            },
        };

        [field: SerializeField]
        [field: LabelText("등급")]
        [field: EnumToggleButtons]
        public PassiveRankType RankType { get; private set; }

        protected Agent _player;

        [SerializeField] private int _maxDetectEnemyCount = 5;
        protected GameEventChannelSO _spawnEventChannel;
        protected GameEventChannelSO _uiEventChannel;
        protected GameEventChannelSO _gameEventChannel;
        protected Collider[] _detectColliders;

        private bool _lock = true;

        public override void Init(IPlayer player)
        {
            _player = player as Agent;
            _detectColliders = new Collider[_maxDetectEnemyCount];
            _spawnEventChannel = AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _lock = false;
        }

        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
            if (this is not ICooldownPassive and not IBuffPassive)
            {
                if (player.CanApplyPassive)
                {
                    var evt = UIEvents.ShowPassiveInfoUI;
                    evt.passive = this;
                    evt.passiveInfoType = PassiveInfoType.None;
                     _uiEventChannel.RaiseEvent(evt);
                }
            }

            if (!_lock)
            {
                player.OnChangedCanApplyPassive += HandleChangedCanApplyPassive;
                if (this is IModifierStatPassive modifierStatPassive)
                {
                    modifierStatPassive.ModifierStatInfo.RemoveModifierEvent = HandleRemoveModifierEvent;
                    modifierStatPassive.ModifierStatInfo.AddModifierEvent = HandleAddModifierEvent;
                }
            }
        }

        private void HandleChangedCanApplyPassive(bool canApplyPassive)
        {
            if (!canApplyPassive)
            {
                _lock = true;
                UnEquipPiece();
                _lock = false;
            }
            else
            {
                _lock = true;
                EquipPiece(_player as IPlayer);
                _lock = false;
            }
        }

        public override void UnEquipPiece()
        {
            base.UnEquipPiece();
            if (!_lock)
            {
                (_player as IPlayer).OnChangedCanApplyPassive -= HandleChangedCanApplyPassive;
                if (this is IModifierStatPassive modifierStatPassive)
                {
                    modifierStatPassive.ModifierStatInfo.AddModifierEvent -= HandleAddModifierEvent;
                    // modifierStatPassive.ModifierStatInfo.RemoveModifierEvent -= HandleRemoveModifierEvent;
                }
            }
        }

        private void HandleAddModifierEvent(ModifierStatInfo modifierStatInfo)
        {
            AgentStat statCompo = _player.ComponentManager.GetCompo<AgentStat>(true);
            foreach (ModifierStat mod in modifierStatInfo.ModifierStats)
            {
                switch (mod.modifierType)
                {
                    case ModifierType.Stat:
                    {
                        if (AddModifierStatActions.TryGetValue(mod.modifierStatType, out var value))
                        {
                            value.Invoke(statCompo, mod.modifierStat, this, mod.modifierValue);
                        }

                        break;
                    }
                    case ModifierType.Shield:
                    {
                        if (AddModifierShieldActions.TryGetValue(mod.modifierShieldType, out var value))
                        {
                            value.Invoke(_player.HealthCompo, mod.modifierValue);
                        }

                        break;
                    }
                }
            }
        }

        private void HandleRemoveModifierEvent(ModifierStatInfo modifierStatInfo)
        {
            AgentStat statCompo = _player.ComponentManager.GetCompo<AgentStat>(true);

            foreach (ModifierStat mod in modifierStatInfo.ModifierStats)
            {
                switch (mod.modifierType)
                {
                    case ModifierType.Stat:
                    {
                        if (RemoveModifierStatActions.TryGetValue(mod.modifierStatType, out var value))
                        {
                            value.Invoke(statCompo, mod.modifierStat, this);
                        }

                        break;
                    }
                    case ModifierType.Shield:
                    {
                        if (RemoveModifierShieldActions.TryGetValue(mod.modifierShieldType, out var value))
                        {
                            value.Invoke(_player.HealthCompo, mod.modifierValue);
                        }

                        break;
                    }
                }
            }
        }


        protected List<Agent> FindEnemiesInRange(Transform checkTransform, float radius)
        {
            int cnt = Physics.OverlapSphereNonAlloc(checkTransform.position, radius,
                _detectColliders, Define.MLayerMask.WhatIsEnemy);
            List<Agent> list = new List<Agent>();

            for (int i = 0; i < cnt; ++i)
            {
                if (_detectColliders[i].TryGetComponent(out Agent enemy))
                {
                    if (!enemy.HealthCompo.IsDead)
                        list.Add(enemy);
                }
            }

            return list;
        }

        public float GetPassiveCoolTime()
        {
            if (this is ICooldownPassive cooldown)
            {
                return cooldown.CooldownPassiveInfo.cooldownTime;
            }

            return 0;
        }

        protected void PlayEffect(PoolTypeSO poolType, Vector3 pos, Quaternion rot)
        {
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = poolType;
            evt.position = pos;
            evt.rotation = rot;
            _spawnEventChannel.RaiseEvent(evt);
        }
    }
}