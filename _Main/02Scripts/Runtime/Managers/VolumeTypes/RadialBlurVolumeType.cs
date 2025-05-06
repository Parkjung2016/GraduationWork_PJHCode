using OccaSoftware.RadialBlur.Runtime;
using UnityEngine.Rendering;

namespace Main.Runtime.Manager.VolumeTypes
{
    public class RadialBlurVolumeType : VolumeType
    {
        public RadialBlurVolumeType(Volume volume) : base(volume)
        {
            volume.profile.TryGet(out RadialBlurPostProcess radialBlur);
            if (radialBlur != null)
                SetFloatParameter(radialBlur.intensity);
        }
    }
}