using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Main.Runtime.Manager
{
    public enum VolumeEffectType
    {
        Blink,
        Brightness,
        ChromaticAberration,
        Frame,
        FinalBlur,
        RadialBlur,
        Saturate,
        Sepia,
        VignettingFade
    }

    public class VolumeManager
    {
        private Dictionary<Type, VolumeType> _volumeTypes;

        public Beautify.Universal.Beautify beautify;

        public void FindVolumeComponent()
        {
            _volumeTypes = new();
            Volume volume = Object.FindAnyObjectByType<Volume>();
            volume.profile.TryGet(out beautify);
            if (volume)
            {
                foreach (VolumeEffectType effectType in Enum.GetValues(typeof(VolumeEffectType)))
                {
                    Type type = Type.GetType($"Main.Runtime.Manager.VolumeTypes.{effectType}VolumeType");
                    if (type != null)
                    {
                        VolumeType volumeType =
                            Activator.CreateInstance(type, args: volume) as VolumeType;
                        _volumeTypes.Add(type, volumeType);
                    }
                }
            }
        }

        public T GetVolumeType<T>() where T : VolumeType
        {
            return (T)_volumeTypes[typeof(T)];
        }
    }
}