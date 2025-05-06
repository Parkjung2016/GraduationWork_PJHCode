using UnityEngine.Rendering;

namespace Main.Runtime.Manager.VolumeTypes
{
    public class VignettingFadeVolumeType : VolumeType
    {
        public VignettingFadeVolumeType(Volume volume) : base(volume)
        {
            volume.profile.TryGet(out Beautify.Universal.Beautify beautify);
            if (beautify != null)
                SetFloatParameter(beautify.vignettingFade);
        }
    }
}