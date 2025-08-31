using System;
using System.Collections.Generic;
using Animancer;
using Main.Core;
using Main.Runtime.Core.StatSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

[Flags]
public enum Ailment : int
{
    None = 0,
    Dot = 1 << 0,
    Flame = 1 << 1,
    Slow = 1 << 2
}

public delegate void AilmentChangedEvent(Ailment oldAilment, Ailment newAilment);

public delegate void AilmentDotDamageEvent(Ailment ailmentType, float damage, ITransition getDamagedAnimation);

[Serializable]
public class AilmentStat
{
    private Dictionary<Ailment, float> _ailmentTimerDictionary;
    private Dictionary<Ailment, float> _ailmentValueDictionary;

    public DotDamagedAnimationListSO getDamagedAnimationClip;
    public Ailment currentAilment;

    public event AilmentDotDamageEvent OnDotDamage;
    public event AilmentChangedEvent OnAilmentChanged;

    private Dictionary<Ailment, float> _dotTimers;
    private float _dotDamageCooldown = 1f;

    public AilmentStat()
    {
        _ailmentTimerDictionary = new Dictionary<Ailment, float>();
        _ailmentValueDictionary = new Dictionary<Ailment, float>();
        _dotTimers = new Dictionary<Ailment, float>();

        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment != Ailment.None)
            {
                _ailmentTimerDictionary.Add(ailment, 0f);
                _ailmentValueDictionary.Add(ailment, 0f);
                _dotTimers.Add(ailment, 0f);
            }
        }

        getDamagedAnimationClip =
            AddressableManager.Load<DotDamagedAnimationListSO>("DotDamageAnimList");
    }

    public void UpdateAilment()
    {
        DotDamageTimer();
        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment == Ailment.None) continue;

            if (_ailmentTimerDictionary[ailment] > 0)
            {
                _ailmentTimerDictionary[ailment] -= Time.deltaTime;
                if (_ailmentTimerDictionary[ailment] <= 0)
                {
                    Ailment oldAilment = currentAilment;
                    currentAilment ^= ailment; //XOR·Î »©ÁÖ°í
                    _dotTimers[ailment] = 0;
                    OnAilmentChanged?.Invoke(oldAilment, currentAilment);
                }
            }
        }
    }

    private void DotDamageTimer()
    {
        if ((currentAilment & (Ailment.Dot | Ailment.Flame)) == 0) return;

        foreach (Ailment type in Enum.GetValues(typeof(Ailment)))
        {
            if (type == Ailment.None) continue;

            if ((currentAilment & type) > 0)
            {
                _dotTimers[type] += Time.deltaTime;
                if (_ailmentTimerDictionary[type] > 0 && _dotTimers[type] > _dotDamageCooldown)
                {
                    _dotTimers[type] = 0;
                    OnDotDamage?.Invoke(Ailment.Dot, _ailmentValueDictionary[type],
                        getDamagedAnimationClip.getDamagedAnimations.GetRandom());
                }
            }
        }
    }

    public bool HasAilment(Ailment ailment)
    {
        return (currentAilment & ailment) > 0;
    }

    public float GetAilmentValue(Ailment ailment)
    {
        return _ailmentValueDictionary[ailment];
    }

    public void ApplyAilments(Ailment type, float duration, float value)
    {
        Ailment oldValue = currentAilment;
        currentAilment |= type;

        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if ((type & ailment) > 0)
            {
                SetAilment(ailment, duration: duration, value: value);
                break;
            }
        }

        if (oldValue != currentAilment)
        {
            OnAilmentChanged?.Invoke(oldValue, currentAilment);
        }
    }

    public void ClearAilment()
    {
        Ailment oldAilment = currentAilment;
        currentAilment = Ailment.None;
        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment == Ailment.None) continue;
            _dotTimers[ailment] = 0;
            _ailmentTimerDictionary[ailment] = 0;
            _ailmentValueDictionary[ailment] = 0;
        }

        OnAilmentChanged?.Invoke(oldAilment, currentAilment);
    }

    private void SetAilment(Ailment type, float duration, float value)
    {
        _ailmentTimerDictionary[type] = duration;
        _ailmentValueDictionary[type] = value;
    }
}