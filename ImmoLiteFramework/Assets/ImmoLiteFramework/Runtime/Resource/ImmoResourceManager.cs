
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Immojoy.LiteFramework.Runtime
{
    public class ImmoResourceManager : MonoBehaviour
    {
        private static ImmoResourceManager m_Instance;
        public static ImmoResourceManager Instance => m_Instance;


        private readonly Dictionary<string, AsyncOperationHandle> m_LoadedHandles = new();
        private readonly Dictionary<string, int> m_ReferenceCounts = new();
        private readonly Dictionary<string, List<OnAssetLoadSuccess>> m_OngoingCallbacks = new();


        /// <summary>
        /// Asynchronously loads an asset from the specified address with callbacks.
        /// </summary>
        public void LoadAssetAsyncWithCallback<T>(string assetAddress, OnAssetLoadSuccess successCallback, object data) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetAddress))
            {
                throw new ArgumentException("Asset address cannot be null or empty.", nameof(assetAddress));
            }

            if (m_LoadedHandles.TryGetValue(assetAddress, out AsyncOperationHandle existingHandle))
            {
                if (existingHandle.Result is T result)
                {
                    m_ReferenceCounts[assetAddress]++;
                    successCallback?.Invoke(assetAddress, result, 0, data);
                    return;
                }
            }

            // Check for ongoing loading of the same asset to avoid duplication
            if (m_OngoingCallbacks.TryGetValue(assetAddress, out List<OnAssetLoadSuccess> callbacks))
            {
                callbacks.Add(successCallback);
                return;
            }

            // Start new load operation
            m_OngoingCallbacks[assetAddress] = new List<OnAssetLoadSuccess> { successCallback };

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetAddress);
            handle.Completed += operation =>
            {
                if (m_OngoingCallbacks.TryGetValue(assetAddress, out List<OnAssetLoadSuccess> callbacks))
                {
                    if (operation.Status == AsyncOperationStatus.Succeeded)
                    {
                        m_LoadedHandles[assetAddress] = handle;
                        m_ReferenceCounts[assetAddress] = 1;

                        foreach (var callback in callbacks)
                        {
                            callback?.Invoke(assetAddress, operation.Result, 0, data);
                        }
                    }
                    else
                    {
                        foreach (var callback in callbacks)
                        {
                            callback?.Invoke(assetAddress, null, 0, data);
                        }
                    }
                }

                m_OngoingCallbacks.Remove(assetAddress);
            };
        }


        /// <summary>
        /// Asynchronously loads an asset from the specified address.
        /// </summary>
        public async Task<T> LoadAssetAsync<T>(string assetAddress) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetAddress))
            {
                throw new ArgumentException("Asset address cannot be null or empty.", nameof(assetAddress));
            }

            if (m_LoadedHandles.TryGetValue(assetAddress, out AsyncOperationHandle existingHandle))
            {
                if (existingHandle.Result is T result)
                {
                    m_ReferenceCounts[assetAddress]++;
                    return result;
                }
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetAddress);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                m_LoadedHandles[assetAddress] = handle;
                m_ReferenceCounts[assetAddress] = 1;

                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset at address: {assetAddress}");
                return null;
            }
        }


        /// <summary>
        /// Releases the asset at the specified address.
        /// </summary>
        /// <param name="assetAddress">Asset address</param>
        public void ReleaseAsset(string assetAddress)
        {
            if (m_LoadedHandles.TryGetValue(assetAddress, out AsyncOperationHandle handle))
            {
                if (m_ReferenceCounts.ContainsKey(assetAddress))
                {
                    m_ReferenceCounts[assetAddress]--;
                    if (m_ReferenceCounts[assetAddress] <= 0)
                    {
                        Addressables.Release(handle);
                        m_LoadedHandles.Remove(assetAddress);
                        m_ReferenceCounts.Remove(assetAddress);
                    }
                }
            }
        }


        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this);
            }
            else
            {
                m_Instance = this;
                DontDestroyOnLoad(this);
            }
        }
    }
}