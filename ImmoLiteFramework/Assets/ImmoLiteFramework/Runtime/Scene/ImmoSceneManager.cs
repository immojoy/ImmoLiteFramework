using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Scene manager. Handles scene transitions with optional transition effects and loading screens.
    /// Owns all scene handle tracking (single source of truth).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ImmoSceneManager : MonoBehaviour
    {
        private ImmoResourceManager m_ResourceManager;
        private ImmoEventManager m_EventManager;

        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> m_LoadedScenes = new();
        private readonly Dictionary<string, IImmoTransitionHandler> m_TransitionHandlers = new();

        private bool m_IsTransitioning = false;
        private float m_SceneLoadDuration = 0f;


        /// <summary>
        /// Registers a transition handler with the given name.
        /// </summary>
        /// <param name="handlerName">The name of the transition handler.</param>
        /// <param name="handler">The transition handler instance.</param>
        public void RegisterTransitionHandler(string handlerName, IImmoTransitionHandler handler)
        {
            if (string.IsNullOrEmpty(handlerName) || handler == null)
            {
                Debug.LogError("[ImmoLiteFramework]-[SceneManager] Invalid transition handler registration.");
                return;
            }
            m_TransitionHandlers[handlerName] = handler;
        }


        /// <summary>
        /// Retrieves a registered transition handler by its name.
        /// </summary>
        /// <param name="handlerName">The name of the transition handler.</param>
        /// <returns>The transition handler instance if found; otherwise, null.</returns>
        public IImmoTransitionHandler GetTransitionHandler(string handlerName)
        {
            if (string.IsNullOrEmpty(handlerName))
            {
                Debug.LogError("[ImmoLiteFramework]-[SceneManager] Handler name cannot be null or empty.");
                return null;
            }
            m_TransitionHandlers.TryGetValue(handlerName, out var handler);
            return handler;
        }


        /// <summary>
        /// Performs a scene transition according to the given configuration.
        /// </summary>
        public async Task LoadSceneAsync(ImmoSceneTransitionConfig config)
        {
            if (m_IsTransitioning)
            {
                Debug.LogWarning("[ImmoLiteFramework]-[SceneManager] Scene transition is already in progress.");
                return;
            }
            m_IsTransitioning = true;

            try
            {
                await ExecuteTransitionAsync(config);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ImmoLiteFramework]-[SceneManager] Scene transition failed: {ex}");
            }
            finally
            {
                m_IsTransitioning = false;
            }
        }


        private async Task ExecuteTransitionAsync(ImmoSceneTransitionConfig config)
        {
            IImmoTransitionHandler transition = config.TransitionHandler;
            IImmoLoadingHandler loading = config.UseLoadingScreen ? config.LoadingHandler : null;

            // # Step 1: Fade in transition cover
            if (transition != null)
            {
                await transition.FadeInAsync();

                // Trigger event and callback after fade-in completes, before heavy loading starts
                m_EventManager.TriggerEvent(new ImmmoFadeInCompleteEvent(this));
                config.FadeInCompleteCallback?.Invoke();
            }

            try
            {
                // # Step 2: Show loading screen
                if (loading != null)
                {
                    await loading.ShowLoadingScreenAsync();
                }

                // Start minimum display time timer in parallel with loading
                Task minDisplayTask = (loading == null && config.MinTransitionTime > 0f)
                    ? Task.Delay(TimeSpan.FromSeconds(config.MinTransitionTime))
                    : Task.CompletedTask;

                // # Step 3: Load new scenes and preload assets
                await LoadScenesInternalAsync(config);

                // # Step 4: Activate all newly loaded scenes
                await ActivateScenesAsync(config.ScenesToLoad);

                // # Step 5: Set the first loaded scene as active
                if (config.ScenesToLoad.Count > 0)
                {
                    string firstScene = config.ScenesToLoad[0];
                    if (m_LoadedScenes.TryGetValue(firstScene, out var handle))
                    {
                        SceneManager.SetActiveScene(handle.Result.Scene);
                    }
                }

                // # Step 6: Unload old scenes (after new ones are ready)
                await UnloadScenesInternalAsync(config.ScenesToUnload);

                // # Step 7: Wait for minimum display time
                await minDisplayTask;

                // # Step 8: Hide loading screen
                if (loading != null)
                {
                    await loading.HideLoadingScreenAsync();
                }
            }
            finally
            {
                // Always fade out to prevent a stuck transition cover
                if (transition != null)
                {
                    try
                    {
                        await transition.FadeOutAsync();

                        // Trigger event and callback after fade-out completes, after all loading is done
                        m_EventManager.TriggerEvent(new ImmmoFadeOutCompleteEvent(this));
                        config.FadeOutCompleteCallback?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[ImmoLiteFramework]-[SceneManager] Failed to fade out transition: {ex}");
                    }
                }
            }
        }


        private async Task LoadScenesInternalAsync(ImmoSceneTransitionConfig config)
        {
            int total = config.ScenesToLoad.Count;
            int completed = 0;

            // Preload additional assets
            foreach (string asset in config.AssetsToPreload)
            {
                await m_ResourceManager.LoadAssetAsync<UnityEngine.Object>(asset);
            }

            // Load scenes with fine-grained progress
            foreach (string scene in config.ScenesToLoad)
            {
                if (m_LoadedScenes.ContainsKey(scene))
                {
                    completed++;

                    m_EventManager.TriggerEvent(new ImmoSceneLoadSuccessEvent(this, scene, 0f));
                    config.SceneLoadSuccessCallback?.Invoke(scene, 0f);

                    m_EventManager.TriggerEvent(new ImmoSceneLoadProgressEvent(this, scene, 1f, completed / (float)total));
                    config.SceneLoadProgressCallback?.Invoke(scene, 1f, completed / (float)total);
                    continue;
                }

                AsyncOperationHandle<SceneInstance> handle = m_ResourceManager.LoadSceneHandle(scene);

                // Poll progress until complete
                while (!handle.IsDone)
                {
                    float progress = Mathf.Clamp01(handle.PercentComplete);
                    float overallProgress = (completed + progress) / total;

                    m_EventManager.TriggerEvent(new ImmoSceneLoadProgressEvent(this, scene, progress, overallProgress));
                    config.SceneLoadProgressCallback?.Invoke(scene, progress, overallProgress);

                    await Task.Yield();
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    m_LoadedScenes[scene] = handle;
                }
                else
                {
                    Debug.LogError($"[ImmoLiteFramework]-[SceneManager] Failed to load scene '{scene}': {handle.OperationException}");
                }

                completed++;

                m_EventManager.TriggerEvent(new ImmoSceneLoadSuccessEvent(this, scene, 0f));
                config.SceneLoadSuccessCallback?.Invoke(scene, 0f);
            }
        }


        private async Task ActivateScenesAsync(List<string> scenesToActivate)
        {
            foreach (string scene in scenesToActivate)
            {
                if (m_LoadedScenes.TryGetValue(scene, out var handle))
                {
                    AsyncOperation activateOp = handle.Result.ActivateAsync();
                    while (!activateOp.isDone)
                    {
                        await Task.Yield();
                    }
                }
            }
        }


        private async Task UnloadScenesInternalAsync(List<string> scenesToUnload)
        {
            foreach (string scene in scenesToUnload)
            {
                if (m_LoadedScenes.TryGetValue(scene, out var handle))
                {
                    AsyncOperationHandle<SceneInstance> unloadHandle = m_ResourceManager.UnloadSceneHandle(handle);
                    await unloadHandle.Task;

                    if (unloadHandle.Status != AsyncOperationStatus.Succeeded)
                    {
                        Debug.LogError($"[ImmoLiteFramework]-[SceneManager] Failed to unload scene '{scene}': {unloadHandle.OperationException}");
                    }

                    m_LoadedScenes.Remove(scene);
                }
            }
        }


        /// <summary>
        /// Loads a single scene additively and tracks its handle.
        /// Does not involve any transition or loading screen.
        /// </summary>
        public async Task LoadSceneAsync(string sceneAddress)
        {
            if (m_LoadedScenes.ContainsKey(sceneAddress))
            {
                Debug.LogWarning($"[ImmoLiteFramework]-[SceneManager] Scene '{sceneAddress}' is already loaded.");
                return;
            }

            AsyncOperationHandle<SceneInstance> handle = m_ResourceManager.LoadSceneHandle(sceneAddress);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                m_LoadedScenes[sceneAddress] = handle;
                AsyncOperation activateOp = handle.Result.ActivateAsync();
                while (!activateOp.isDone)
                {
                    await Task.Yield();
                }
            }
            else
            {
                Debug.LogError($"[ImmoLiteFramework]-[SceneManager] Failed to load scene '{sceneAddress}': {handle.OperationException}");
            }
        }


        /// <summary>
        /// Unloads a previously loaded scene.
        /// </summary>
        public async Task UnloadSceneAsync(string sceneAddress)
        {
            if (!m_LoadedScenes.TryGetValue(sceneAddress, out var handle))
            {
                Debug.LogWarning($"[ImmoLiteFramework]-[SceneManager] Scene '{sceneAddress}' is not loaded.");
                return;
            }

            AsyncOperationHandle<SceneInstance> unloadHandle = m_ResourceManager.UnloadSceneHandle(handle);
            await unloadHandle.Task;

            if (unloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[ImmoLiteFramework]-[SceneManager] Failed to unload scene '{sceneAddress}': {unloadHandle.OperationException}");
            }

            m_LoadedScenes.Remove(sceneAddress);
        }


        public void Initialize(ImmoResourceManager resourceManager, ImmoEventManager eventManager)
        {
            m_ResourceManager = resourceManager;
            m_EventManager = eventManager;
        }


        public void Dispose()
        {
            foreach (var kvp in m_LoadedScenes)
            {
                if (kvp.Value.IsValid())
                {
                    Addressables.UnloadSceneAsync(kvp.Value);
                }
            }
            m_LoadedScenes.Clear();
        }
    }
}
