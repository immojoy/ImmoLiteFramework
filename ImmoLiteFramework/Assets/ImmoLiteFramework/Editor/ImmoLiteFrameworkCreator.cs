using UnityEditor;
using UnityEngine;
using Immojoy.LiteFramework.Runtime;

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
            
            // Create UI Manager child object
            GameObject uiManager = new GameObject("UI Manager");
            uiManager.AddComponent<ImmoUiManager>();
            Undo.RegisterCreatedObjectUndo(uiManager, "Create UI Manager");
            uiManager.transform.SetParent(frameworkRoot.transform, false);
            
            // Select the newly created framework root object
            Selection.activeObject = frameworkRoot;
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
            // Create UI Manager object
            GameObject uiManager = new GameObject("UI Manager");
            uiManager.AddComponent<ImmoUiManager>();
            
            // Register with the Undo system
            Undo.RegisterCreatedObjectUndo(uiManager, "Create UI Manager");
            
            // Set the parent object
            GameObjectUtility.SetParentAndAlign(uiManager, menuCommand.context as GameObject);
            
            // Select the newly created object
            Selection.activeObject = uiManager;
        }
    }
}
