using UnityEngine.Rendering;

namespace Main.Runtime.Manager.VolumeTypes
{
    public class SepiaVolumeType : VolumeType
    {
        public SepiaVolumeType(Volume volume) : base(volume)
        {
            volume.profile.TryGet(out Beautify.Universal.Beautify beautify);
            if (beautify != null)

            SetFloatParameter(beautify.sepia);
        }
    }
}