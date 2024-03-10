using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace And.VisualEffects.VolumeLight.Editor
{
    public class SpotLightVolumeContextMenu
    {
        [MenuItem("GameObject/Light/Spotlight with Volume", priority = 0)]
        private static void CreateSpotLight(MenuCommand command)
        {
            GameObject obj = new GameObject("Spot Light");
            GameObject context = command.context as GameObject;

            if (context != null)
                GameObjectUtility.SetParentAndAlign(obj, context);
            else
            {
                obj.transform.Rotate(90, 0, 0);
                var view = SceneView.lastActiveSceneView;
                if (view != null)
                {
                    obj.transform.position = view.camera.transform.position + view.camera.transform.forward*20;
                }
            }

            Light light = obj.AddComponent<Light>();
            light.type = LightType.Spot;

            obj.AddComponent<SpotVolumeLight>();

            Undo.RegisterCreatedObjectUndo(obj,"Create " + obj.name);
            Selection.activeObject = obj;
        }
    }
}