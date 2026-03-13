using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Immojoy.LiteFramework.Runtime
{
    public sealed partial class ImmoResourceManager : MonoBehaviour
    {
        /// <summary>
        /// Starts loading a scene via Addressables. The scene is loaded additively
        /// with activation deferred (activateOnLoad = false).
        /// The caller is responsible for tracking the returned handle and activating the scene.
        /// </summary>
        /// <param name="sceneAddress">Addressable address of the scene.</param>
        /// <returns>The async operation handle for tracking progress and completion.</returns>
        public AsyncOperationHandle<SceneInstance> LoadSceneHandle(string sceneAddress)
        {
            if (string.IsNullOrEmpty(sceneAddress))
            {
                Debug.LogError("[ImmoLiteFramework]-[ResourceManager] Scene address cannot be null or empty.");
                return default;
            }

            return Addressables.LoadSceneAsync(sceneAddress,
                UnityEngine.SceneManagement.LoadSceneMode.Additive, false);
        }


        /// <summary>
        /// Starts unloading a previously loaded scene via Addressables.
        /// </summary>
        /// <param name="sceneHandle">The handle returned from LoadSceneHandle.</param>
        /// <returns>The async operation handle for tracking completion.</returns>
        public AsyncOperationHandle<SceneInstance> UnloadSceneHandle(AsyncOperationHandle<SceneInstance> sceneHandle)
        {
            return Addressables.UnloadSceneAsync(sceneHandle);
        }
    }
}