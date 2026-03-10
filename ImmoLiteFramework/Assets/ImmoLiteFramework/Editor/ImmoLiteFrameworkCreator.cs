using UnityEditor;
using UnityEngine;
using Immojoy.LiteFramework.Runtime;
using UnityEngine.UI;

namespace Immojoy.LiteFramework.Editor
{
    public static class ImmoLiteFrameworkCreator
    {
        [MenuItem("GameObject/Immojoy/Lite Framework", false, 10)]
        private static void CreateFramework(MenuCommand menuCommand)
        {
            // Create the root object with the ImmoFramework component.
            // ImmoFramework will automatically create and initialize all managers at runtime.
            GameObject frameworkRoot = new GameObject("Immo Lite Framework");
            frameworkRoot.AddComponent<ImmoFramework>();

            Undo.RegisterCreatedObjectUndo(frameworkRoot, "Create Immo Lite Framework");
            GameObjectUtility.SetParentAndAlign(frameworkRoot, menuCommand.context as GameObject);

            // Create the UI Canvas structure required by ImmoUiManager
            CreateUICanvas(frameworkRoot);

            Selection.activeObject = frameworkRoot;
        }


        private static void CreateUICanvas(GameObject frameworkRoot)
        {
            GameObject canvasObject = new GameObject("UI Canvas");
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create UI Canvas");
            canvasObject.transform.SetParent(frameworkRoot.transform, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            CreateUILayer(canvasObject.transform, "UI Background", 0);
            CreateUILayer(canvasObject.transform, "UI Normal", 100);
            CreateUILayer(canvasObject.transform, "UI Popup", 200);
            CreateUILayer(canvasObject.transform, "UI Top", 300);
            CreateUILayer(canvasObject.transform, "UI System", 400);
        }


        private static void CreateUILayer(Transform parent, string layerName, int sortOrder)
        {
            GameObject layer = new GameObject(layerName);
            layer.transform.SetParent(parent, false);

            RectTransform rectTransform = layer.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            Canvas canvas = layer.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortOrder;

            layer.AddComponent<GraphicRaycaster>();

            Undo.RegisterCreatedObjectUndo(layer, $"Create {layerName}");
        }
    }
}
