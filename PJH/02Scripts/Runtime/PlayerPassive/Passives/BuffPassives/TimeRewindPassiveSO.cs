using System;
using Main.Runtime.Combat.Core;
using Main.Shared;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    public enum TimeRewindPassiveType
    {
        Heal,
        IgnoreDamage
    }

    [CreateAssetMenu(menuName = "SO/Passive/Persistent/TimeRewindPassive")]
    public class TimeRewindPassiveSO : PassiveSO, ICooldownPassive, IBuffPassive, ICooldownPassiveEndable
    {
        [SerializeField, EnumToggleButtons] private TimeRewindPassiveType _timeRewindType;

        [SerializeField, Range(0, 100f)] [SuffixLabel("%", true)]
        private float _returnValuePercent;

        [field: SerializeField, OdinSerialize] public CooldownPassiveInfo CooldownPassiveInfo { get; set; }
        [field: SerializeField, OdinSerialize] public BuffPassiveInfo BuffPassiveInfo { get; set; }

        private Player _player;
        private bool _isApplyingBuff;

        private bool _canApplyBuff = true;
        private float _cumulativeDamage;

        private string _effectName;

        private void OnEnable()
        {
            _effectName = $"TimeRewindEffect_{_timeRewindType}";
        }

        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
            _player = player as Player;
            (_player.HealthCompo as PlayerHealth).SetGetDamagedInfoBeforeApplyDamagedEvent +=
                HandleSetGetDamagedInfoBeforeApplyDamagedEvent;
        }

        private GetDamagedInfo? HandleSetGetDamagedInfoBeforeApplyDamagedEvent(GetDamagedInfo getDamagedInfo)
        {
            if (_canApplyBuff)
            {
                BuffPassiveInfo.ApplyBuffEvent?.Invoke();
            }

            if (!_isApplyingBuff) return getDamagedInfo;
            _cumulativeDamage += getDamagedInfo.damage;
            switch (_timeRewindType)
            {
                case TimeRewindPassiveType.IgnoreDamage:
                    getDamagedInfo.damage *= (1 - _returnValuePercent * 0.01f);
                    break;
            }

            return getDamagedInfo;
        }

        public override void UnEquipPiece()
        {
            base.UnEquipPiece();
            (_player.HealthCompo as PlayerHealth).SetGetDamagedInfoBeforeApplyDamagedEvent -=
                HandleSetGetDamagedInfoBeforeApplyDamagedEvent;
            _player.GetCompo<PlayerEffect>().StopMagioEffect(_effectName);
        }


        public void StartBuff()
        {
            _isApplyingBuff = true;
            _canApplyBuff = false;
            _player.GetCompo<PlayerEffect>().PlayMagioEffect(_effectName);
        }

        public void EndBuff()
        {
            _isApplyingBuff = false;

            if (_timeRewindType == TimeRewindPassiveType.Heal)
            {
                float healAmount = _cumulativeDamage * _returnValuePercent * 0.01f;
                _player.HealthCompo.ApplyHeal(healAmount);
            }

            _cumulativeDamage = 0;
            _player.GetCompo<PlayerEffect>().StopMagioEffect(_effectName);
            CooldownPassiveInfo.StartCooldownEvent?.Invoke();
        }

        public void EndCooldown()
        {
            _canApplyBuff = true;
        }
    }
}