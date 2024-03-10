using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    [System.Serializable]
    public class ComponentURPSpotLightData : ISpotLightData
    {
        [SerializeField]
        private float _outerAngleMultiply = 1;
        [SerializeField]
        private float _innerAngleMultiply = 1;
        [SerializeField]
        private Color _colorMultiply = Color.white;
        [SerializeField]
        private float _intensityMultiply = 1;
        [SerializeField]
        private float _rangeMultiply = 1;

        public bool IsScalingAllowed => false;
        public float GetOuterAngle(Light light) => Mathf.Clamp(_outerAngleMultiply * light.spotAngle,0, 179);
        public float GetInnerAngle(Light light)
        {
            float angle = _innerAngleMultiply * light.innerSpotAngle;
            angle = Mathf.Clamp(angle, 0, 179);
            angle = Mathf.Min(angle, GetOuterAngle(light));
            return angle;
        }

        public float GetRange(Light light) => _rangeMultiply * light.range;
        public Color GetColor(Light light) => _colorMultiply * light.color;
        public float GetIntensity(Light light) => _intensityMultiply * light.intensity;
    }
}