using System.Collections.Generic;
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Scene manager usage example demonstrating the transition config API.
    /// </summary>
    [AddComponentMenu("Immojoy/Lite Framework/Scene/Immo Scene Example")]
    public class ImmoSceneExample : MonoBehaviour
    {
        [Header("Scene Settings")]
        [Tooltip("Addressable scene address to load")]
        public string m_SceneToLoad = "SampleScene";

        [Tooltip("Addressable scene address to unload")]
        public string m_SceneToUnload = "";


        public async void LoadScene()
        {
            if (string.IsNullOrEmpty(m_SceneToLoad))
            {
                Debug.LogWarning("Scene address is not set.");
                return;
            }

            var config = new ImmoSceneTransitionConfig
            {
                ScenesToLoad = new List<string> { m_SceneToLoad },
                ScenesToUnload = string.IsNullOrEmpty(m_SceneToUnload)
                    ? new List<string>()
                    : new List<string> { m_SceneToUnload },
            };

            await ImmoFramework.Instance.SceneManager.LoadSceneAsync(config);
        }


        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 200));

            GUILayout.Label("=== ImmoSceneManager Example ===");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Load:", GUILayout.Width(60));
            m_SceneToLoad = GUILayout.TextField(m_SceneToLoad, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Unload:", GUILayout.Width(60));
            m_SceneToUnload = GUILayout.TextField(m_SceneToUnload, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Transition", GUILayout.Width(150)))
            {
                LoadScene();
            }

            GUILayout.EndArea();
        }
    }
}
