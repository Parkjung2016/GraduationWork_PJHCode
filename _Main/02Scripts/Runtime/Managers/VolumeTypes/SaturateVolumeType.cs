using UnityEngine;
using UnityEngine.Rendering;

namespace Main.Runtime.Manager.VolumeTypes
{
    public class SaturateVolumeType : VolumeType
    {
        public SaturateVolumeType(Volume volume) : base(volume)
        {
            volume.profile.TryGet(out Beautify.Universal.Beautify beautify);
            if (beautify != null)
                SetFloatParameter(beautify.saturate);
        }
    }
}