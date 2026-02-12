using UnityEditor;
using UnityEngine;
using Immojoy.LiteFramework.Runtime;

namespace Immojoy.LiteFramework.Editor
{
    /// <summary>
    /// Editor utility for creating Finite State Machine Manager in the Unity editor.
    /// </summary>
    public static class ImmoFsmCreator
    {
        [MenuItem("GameObject/Immo Lite Framework/Manager/Fsm Manager", false, 16)]
        private static void CreateFsmManager(MenuCommand menuCommand)
        {
            // Create Fsm Manager object
            GameObject fsmManager = new GameObject("Fsm Manager");
            fsmManager.AddComponent<ImmoFsmManager>();

            // Register with the Undo system
            Undo.RegisterCreatedObjectUndo(fsmManager, "Create Fsm Manager");

            // Set the parent object
            GameObjectUtility.SetParentAndAlign(fsmManager, menuCommand.context as GameObject);

            // Select the newly created object
            Selection.activeObject = fsmManager;
        }
    }
}
