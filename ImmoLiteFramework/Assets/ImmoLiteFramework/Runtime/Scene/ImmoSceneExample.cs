using UnityEngine;
using UnityEngine.SceneManagement;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Scene manager usage example.
    /// </summary>
    [AddComponentMenu("Immojoy/Lite Framework/Scene/Immo Scene Example")]
    public class ImmoSceneExample : MonoBehaviour
    {
        [Header("Scene Settings")]
        [Tooltip("Scene name to load")]
        public string m_SceneToLoad = "SampleScene";

        [Tooltip("Load mode")]
        public LoadSceneMode m_LoadMode = LoadSceneMode.Single;

        [Header("Test Features")]
        [Tooltip("Auto load scene on start")]
        public bool m_AutoLoadOnStart = false;


        private void Start()
        {
            if (ImmoSceneManager.Instance == null)
            {
                Debug.LogError("ImmoSceneManager is not found in the scene.");
                return;
            }

            // Subscribe to scene events
            ImmoSceneManager.Instance.LoadSceneSuccess += OnLoadSceneSuccess;
            ImmoSceneManager.Instance.LoadSceneFailure += OnLoadSceneFailure;
            ImmoSceneManager.Instance.LoadSceneUpdate += OnLoadSceneUpdate;
            ImmoSceneManager.Instance.UnloadSceneSuccess += OnUnloadSceneSuccess;
            ImmoSceneManager.Instance.UnloadSceneFailure += OnUnloadSceneFailure;

            if (m_AutoLoadOnStart && !string.IsNullOrEmpty(m_SceneToLoad))
            {
                LoadScene();
            }
        }


        private void OnDestroy()
        {
            if (ImmoSceneManager.Instance != null)
            {
                // Unsubscribe from scene events
                ImmoSceneManager.Instance.LoadSceneSuccess -= OnLoadSceneSuccess;
                ImmoSceneManager.Instance.LoadSceneFailure -= OnLoadSceneFailure;
                ImmoSceneManager.Instance.LoadSceneUpdate -= OnLoadSceneUpdate;
                ImmoSceneManager.Instance.UnloadSceneSuccess -= OnUnloadSceneSuccess;
                ImmoSceneManager.Instance.UnloadSceneFailure -= OnUnloadSceneFailure;
            }
        }


        /// <summary>
        /// Load scene.
        /// </summary>
        public void LoadScene()
        {
            if (string.IsNullOrEmpty(m_SceneToLoad))
            {
                Debug.LogWarning("Scene name is not set.");
                return;
            }

            Debug.Log($"Loading scene: {m_SceneToLoad}");
            ImmoSceneManager.Instance.LoadSceneAsync(m_SceneToLoad, m_LoadMode);
        }


        /// <summary>
        /// Unload scene.
        /// </summary>
        public void UnloadScene()
        {
            if (string.IsNullOrEmpty(m_SceneToLoad))
            {
                Debug.LogWarning("Scene name is not set.");
                return;
            }

            Debug.Log($"Unloading scene: {m_SceneToLoad}");
            ImmoSceneManager.Instance.UnloadSceneAsync(m_SceneToLoad);
        }


        /// <summary>
        /// Scene load success callback.
        /// </summary>
        private void OnLoadSceneSuccess(string sceneName, float duration)
        {
            Debug.Log($"[Example] Scene '{sceneName}' loaded successfully in {duration:F2} seconds.");
        }


        /// <summary>
        /// Scene load failure callback.
        /// </summary>
        private void OnLoadSceneFailure(string sceneName, string errorMessage)
        {
            Debug.LogError($"[Example] Failed to load scene '{sceneName}': {errorMessage}");
        }


        /// <summary>
        /// Scene load update callback.
        /// </summary>
        private void OnLoadSceneUpdate(string sceneName, float progress)
        {
            Debug.Log($"[Example] Loading scene '{sceneName}': {progress * 100f:F1}%");
        }


        /// <summary>
        /// Scene unload success callback.
        /// </summary>
        private void OnUnloadSceneSuccess(string sceneName)
        {
            Debug.Log($"[Example] Scene '{sceneName}' unloaded successfully.");
        }


        /// <summary>
        /// Scene unload failure callback.
        /// </summary>
        private void OnUnloadSceneFailure(string sceneName, string errorMessage)
        {
            Debug.LogError($"[Example] Failed to unload scene '{sceneName}': {errorMessage}");
        }


        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 300));

            GUILayout.Label("=== ImmoSceneManager Example ===");
            GUILayout.Space(10);

            // Scene name input
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scene Name:", GUILayout.Width(100));
            m_SceneToLoad = GUILayout.TextField(m_SceneToLoad, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Load mode selection
            GUILayout.BeginHorizontal();
            GUILayout.Label("Load Mode:", GUILayout.Width(100));
            if (GUILayout.Button(m_LoadMode.ToString(), GUILayout.Width(200)))
            {
                m_LoadMode = m_LoadMode == LoadSceneMode.Single ? LoadSceneMode.Additive : LoadSceneMode.Single;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Load button
            if (GUILayout.Button("Load Scene", GUILayout.Width(150)))
            {
                LoadScene();
            }

            // Unload button
            if (GUILayout.Button("Unload Scene", GUILayout.Width(150)))
            {
                UnloadScene();
            }

            GUILayout.Space(20);

            // Display scene status
            GUILayout.Label("=== Scene Status ===");

            if (ImmoSceneManager.Instance != null)
            {
                // Current active scene
                UnityEngine.SceneManagement.Scene activeScene = ImmoSceneManager.Instance.GetActiveScene();
                GUILayout.Label($"Active Scene: {activeScene.name}");

                GUILayout.Space(5);

                // Loaded scenes
                string[] loadedScenes = ImmoSceneManager.Instance.GetLoadedSceneNames();
                GUILayout.Label($"Loaded Scenes ({loadedScenes.Length}):");
                foreach (string scene in loadedScenes)
                {
                    GUILayout.Label($"  - {scene}");
                }

                // Loading scenes
                string[] loadingScenes = ImmoSceneManager.Instance.GetLoadingSceneNames();
                if (loadingScenes.Length > 0)
                {
                    GUILayout.Label($"Loading Scenes ({loadingScenes.Length}):");
                    foreach (string scene in loadingScenes)
                    {
                        GUILayout.Label($"  - {scene}");
                    }
                }

                // Unloading scenes
                string[] unloadingScenes = ImmoSceneManager.Instance.GetUnloadingSceneNames();
                if (unloadingScenes.Length > 0)
                {
                    GUILayout.Label($"Unloading Scenes ({unloadingScenes.Length}):");
                    foreach (string scene in unloadingScenes)
                    {
                        GUILayout.Label($"  - {scene}");
                    }
                }
            }

            GUILayout.EndArea();
        }
    }
}
