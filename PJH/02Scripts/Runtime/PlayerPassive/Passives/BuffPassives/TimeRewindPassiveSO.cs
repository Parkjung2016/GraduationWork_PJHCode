using DamageNumbersPro;
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
    public class TimeRewindPassiveSO : PassiveSO, ICooldownPassive, IBuffPassive, ICooldownPassiveEndable,
        IDependSlotPassive, IDependSlotWeightModifier
    {
        [SerializeField, EnumToggleButtons] private TimeRewindPassiveType _timeRewindType;

        [SerializeField, ShowIf("_timeRewindType", TimeRewindPassiveType.Heal)]
        private DamageNumber _healDamageNumber;

        [SerializeField, ShowIf("_timeRewindType", TimeRewindPassiveType.IgnoreDamage)]
        private DamageNumber _ignoreDamageNumber;

        [SerializeField, Range(0, 100f)] [SuffixLabel("%", true)]
        private float _returnValuePercent;

        [field: SerializeField, OdinSerialize] public CooldownPassiveInfo CooldownPassiveInfo { get; set; }
        [field: SerializeField, OdinSerialize] public BuffPassiveInfo BuffPassiveInfo { get; set; }
        [field: SerializeField, OdinSerialize] public DependPassiveInfo DependSlotPassiveInfo { get; set; }

        private bool _isApplyingBuff;

        private bool _canApplyBuff = true;
        private float _cumulativeDamage;
        private float _originReturnValuePercent;
        private string _effectName;

        private void OnEnable()
        {
            _originReturnValuePercent = _returnValuePercent;
            _effectName = $"TimeRewindEffect_{_timeRewindType}";
        }

        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
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
            switch (_timeRewindType)
            {
                case TimeRewindPassiveType.IgnoreDamage:
                    float appliedIgnoreDamageAmount =
                        GetValueAppliedToReturnValuePercent(getDamagedInfo.damage);
                    _ignoreDamageNumber.Spawn(_player.transform.position, appliedIgnoreDamageAmount);
                    getDamagedInfo.damage = appliedIgnoreDamageAmount;
                    break;
                case TimeRewindPassiveType.Heal:
                    _cumulativeDamage += getDamagedInfo.damage;
                    float appliedHealAmount =
                        GetValueAppliedToReturnValuePercent(_cumulativeDamage);
                    _healDamageNumber.Spawn(_player.transform.position, appliedHealAmount);
                    break;
            }

            return getDamagedInfo;
        }

        public override void UnEquipPiece()
        {
            (_player.HealthCompo as PlayerHealth).SetGetDamagedInfoBeforeApplyDamagedEvent -=
                HandleSetGetDamagedInfoBeforeApplyDamagedEvent;
            base.UnEquipPiece();
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
                float healAmount = GetValueAppliedToReturnValuePercent(_cumulativeDamage);
                _player.HealthCompo.ApplyHeal(healAmount);
                _cumulativeDamage = 0;
            }

            _player.GetCompo<PlayerEffect>().StopMagioEffect(_effectName);
            CooldownPassiveInfo.StartCooldownEvent?.Invoke();
        }

        private float GetValueAppliedToReturnValuePercent(float value)
        {
            return value * (1 - _returnValuePercent * 0.01f);
        }

        public void EndCooldown()
        {
            _canApplyBuff = true;
        }

        public void ChangePassiveValueToWeightModifier()
        {
            _returnValuePercent *= .5f;
        }

        public void ChangePassiveValueToOrigin()
        {
            _returnValuePercent = _originReturnValuePercent;
        }
    }
}