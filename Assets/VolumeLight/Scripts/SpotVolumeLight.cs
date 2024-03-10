using UnityEngine;
using UnityEngine.Rendering;

namespace And.VisualEffects.VolumeLight
{
    [ExecuteInEditMode, SelectionBase]
    public class SpotVolumeLight : MonoBehaviour
    {
        private enum UpdateMode
        {
            [HideInInspector]
            None = 0,
            Always = 1,
            Manual = 2,
        }

        [SerializeField]
        private UpdateMode _update = UpdateMode.Always;
        [SerializeField]
        private bool _manualSettings = false;

        [SerializeField]
        private ComponentURPSpotLightData _componentData = new ComponentURPSpotLightData();
        [SerializeField]
        private ManualSpotLightData _manualData = new ManualSpotLightData();

        private Transform _rendererTransform = null;
        private MeshRenderer _renderer = null;
        private Light _light = null;
        private Material _material = null;

        private UpdateMode ActiveUpdateMode => Application.isPlaying ? _update : UpdateMode.Always;

        private const string SHADER_NAME = "Hidden/And/VolumeLight/Cone";
        private const string RESOURCE_MESH_NAME = "VolumeLight/Cone";
        private const string MESH_PROPERTY_OUTER_ANGLE_SQR_COS = "_MeshConeOuterAngleSqrCos";
        private const string MESH_PROPERTY_OUTER_ANGLE_TAN = "_MeshConeOuterAngleTan";
        private const string MESH_PROPERTY_INNER_ANGLE_SQR_COS = "_MeshConeInnerAngleSqrCos";
        private const string MESH_PROPERTY_HEIGHT = "_MeshConeHeight";
        private const string MESH_PROPERTY_POSITION = "_MeshConePosition";
        private const string MESH_PROPERTY_DIRECTION = "_MeshConeDirection";
        private const string MESH_PROPERTY_COLOR = "_LightColor";
        private const float LIGHT_INTENSITY_NORMALIZE = 0.001f;

        private void Awake()
        {
            CreateRenderer();
            InitializeRenderer();
            InitializeMaterial();

            Refresh();
            ManualUpdate();
        }

        private void OnEnable()
        {
            _rendererTransform.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _rendererTransform.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
                Destroy(_rendererTransform.gameObject);
            else
                DestroyImmediate(_rendererTransform.gameObject);
        }

        private void LateUpdate()
        {
            if (ActiveUpdateMode == UpdateMode.Always)
                ManualUpdate();
        }

        private void CreateRenderer()
        {
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(transform, false);
            newObj.layer = gameObject.layer;
            newObj.hideFlags = HideFlags.HideAndDontSave;

            _rendererTransform = newObj.transform;
            _renderer = newObj.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = newObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Resources.Load<Mesh>(RESOURCE_MESH_NAME);
        }

        private void InitializeRenderer()
        {
            _renderer.shadowCastingMode = ShadowCastingMode.Off;
            _renderer.lightProbeUsage = LightProbeUsage.Off;
            _renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }

        private void InitializeMaterial()
        {
            Shader shader = Shader.Find(SHADER_NAME);
            _material = new Material(shader);
            _material.name = gameObject.name;
            _renderer.material = _material;
        }

        private void UpdateRenderer(ISpotLightData lightData)
        {
            Vector3 rendererScale = Vector3.one;

            if (!lightData.IsScalingAllowed)
            {
                Vector3 scale = transform.lossyScale;
                rendererScale.x = 1 / scale.x;
                rendererScale.y = 1 / scale.y;
                rendererScale.z = 1 / scale.z;
            }

            _rendererTransform.localScale = rendererScale;
            UpdateRendererBounds(lightData);
        }

        private void UpdateRendererBounds(ISpotLightData lightData)
        {
            float coneRange = lightData.GetRange(_light);
            float coneOuterAngle = lightData.GetOuterAngle(_light) * Mathf.Deg2Rad / 2;
            Vector3 pointA = _rendererTransform.position;
            float boxRangeA = 0;
            Vector3 pointB = _rendererTransform.position + _rendererTransform.forward * coneRange;
            float boxRangeB = coneRange * Mathf.Sin(coneOuterAngle);

            Vector3 a = pointB - pointA;
            Vector3 sqrA = Vector3.Scale(a, a);
            float dotA = Vector3.Dot(a, a);
            Vector3 e = Vector3.one - sqrA / dotA;
            e.x = Mathf.Sqrt(e.x);
            e.y = Mathf.Sqrt(e.y);
            e.z = Mathf.Sqrt(e.z);

            Vector3 min = Vector3.Min(pointA - e * boxRangeA, pointB - e * boxRangeB);
            Vector3 max = Vector3.Max(pointA + e * boxRangeA, pointB + e * boxRangeB);

            Bounds bounds = new Bounds();
            bounds.min = min;
            bounds.max = max;
            _renderer.bounds = bounds;
        }

        private void UpdateMaterial(ISpotLightData lightData)
        {
            if (_material != null)
            {
                float outerAngle = lightData.GetOuterAngle(_light) * Mathf.Deg2Rad / 2;
                float outerAngleTan = Mathf.Tan(outerAngle);
                float outerAngleCos = Mathf.Cos(outerAngle);
                float outerAngleSqrCos = outerAngleCos * outerAngleCos;

                float innerAngleCos = Mathf.Cos(lightData.GetInnerAngle(_light) * Mathf.Deg2Rad / 2);
                float innerAngleSqrCos = innerAngleCos * innerAngleCos;

                float height = lightData.GetRange(_light);


                Color lightColor = lightData.GetColor(_light) * lightData.GetIntensity(_light) * LIGHT_INTENSITY_NORMALIZE;

                _material.SetVector(MESH_PROPERTY_POSITION, Vector3.zero);
                _material.SetVector(MESH_PROPERTY_DIRECTION, Vector3.forward);
                _material.SetFloat(MESH_PROPERTY_OUTER_ANGLE_SQR_COS, outerAngleSqrCos);
                _material.SetFloat(MESH_PROPERTY_INNER_ANGLE_SQR_COS, innerAngleSqrCos);
                _material.SetFloat(MESH_PROPERTY_HEIGHT, height);
                _material.SetFloat(MESH_PROPERTY_OUTER_ANGLE_TAN, outerAngleTan);
                _material.SetVector(MESH_PROPERTY_COLOR, lightColor);
            }
        }

        private ISpotLightData GetLightData()
        {
            if (_manualSettings || _light == null)
                return _manualData;
            else
                return _componentData;
        }

        public void ManualUpdate()
        {
            ISpotLightData lightData = GetLightData();

            UpdateRenderer(lightData);
            if (_renderer.isVisible)
                UpdateMaterial(lightData);
        }

        public void Refresh()
        {
            _light = GetComponent<Light>();
        }
    }
}