using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace And.VisualEffects.VolumeLight.Editor
{
    [CustomEditor(typeof(SpotVolumeLight))]
    public class SpotVolumeLightEditor : UnityEditor.Editor
    {
        SerializedProperty _update;
        SerializedProperty _manualSettings;
        SerializedProperty _manualData;
        SerializedProperty _componentData;

        private void OnEnable()
        {
            _update = serializedObject.FindProperty(nameof(_update));
            _manualSettings = serializedObject.FindProperty(nameof(_manualSettings));
            _manualData = serializedObject.FindProperty(nameof(_manualData));
            _componentData = serializedObject.FindProperty(nameof(_componentData));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_update);
            bool isLight = serializedObject.targetObject.GetComponent<Light>() != null;

            if (isLight)
                EditorGUILayout.PropertyField(_manualSettings);

            GUIContent label = new GUIContent("Properties");

            if (!isLight || _manualSettings.boolValue)
                EditorGUILayout.PropertyField(_manualData, label);
            else
                EditorGUILayout.PropertyField(_componentData, label);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
