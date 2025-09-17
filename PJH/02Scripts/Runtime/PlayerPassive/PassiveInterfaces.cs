using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive
{
    public delegate void UpdatePassiveTimeEventHandler(float remainingTime, float time);


    public interface IActivePassive
    {
        public void ActivePassive();
        public void DeActivePassive();
    }


    public class CooldownPassiveInfo
    {
        [LabelText("⏱️ 재사용 시간(초)")] [SuffixLabel("sec", true)]
        public float cooldownTime;

        [ReadOnly] public float remainingCooldownTime;
        [ReadOnly] public bool isCooldowning;

        [HideInInspector] public UpdatePassiveTimeEventHandler OnUpdateCooldownTime;
        [HideInInspector] public Action StartCooldownEvent;
    }

    public interface ICooldownPassive
    {
        public CooldownPassiveInfo CooldownPassiveInfo { get; set; }
    }

    public interface ICooldownPassiveEndable
    {
        public void EndCooldown();
    }

    public class BuffPassiveInfo
    {
        [LabelText("⏱️ 버프 지속시간(초)")] [SuffixLabel("sec", true)]
        public float buffDuration;

        [ReadOnly] public float remainingBuffTime;
        [ReadOnly] public bool isBuffing;
        
        [HideInInspector] public UpdatePassiveTimeEventHandler OnUpdateBuffTime;
        [HideInInspector] public Action ApplyBuffEvent;
    }

    public interface IBuffPassive
    {
        public BuffPassiveInfo BuffPassiveInfo { get; set; }
        public void StartBuff();
        public void EndBuff();
    }

    public interface IBuffPassiveUpdateable
    {
        public void UpdateBuff();
    }

    public class DependPassiveInfo
    {
        [LabelText("🔗 종속 슬롯 인덱스(0~2)")] public int dependSlotIndex;
    }

    public interface IDependSlotPassive
    {
        public DependPassiveInfo DependSlotPassiveInfo { get; set; }
    }

    public interface IDependSlotWeightModifier
    {
        public void ChangePassiveValueToWeightModifier();
        public void ChangePassiveValueToOrigin();
    }
}