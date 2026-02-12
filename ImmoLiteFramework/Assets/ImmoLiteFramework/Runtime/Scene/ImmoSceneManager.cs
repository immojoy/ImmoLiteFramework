using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Scene manager.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Immojoy/Lite Framework/Manager/Immo Scene Manager")]
    public sealed class ImmoSceneManager : MonoBehaviour
    {
        private static ImmoSceneManager m_Instance;
        public static ImmoSceneManager Instance => m_Instance;


        private readonly List<string> m_LoadedSceneNames = new List<string>();
        private readonly List<string> m_LoadingSceneNames = new List<string>();
        private readonly List<string> m_UnloadingSceneNames = new List<string>();
        private readonly Dictionary<string, AsyncOperation> m_LoadingOperations = new Dictionary<string, AsyncOperation>();


        /// <summary>
        /// Scene load success event.
        /// </summary>
        public event Action<string, float> LoadSceneSuccess;


        /// <summary>
        /// Scene load failure event.
        /// </summary>
        public event Action<string, string> LoadSceneFailure;


        /// <summary>
        /// Scene load update event.
        /// </summary>
        public event Action<string, float> LoadSceneUpdate;


        /// <summary>
        /// Scene unload success event.
        /// </summary>
        public event Action<string> UnloadSceneSuccess;


        /// <summary>
        /// Scene unload failure event.
        /// </summary>
        public event Action<string, string> UnloadSceneFailure;


        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this);
                return;
            }

            m_Instance = this;
        }


        private void Update()
        {
            // Update loading progress
            List<string> completedScenes = new List<string>();
            foreach (var kvp in m_LoadingOperations)
            {
                string sceneName = kvp.Key;
                AsyncOperation operation = kvp.Value;

                // Trigger update event
                LoadSceneUpdate?.Invoke(sceneName, operation.progress);

                // Check if completed
                if (operation.isDone)
                {
                    completedScenes.Add(sceneName);
                }
            }

            // Clean up completed loading operations
            foreach (string sceneName in completedScenes)
            {
                m_LoadingOperations.Remove(sceneName);
            }
        }


        /// <summary>
        /// Gets whether a scene is loaded.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <returns>Whether the scene is loaded.</returns>
        public bool IsSceneLoaded(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is invalid.");
                return false;
            }

            return m_LoadedSceneNames.Contains(sceneName);
        }


        /// <summary>
        /// Gets whether a scene is loading.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <returns>Whether the scene is loading.</returns>
        public bool IsSceneLoading(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is invalid.");
                return false;
            }

            return m_LoadingSceneNames.Contains(sceneName);
        }


        /// <summary>
        /// Gets whether a scene is unloading.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <returns>Whether the scene is unloading.</returns>
        public bool IsSceneUnloading(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is invalid.");
                return false;
            }

            return m_UnloadingSceneNames.Contains(sceneName);
        }


        /// <summary>
        /// Gets the list of loaded scene names.
        /// </summary>
        /// <returns>List of loaded scene names.</returns>
        public string[] GetLoadedSceneNames()
        {
            return m_LoadedSceneNames.ToArray();
        }


        /// <summary>
        /// Gets the list of loading scene names.
        /// </summary>
        /// <returns>List of loading scene names.</returns>
        public string[] GetLoadingSceneNames()
        {
            return m_LoadingSceneNames.ToArray();
        }


        /// <summary>
        /// Gets the list of unloading scene names.
        /// </summary>
        /// <returns>List of unloading scene names.</returns>
        public string[] GetUnloadingSceneNames()
        {
            return m_UnloadingSceneNames.ToArray();
        }


        /// <summary>
        /// Load scene (asynchronous).
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="loadMode">Load mode.</param>
        public void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            LoadSceneAsync(sceneName, loadMode);
        }


        /// <summary>
        /// Load scene asynchronously.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="loadMode">Load mode (Single or Additive).</param>
        public void LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                string errorMessage = "Scene name is invalid.";
                Debug.LogError(errorMessage);
                LoadSceneFailure?.Invoke(sceneName, errorMessage);
                return;
            }

            if (IsSceneLoading(sceneName))
            {
                string errorMessage = $"Scene '{sceneName}' is already being loaded.";
                Debug.LogWarning(errorMessage);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                string errorMessage = $"Scene '{sceneName}' is already loaded.";
                Debug.LogWarning(errorMessage);
                return;
            }

            if (IsSceneUnloading(sceneName))
            {
                string errorMessage = $"Scene '{sceneName}' is being unloaded.";
                Debug.LogError(errorMessage);
                LoadSceneFailure?.Invoke(sceneName, errorMessage);
                return;
            }

            m_LoadingSceneNames.Add(sceneName);

            try
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
                if (operation == null)
                {
                    string errorMessage = $"Failed to start loading scene '{sceneName}'.";
                    Debug.LogError(errorMessage);
                    m_LoadingSceneNames.Remove(sceneName);
                    LoadSceneFailure?.Invoke(sceneName, errorMessage);
                    return;
                }

                m_LoadingOperations[sceneName] = operation;

                float startTime = Time.realtimeSinceStartup;

                operation.completed += (AsyncOperation op) =>
                {
                    m_LoadingSceneNames.Remove(sceneName);

                    if (op.isDone)
                    {
                        m_LoadedSceneNames.Add(sceneName);
                        float duration = Time.realtimeSinceStartup - startTime;
                        Debug.Log($"Scene '{sceneName}' loaded successfully in {duration:F2} seconds.");
                        LoadSceneSuccess?.Invoke(sceneName, duration);
                    }
                    else
                    {
                        string errorMessage = $"Scene '{sceneName}' failed to load.";
                        Debug.LogError(errorMessage);
                        LoadSceneFailure?.Invoke(sceneName, errorMessage);
                    }
                };
            }
            catch (Exception ex)
            {
                m_LoadingSceneNames.Remove(sceneName);
                string errorMessage = $"Exception occurred while loading scene '{sceneName}': {ex.Message}";
                Debug.LogError(errorMessage);
                LoadSceneFailure?.Invoke(sceneName, errorMessage);
            }
        }


        /// <summary>
        /// Unload scene (asynchronous).
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        public void UnloadScene(string sceneName)
        {
            UnloadSceneAsync(sceneName);
        }


        /// <summary>
        /// Unload scene asynchronously.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        public void UnloadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                string errorMessage = "Scene name is invalid.";
                Debug.LogError(errorMessage);
                UnloadSceneFailure?.Invoke(sceneName, errorMessage);
                return;
            }

            if (IsSceneUnloading(sceneName))
            {
                string errorMessage = $"Scene '{sceneName}' is already being unloaded.";
                Debug.LogWarning(errorMessage);
                return;
            }

            if (IsSceneLoading(sceneName))
            {
                string errorMessage = $"Scene '{sceneName}' is being loaded.";
                Debug.LogError(errorMessage);
                UnloadSceneFailure?.Invoke(sceneName, errorMessage);
                return;
            }

            if (!IsSceneLoaded(sceneName))
            {
                string errorMessage = $"Scene '{sceneName}' is not loaded yet.";
                Debug.LogError(errorMessage);
                UnloadSceneFailure?.Invoke(sceneName, errorMessage);
                return;
            }

            m_UnloadingSceneNames.Add(sceneName);

            try
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
                if (operation == null)
                {
                    string errorMessage = $"Failed to start unloading scene '{sceneName}'.";
                    Debug.LogError(errorMessage);
                    m_UnloadingSceneNames.Remove(sceneName);
                    UnloadSceneFailure?.Invoke(sceneName, errorMessage);
                    return;
                }

                operation.completed += (AsyncOperation op) =>
                {
                    m_UnloadingSceneNames.Remove(sceneName);

                    if (op.isDone)
                    {
                        m_LoadedSceneNames.Remove(sceneName);
                        Debug.Log($"Scene '{sceneName}' unloaded successfully.");
                        UnloadSceneSuccess?.Invoke(sceneName);
                    }
                    else
                    {
                        string errorMessage = $"Scene '{sceneName}' failed to unload.";
                        Debug.LogError(errorMessage);
                        UnloadSceneFailure?.Invoke(sceneName, errorMessage);
                    }
                };
            }
            catch (Exception ex)
            {
                m_UnloadingSceneNames.Remove(sceneName);
                string errorMessage = $"Exception occurred while unloading scene '{sceneName}': {ex.Message}";
                Debug.LogError(errorMessage);
                UnloadSceneFailure?.Invoke(sceneName, errorMessage);
            }
        }


        /// <summary>
        /// Gets the current active scene.
        /// </summary>
        /// <returns>Current active scene.</returns>
        public UnityEngine.SceneManagement.Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }


        /// <summary>
        /// Sets the active scene.
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        /// <returns>Whether setting was successful.</returns>
        public bool SetActiveScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is invalid.");
                return false;
            }

            if (!IsSceneLoaded(sceneName))
            {
                Debug.LogError($"Scene '{sceneName}' is not loaded.");
                return false;
            }

            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid())
            {
                Debug.LogError($"Scene '{sceneName}' is not valid.");
                return false;
            }

            return SceneManager.SetActiveScene(scene);
        }


        private void OnDestroy()
        {
            // Clean up event subscriptions
            LoadSceneSuccess = null;
            LoadSceneFailure = null;
            LoadSceneUpdate = null;
            UnloadSceneSuccess = null;
            UnloadSceneFailure = null;
        }
    }
}
