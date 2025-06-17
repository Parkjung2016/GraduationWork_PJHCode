using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive
{
    public delegate void UpdateBuffTimeEventHandler(float remainingCooldownTime, float cooldownTime);

    public delegate void UpdateCooldownTimeEventHandler(float remainingCooldownTime, float cooldownTime);

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

        [HideInInspector] public UpdateCooldownTimeEventHandler OnUpdateCooldownTime;
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
        [HideInInspector] public UpdateBuffTimeEventHandler OnUpdateBuffTime;
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
}