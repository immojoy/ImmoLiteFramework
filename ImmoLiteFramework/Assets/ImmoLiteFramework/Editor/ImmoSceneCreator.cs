using UnityEditor;
using UnityEngine;
using Immojoy.LiteFramework.Runtime;

namespace Immojoy.LiteFramework.Editor
{
    /// <summary>
    /// Editor utility for creating Scene Manager in the Unity editor.
    /// </summary>
    public static class ImmoSceneCreator
    {
        [MenuItem("GameObject/Immo Lite Framework/Manager/Scene Manager", false, 14)]
        private static void CreateSceneManager(MenuCommand menuCommand)
        {
            // Create Scene Manager object
            GameObject sceneManager = new GameObject("Scene Manager");
            sceneManager.AddComponent<ImmoSceneManager>();

            // Register with the Undo system
            Undo.RegisterCreatedObjectUndo(sceneManager, "Create Scene Manager");

            // Set the parent object
            GameObjectUtility.SetParentAndAlign(sceneManager, menuCommand.context as GameObject);

            // Select the newly created object
            Selection.activeObject = sceneManager;
        }
    }
}
