using UnityEditor;
using UnityEngine;
using Immojoy.LiteFramework.Runtime;
using UnityEngine.UI;

namespace Immojoy.LiteFramework.Editor
{
    public static class ImmoLiteFrameworkCreator
    {
        [MenuItem("GameObject/Immo Lite Framework/Framework", false, 10)]
        private static void CreateFramework(MenuCommand menuCommand)
        {
            // Create the root object
            GameObject frameworkRoot = new GameObject("Immo Lite Framework");

            // Register with the Undo system to support undo operations
            Undo.RegisterCreatedObjectUndo(frameworkRoot, "Create Immo Lite Framework");

            // Set the parent object (if an object is selected in the Hierarchy, set it as the parent)
            GameObjectUtility.SetParentAndAlign(frameworkRoot, menuCommand.context as GameObject);

            // Create Event Manager child object
            GameObject eventManager = new GameObject("Event Manager");
            eventManager.AddComponent<ImmoEventManager>();
            Undo.RegisterCreatedObjectUndo(eventManager, "Create Event Manager");
            eventManager.transform.SetParent(frameworkRoot.transform, false);

            // Create Resource Manager child object
            GameObject resourceManager = new GameObject("Resource Manager");
            resourceManager.AddComponent<ImmoResourceManager>();
            Undo.RegisterCreatedObjectUndo(resourceManager, "Create Resource Manager");
            resourceManager.transform.SetParent(frameworkRoot.transform, false);

            // Create UI Manager child object with layers
            GameObject uiManager = CreateUIManagerWithLayers();
            uiManager.transform.SetParent(frameworkRoot.transform, false);

            // Create Scene Manager child object
            GameObject sceneManager = new GameObject("Scene Manager");
            sceneManager.AddComponent<ImmoSceneManager>();
            Undo.RegisterCreatedObjectUndo(sceneManager, "Create Scene Manager");
            sceneManager.transform.SetParent(frameworkRoot.transform, false);

            // Create Procedure Manager child object
            GameObject procedureManager = new GameObject("Procedure Manager");
            procedureManager.AddComponent<ImmoProcedureManager>();
            Undo.RegisterCreatedObjectUndo(procedureManager, "Create Procedure Manager");
            procedureManager.transform.SetParent(frameworkRoot.transform, false);

            // Create Fsm Manager child object
            GameObject fsmManager = new GameObject("Fsm Manager");
            fsmManager.AddComponent<ImmoFsmManager>();
            Undo.RegisterCreatedObjectUndo(fsmManager, "Create Fsm Manager");
            fsmManager.transform.SetParent(frameworkRoot.transform, false);

            // Select the newly created framework root object
            Selection.activeObject = frameworkRoot;
        }


        private static GameObject CreateUIManagerWithLayers()
        {
            GameObject uiManager = new GameObject("UI Manager");
            uiManager.AddComponent<ImmoUiManager>();
            Undo.RegisterCreatedObjectUndo(uiManager, "Create UI Manager");

            // Create UI Canvas as a separate root (not under UI Manager)
            GameObject canvasObject = new GameObject("UI Canvas");
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create UI Canvas");

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;
            
            canvasObject.AddComponent<GraphicRaycaster>();

            // Create layer objects with Canvas components for sort order control
            CreateUILayer(canvasObject.transform, "UI Background", 0);
            CreateUILayer(canvasObject.transform, "UI Normal", 100);
            CreateUILayer(canvasObject.transform, "UI Popup", 200);
            CreateUILayer(canvasObject.transform, "UI Top", 300);
            CreateUILayer(canvasObject.transform, "UI System", 400);

            return uiManager;
        }
        
        
        private static void CreateUILayer(Transform parent, string layerName, int sortOrder)
        {
            GameObject layer = new GameObject(layerName);
            layer.transform.SetParent(parent, false);
            
            // Add RectTransform for proper UI layout
            RectTransform rectTransform = layer.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Add Canvas for independent sort order
            Canvas canvas = layer.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortOrder;
            
            // Add GraphicRaycaster for UI interaction
            layer.AddComponent<GraphicRaycaster>();
            
            Undo.RegisterCreatedObjectUndo(layer, $"Create {layerName}");
        }


        [MenuItem("GameObject/Immo Lite Framework/Manager/Event Manager", false, 11)]
        private static void CreateEventManager(MenuCommand menuCommand)
        {
            // Create Event Manager object
            GameObject eventManager = new GameObject("Event Manager");
            eventManager.AddComponent<ImmoEventManager>();

            // Register with the Undo system
            Undo.RegisterCreatedObjectUndo(eventManager, "Create Event Manager");

            // Set the parent object
            GameObjectUtility.SetParentAndAlign(eventManager, menuCommand.context as GameObject);

            // Select the newly created object
            Selection.activeObject = eventManager;
        }


        [MenuItem("GameObject/Immo Lite Framework/Manager/Resource Manager", false, 12)]
        private static void CreateResourceManager(MenuCommand menuCommand)
        {
            // Create Resource Manager object
            GameObject resourceManager = new GameObject("Resource Manager");
            resourceManager.AddComponent<ImmoResourceManager>();

            // Register with the Undo system
            Undo.RegisterCreatedObjectUndo(resourceManager, "Create Resource Manager");

            // Set the parent object
            GameObjectUtility.SetParentAndAlign(resourceManager, menuCommand.context as GameObject);

            // Select the newly created object
            Selection.activeObject = resourceManager;
        }
        
        
        [MenuItem("GameObject/Immo Lite Framework/Manager/UI Manager", false, 13)]
        private static void CreateUIManager(MenuCommand menuCommand)
        {
            // Create UI Manager object with complete layer structure
            GameObject uiManager = CreateUIManagerWithLayers();
            
            // Set the parent object
            GameObjectUtility.SetParentAndAlign(uiManager, menuCommand.context as GameObject);
            
            // Also parent the UI Canvas if created
            GameObject uiCanvas = GameObject.Find("UI Canvas");
            if (uiCanvas != null)
            {
                GameObjectUtility.SetParentAndAlign(uiCanvas, menuCommand.context as GameObject);
            }
            
            // Select the newly created object
            Selection.activeObject = uiManager;
        }
    }
}
