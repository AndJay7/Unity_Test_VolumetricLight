using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    [System.Serializable]
    public class ManualSpotLightData : ISpotLightData
    {
        [SerializeField, Range(0, 179)] 
        private float _outerAngle = 50;
        [SerializeField, Range(0, 179)]
        private float _innerAngle = 0;
        [SerializeField]
        private Color _color = Color.white;
        [SerializeField]
        private float _intensity = 1;
        [SerializeField]
        private float _range = 10;
        [SerializeField]
        private bool _isScalingAllowed = false;

        public bool IsScalingAllowed => _isScalingAllowed;  
        public float GetOuterAngle(Light light) => _outerAngle;
        public float GetInnerAngle(Light light) => Mathf.Min(_innerAngle, GetOuterAngle(light));
        public float GetRange(Light light) => _range;
        public Color GetColor(Light light) => _color;
        public float GetIntensity(Light light) => _intensity;
    }
}