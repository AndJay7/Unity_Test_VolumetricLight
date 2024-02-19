using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    public interface ISpotLightData
    {
        bool IsValid { get; }
        bool IsScalingAllowed { get; }
        float OuterAngle { get; }
        float InnerAngle { get; }        
        float Range { get; }
        Color Color { get; }
        float Intensity { get; }
    }
}