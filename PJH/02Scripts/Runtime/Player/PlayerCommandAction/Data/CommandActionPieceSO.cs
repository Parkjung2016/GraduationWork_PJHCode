using System;
using System.Collections.Generic;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Runtime.PlayerPassive;
using PJH.Shared;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLinq;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/CommandAction/Piece")]
    public class CommandActionPieceSO : PlayerPieceSO
    {
        public event Action OnChangePassive;

        [VerticalGroup("Top/Right")] [LabelText("⚔ 전투 데이터")]
        public PlayerCombatDataSO combatData;

        [BoxGroup("⚙ 패시브 설정", showLabel: true)] [LabelText("🌟 각인된 패시브들")] [SerializeField]
        private List<PassiveSO> _passives = new();

        public IReadOnlyList<PassiveSO> Passives => _passives;
        private GameEventChannelSO _uiEventChannel;
        public List<PassiveSO> PassivesList => _passives;
        private IPlayer _player;

        [ReadOnly] public int equipSlotIndex;

        public override void Init(IPlayer player)
        {
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            combatData = Instantiate(combatData);
            _player = player;
        }

        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
            OnChangePassive += HandleChangedPassive;

            for (int i = 0; i < _passives.Count; i++)
            {
                PassiveSO passive = _passives[i] = _passives[i].Clone<PassiveSO>();

                if (passive is IDependSlotPassive dependSlotPassive)
                {
                    if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                    {
                        if (passive is IDependSlotWeightModifier dependSlotWeightModifier)
                        {
                            dependSlotWeightModifier.ChangePassiveValueToWeightModifier();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (passive is IDependSlotWeightModifier dependSlotWeightModifier)
                    {
                        dependSlotWeightModifier.ChangePassiveValueToOrigin();
                    }
                }

                if (passive is IBuffPassive buffPassive)
                {
                    BuffPassiveInfo buffPassiveInfo = buffPassive.BuffPassiveInfo;
                    buffPassive.BuffPassiveInfo.ApplyBuffEvent += () =>
                    {
                        if (!_player.CanApplyPassive) return;
                        if (buffPassive is ICooldownPassive cooldownPassive)
                        {
                            if (cooldownPassive.CooldownPassiveInfo.remainingCooldownTime > 0)
                            {
                                Debug.LogError("쿨타임이 남아있습니다. 쿨타임: " +
                                               cooldownPassive.CooldownPassiveInfo.remainingCooldownTime);
                                return;
                            }
                        }

                        buffPassiveInfo.remainingBuffTime = buffPassiveInfo.buffDuration;
                        buffPassive.StartBuff();
                        var evt = UIEvents.ShowPassiveInfoUI;
                        evt.passive = passive;
                        evt.passiveInfoType = PassiveInfoType.Buff;
                        _uiEventChannel.RaiseEvent(evt);
                    };
                }

                if (passive is ICooldownPassive cooldownPassive)
                {
                    CooldownPassiveInfo cooldownPassiveInfo = cooldownPassive.CooldownPassiveInfo;
                    cooldownPassiveInfo.StartCooldownEvent += () =>
                    {
                        if (!_player.CanApplyPassive) return;
                        var evt = UIEvents.ShowPassiveInfoUI;
                        evt.passive = passive;
                        evt.passiveInfoType = PassiveInfoType.Cooldown;
                        _uiEventChannel.RaiseEvent(evt);

                        cooldownPassiveInfo.remainingCooldownTime = cooldownPassiveInfo.cooldownTime;
                        cooldownPassiveInfo.OnUpdateCooldownTime?.Invoke(cooldownPassiveInfo.remainingCooldownTime,
                            cooldownPassiveInfo.cooldownTime);
                    };
                }

                passive.EquipPiece(player);
            }
        }

        public override void UnEquipPiece()
        {
            base.UnEquipPiece();
            OnChangePassive -= HandleChangedPassive;
            _passives.ForEach(passive =>
            {
                if (passive is IDependSlotPassive dependSlotPassive)
                {
                    if (dependSlotPassive is not IDependSlotWeightModifier)
                        if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                            return;
                }

                passive.UnEquipPiece();
            });
        }

        private void HandleChangedPassive()
        {
            _passives.ForEach(passive =>
            {
                if (passive is IDependSlotPassive dependSlotPassive)
                {
                    if (dependSlotPassive is not IDependSlotWeightModifier)
                        if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                            return;
                }

                passive.EquipPiece(_player);
            });
        }

        public void ActivePassive()
        {
            if (!_player.CanApplyPassive) return;
            _passives.AsValueEnumerable().OfType<IActivePassive>().ToList()
                .ForEach(activePassive =>
                {
                    if (activePassive is IDependSlotPassive dependSlotPassive)
                    {
                        if (dependSlotPassive is not IDependSlotWeightModifier)
                            if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                                return;
                    }

                    if (activePassive is ICooldownPassive cooldownPassive)
                    {
                        if (cooldownPassive.CooldownPassiveInfo.remainingCooldownTime > 0)
                        {
                            Debug.LogError("쿨타임이 남아있습니다. 쿨타임: " +
                                           cooldownPassive.CooldownPassiveInfo.remainingCooldownTime);
                            return;
                        }
                    }

                    activePassive.ActivePassive();
                });
        }

        public void DeActivePassive()
        {
            _passives.AsValueEnumerable().OfType<IActivePassive>().ToList()
                .ForEach(activePassive =>
                {
                    if (activePassive is IDependSlotPassive dependSlotPassive)
                    {
                        if (dependSlotPassive is not IDependSlotWeightModifier)
                            if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                                return;
                    }

                    activePassive.DeActivePassive();
                });
        }

        public void UpdatePassive()
        {
            _passives.AsValueEnumerable().OfType<ICooldownPassive>().ToList()
                .ForEach(UpdateCooldownPassiveTime);
            _passives.AsValueEnumerable().OfType<IBuffPassive>().ToList()
                .ForEach(UpdateBuffPassiveTime);
        }

        private void UpdateBuffPassiveTime(IBuffPassive buffPassive)
        {
            if (buffPassive is IDependSlotPassive dependSlotPassive)
            {
                if (dependSlotPassive is not IDependSlotWeightModifier)
                    if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                        return;
            }

            if (buffPassive.BuffPassiveInfo.remainingBuffTime > 0)
            {
                BuffPassiveInfo buffPassiveInfo = buffPassive.BuffPassiveInfo;
                buffPassiveInfo.remainingBuffTime -= Time.deltaTime;
                if (buffPassiveInfo.remainingBuffTime < 0)
                {
                    buffPassiveInfo.remainingBuffTime = 0;

                    buffPassive.EndBuff();
                }
                else
                {
                    (buffPassive as IBuffPassiveUpdateable)?.UpdateBuff();
                }

                buffPassiveInfo.OnUpdateBuffTime?.Invoke(buffPassiveInfo.remainingBuffTime,
                    buffPassiveInfo.buffDuration);
            }
        }

        private void UpdateCooldownPassiveTime(ICooldownPassive cooldownPassive)
        {
            if (cooldownPassive is IDependSlotPassive dependSlotPassive)
            {
                if (cooldownPassive is not IDependSlotWeightModifier)
                    if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                        return;
            }

            if (cooldownPassive.CooldownPassiveInfo.remainingCooldownTime > 0)
            {
                CooldownPassiveInfo cooldownPassiveInfo = cooldownPassive.CooldownPassiveInfo;
                cooldownPassiveInfo.remainingCooldownTime -= Time.deltaTime;
                if (cooldownPassiveInfo.remainingCooldownTime < 0)
                {
                    cooldownPassiveInfo.remainingCooldownTime = 0;

                    if (cooldownPassive is ICooldownPassiveEndable cooldownPassiveEndable)
                    {
                        cooldownPassiveEndable.EndCooldown();
                    }
                }

                cooldownPassiveInfo.OnUpdateCooldownTime?.Invoke(cooldownPassiveInfo.remainingCooldownTime,
                    cooldownPassiveInfo.cooldownTime);
            }
        }

        public bool TryCombineCommandActionPiece(CommandActionPieceSO other)
        {
            if (!other) return false;
            if (other.pieceDisplayName != pieceDisplayName)
            {
                Debug.LogError("다른 타입의 CommandActionPiece를 합칠 수 없습니다.");
                return false;
            }

            if (_passives.Count + other._passives.Count > 3)
            {
                Debug.LogError("패시브의 최대 개수는 3개입니다.");
                return false;
            }

            if (other._passives.Count > 0)
            {
                foreach (var passive in other._passives)
                {
                    if (!_passives.Contains(passive))
                    {
                        _passives.Add(passive);
                    }
                }
            }

            other = null;
            Debug.Log("<color=green>병합 성공</color>");
            OnChangePassive?.Invoke();
            return true;
        }

        public bool TryAddPassive(PassiveSO passive)
        {
            _passives.Add(passive);
            return true;
        }

        public bool TryRemovePassive(PassiveSO passive)
        {
            if (_passives.Remove(passive))
            {
                if (passive is IBuffPassive buffPassive)
                {
                    BuffPassiveInfo buffPassiveInfo = buffPassive.BuffPassiveInfo;
                    buffPassiveInfo.ApplyBuffEvent = null;
                }

                if (passive is ICooldownPassive cooldownPassive)
                {
                    CooldownPassiveInfo cooldownPassiveInfo = cooldownPassive.CooldownPassiveInfo;
                    cooldownPassiveInfo.StartCooldownEvent = null;
                }

                return true;
            }

            return false;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            string newName = $"Command Action Piece_{combatData.name}";
            if (name == newName) return;
            string path = UnityEditor.AssetDatabase.GetAssetPath(this);
            UnityEditor.AssetDatabase.RenameAsset(path, newName);
        }

#endif
    }
}