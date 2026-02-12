using UnityEditor;
using UnityEngine;
using Immojoy.LiteFramework.Runtime;

namespace Immojoy.LiteFramework.Editor
{
    /// <summary>
    /// Editor utility for creating Procedure Manager in the Unity editor.
    /// </summary>
    public static class ImmoProcedureCreator
    {
        [MenuItem("GameObject/Immo Lite Framework/Manager/Procedure Manager", false, 15)]
        private static void CreateProcedureManager(MenuCommand menuCommand)
        {
            // Create Procedure Manager object
            GameObject procedureManager = new GameObject("Procedure Manager");
            procedureManager.AddComponent<ImmoProcedureManager>();

            // Register with the Undo system
            Undo.RegisterCreatedObjectUndo(procedureManager, "Create Procedure Manager");

            // Set the parent object
            GameObjectUtility.SetParentAndAlign(procedureManager, menuCommand.context as GameObject);

            // Select the newly created object
            Selection.activeObject = procedureManager;
        }
    }
}
