using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    public interface ISpotLightData
    {
        bool IsScalingAllowed { get; }
        float GetOuterAngle(Light light);
        float GetInnerAngle(Light light);
        float GetRange(Light light);
        Color GetColor(Light light);
        float GetIntensity(Light light);
    }
}