using UnityEngine.Rendering;

namespace Main.Runtime.Manager.VolumeTypes
{
    public class FrameVolumeType : VolumeType
    {
        private Beautify.Universal.Beautify _beautify;

        public FrameVolumeType(Volume volume) : base(volume)
        {
            volume.profile.TryGet(out _beautify);
            _beautify.frameBandVerticalSize.Override(0);
        }

        public void ChangeToBorder()
        {
            if (!_beautify) return;
            _beautify.frameStyle.Override(Beautify.Universal.Beautify.FrameStyle.Border);
            _beautify.frameMask.overrideState = true;
            SetFloatParameter(null);
        }

        public void ChangeToCinematicBands()
        {
            if (!_beautify) return;
            _beautify.frameStyle.Override(Beautify.Universal.Beautify.FrameStyle.CinematicBands);
            _beautify.frameMask.overrideState = false;
            SetFloatParameter(_beautify.frameBandVerticalSize);
        }
    }
}