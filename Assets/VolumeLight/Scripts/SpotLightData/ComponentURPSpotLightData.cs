using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    public class ComponentURPSpotLightData : ISpotLightData
    {
        public bool IsValid => lightComponent != null;
        public bool IsScalingAllowed => false;
        public float OuterAngle => lightComponent.spotAngle;
        public float InnerAngle => lightComponent.innerSpotAngle;
        public float Range => lightComponent.range;
        public Color Color => lightComponent.color;
        public float Intensity => lightComponent.intensity;

        private Light lightComponent = null;

        public ComponentURPSpotLightData(Light light)
        {
            lightComponent = light;
        }
    }
}