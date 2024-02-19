using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    public class CombinedSpotLightData : ISpotLightData
    {
        public bool IsValid => _firstData.IsValid && _secondData.IsValid;
        public bool IsScalingAllowed => _firstData.IsScalingAllowed && _secondData.IsScalingAllowed;
        public float OuterAngle => (_firstData.OuterAngle + _secondData.OuterAngle)/2;
        public float InnerAngle => (_firstData.InnerAngle + _secondData.InnerAngle) / 2;
        public float Range => (_firstData.Range + _secondData.Range) / 2;
        public Color Color => (_firstData.Color + _secondData.Color) / 2;
        public float Intensity => (_firstData.Intensity + _secondData.Intensity) / 2;

        private ISpotLightData _firstData = null;
        private ISpotLightData _secondData = null;

        public CombinedSpotLightData(in ISpotLightData firstData, in ISpotLightData secondData)
        {
            _firstData = firstData;
            _secondData = secondData;
        }
    }
}