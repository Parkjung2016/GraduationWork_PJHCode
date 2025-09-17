using System;
using System.Collections.Generic;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Runtime.PlayerPassive;
using PJH.Shared;
using PJH.Utility.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLinq;

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
            if (!_player.CanApplyPassive) return;
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
                    buffPassive.BuffPassiveInfo.ApplyBuffEvent = () =>
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
                        buffPassiveInfo.isBuffing = true;
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
                    cooldownPassiveInfo.StartCooldownEvent = () =>
                    {
                        if (!_player.CanApplyPassive) return;
                        cooldownPassiveInfo.isCooldowning = true;
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
            if (!_inited) return;
            base.UnEquipPiece();
            _passives.ForEach(passive =>
            {
                if (passive is IDependSlotPassive dependSlotPassive)
                {
                    if (dependSlotPassive is not IDependSlotWeightModifier)
                        if (equipSlotIndex != dependSlotPassive.DependSlotPassiveInfo.dependSlotIndex)
                            return;
                }

                if (passive is IBuffPassive buffPassive)
                {
                    buffPassive.BuffPassiveInfo.ApplyBuffEvent = null;
                }

                if (passive is ICooldownPassive cooldownPassive)
                {
                    CooldownPassiveInfo cooldownPassiveInfo = cooldownPassive.CooldownPassiveInfo;
                    cooldownPassiveInfo.StartCooldownEvent = null;
                }


                passive.UnEquipPiece();
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
                    buffPassiveInfo.isBuffing = false;

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
                    cooldownPassiveInfo.isCooldowning = false;
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

            if (other._passives.Count > 0)
            {
                foreach (var passive in other._passives)
                {
                    if (!_passives.AsValueEnumerable().Any(x => x.pieceDisplayName == passive.pieceDisplayName))
                    {
                        _passives.Add(passive);
                    }
                }
            }

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

        public void EndAllCooldownPassive()
        {
            _passives.AsValueEnumerable().OfType<ICooldownPassive>().ToList()
                .ForEach(cooldownPassive =>
                {
                    cooldownPassive.CooldownPassiveInfo.remainingCooldownTime = 0;
                    cooldownPassive.CooldownPassiveInfo.isCooldowning = false;
                    if (cooldownPassive is ICooldownPassiveEndable cooldownPassiveEndable)
                    {
                        cooldownPassiveEndable.EndCooldown();
                    }
                });
        }

        public void EndAllBuffPassive()
        {
            _passives.AsValueEnumerable().OfType<IBuffPassive>().ToList()
                .ForEach(buffPassive =>
                {
                    buffPassive.BuffPassiveInfo.remainingBuffTime = 0;
                    buffPassive.BuffPassiveInfo.isBuffing = false;
                    buffPassive.EndBuff();
                });
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