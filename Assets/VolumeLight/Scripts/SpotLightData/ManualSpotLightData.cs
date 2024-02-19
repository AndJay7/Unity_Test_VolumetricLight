using UnityEngine;

namespace And.VisualEffects.VolumeLight
{
    [System.Serializable]
    public class ManualSpotLightData : ISpotLightData
    {
        [SerializeField, Range(0,180)] 
        private float _outerAngle = 30;
        [SerializeField, Range(0, 180)]
        private float _innerAngle = 0;
        [SerializeField]
        private Color _color = Color.white;
        [SerializeField]
        private float _intensity = 1;
        [SerializeField]
        private float _range = 10;
        [SerializeField]
        private bool _isScalingAllowed = false;

        public bool IsValid => true;
        public bool IsScalingAllowed => _isScalingAllowed;  
        public float OuterAngle => _outerAngle;
        public float InnerAngle => Mathf.Min(_innerAngle,OuterAngle);
        public float Range => _range;
        public Color Color => _color;
        public float Intensity => _intensity;
    }
}