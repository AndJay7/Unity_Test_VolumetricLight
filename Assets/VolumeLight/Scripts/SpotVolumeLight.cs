using UnityEngine;
using UnityEngine.Rendering;

namespace And.VisualEffects.VolumeLight
{
    [ExecuteInEditMode, SelectionBase]
    public class SpotVolumeLight : MonoBehaviour
    {
        private enum ControlMode
        {
            Manual = 0,
            Light = 1,
            Combined = 2,
        }

        private enum UpdateMode
        {
            None = 0,
            Always = 1,
            Manual = 2,
        }

        [SerializeField]
        private ControlMode _controlMode = ControlMode.Light;
        [SerializeField]
        private UpdateMode _updateMode = UpdateMode.Always;
        [Header("Parameters")]
        [SerializeField]
        private ManualSpotLightData _manualData = new ManualSpotLightData();

        [SerializeField]
        [HideInInspector]
        private Transform rendererTransform = null;
        [SerializeField]
        [HideInInspector]
        private new MeshRenderer renderer = null;

        private UpdateMode ActiveUpdateMode => Application.isPlaying ? _updateMode : UpdateMode.Always;

        private ComponentURPSpotLightData ComponentData
        {
            get
            {
                if (_componentData == null)
                    _componentData = new ComponentURPSpotLightData(GetComponent<Light>());
                return _componentData;
            }
        }

        private CombinedSpotLightData CombinedData
        {
            get
            {
                if (_combinedData == null)
                {
                    _combinedData = new CombinedSpotLightData(ComponentData, _manualData as ISpotLightData);
                }
                return _combinedData;
            }
        }

        private Material _material;
        private ComponentURPSpotLightData _componentData;
        private CombinedSpotLightData _combinedData;

        private const string SHADER_NAME = "Hidden/And/VolumeLight/Cone";
        private const string RESOURCE_MESH_NAME = "VolumeLight/Cone";
        private const string MESH_PROPERTY_OUTER_ANGLE_SQR_COS = "_MeshConeOuterAngleSqrCos";
        private const string MESH_PROPERTY_OUTER_ANGLE_TAN = "_MeshConeOuterAngleTan";
        private const string MESH_PROPERTY_INNER_ANGLE_SQR_COS = "_MeshConeInnerAngleSqrCos";
        private const string MESH_PROPERTY_HEIGHT = "_MeshConeHeight";
        private const string MESH_PROPERTY_POSITION = "_MeshConePosition";
        private const string MESH_PROPERTY_DIRECTION = "_MeshConeDirection";
        private const string MESH_PROPERTY_COLOR = "_LightColor";

        private void Start()
        {
            _componentData = new ComponentURPSpotLightData(GetComponent<Light>());
            _combinedData = new CombinedSpotLightData(_componentData, _manualData);

            CreateRenderer();
            InitializeRenderer();
            InitializeMaterial();

            ManualUpdate();
        }

        private void LateUpdate()
        {
            if (ActiveUpdateMode == UpdateMode.Always)
                ManualUpdate();
        }

        private void CreateRenderer()
        {
            if (rendererTransform != null)
                return;

            GameObject newObj = new GameObject();
            newObj.name = "LightVolumeRenderer";
            newObj.transform.SetParent(transform, false);
            newObj.layer = gameObject.layer;

            rendererTransform = newObj.transform;
            renderer = newObj.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = newObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Resources.Load<Mesh>(RESOURCE_MESH_NAME);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(newObj);
#endif
        }

        private void InitializeRenderer()
        {
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }

        private void InitializeMaterial()
        {
            Shader shader = Shader.Find(SHADER_NAME);
            _material = new Material(shader);
            _material.name = gameObject.name;
            renderer.material = _material;
        }

        private void UpdateRenderer(bool isScalingAllowed)
        {
            Vector3 rendererScale = Vector3.one;

            if (!isScalingAllowed)
            {
                Vector3 scale = transform.lossyScale;
                rendererScale.x = 1 / scale.x;
                rendererScale.y = 1 / scale.y;
                rendererScale.z = 1 / scale.z;
            }

            rendererTransform.localScale = rendererScale;
        }

        private void UpdateMaterial(ISpotLightData lightData)
        {
            if (_material != null)
            {
                float outerAngleCos = Mathf.Cos(lightData.OuterAngle * Mathf.Deg2Rad / 2);
                float outerAngleSqrCos = outerAngleCos * outerAngleCos;

                float innerAngleCos = Mathf.Cos(lightData.InnerAngle * Mathf.Deg2Rad / 2);
                float innerAngleSqrCos = innerAngleCos * innerAngleCos;

                float height = lightData.Range;

                float outerAngleTan = Mathf.Tan(lightData.OuterAngle * Mathf.Deg2Rad / 2);

                Color lightColor = lightData.Color * lightData.Intensity;

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
            switch (_controlMode)
            {
                case ControlMode.Manual:
                    return _manualData;
                case ControlMode.Light:
                    return ComponentData.IsValid ? ComponentData : _manualData;
                case ControlMode.Combined:
                    return CombinedData.IsValid ? CombinedData : _manualData;
            }
            return _manualData;
        }

        public void ManualUpdate()
        {
            ISpotLightData lightData = GetLightData();

            UpdateRenderer(lightData.IsScalingAllowed);
            if (renderer.isVisible)
                UpdateMaterial(lightData);
        }
    }
}