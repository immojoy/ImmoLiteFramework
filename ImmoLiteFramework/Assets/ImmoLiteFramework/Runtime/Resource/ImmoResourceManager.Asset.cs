
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Immojoy.LiteFramework.Runtime
{
    public sealed partial class ImmoResourceManager : MonoBehaviour
    {
        /// <summary>
        /// Asynchronously loads an asset from the specified address with callbacks.
        /// </summary>
        public void LoadAssetAsyncWithCallback<T>(string assetAddress, OnObjectLoadCallback callback, object data) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetAddress))
            {
                Debug.LogError($"[ImmoLiteFramework]-[ResourceManager] Asset address cannot be null or empty.");
                return;
            }

            if (m_LoadedHandles.TryGetValue(assetAddress, out AsyncOperationHandle existingHandle))
            {
                if (existingHandle.Result is T result)
                {
                    m_ReferenceCounts[assetAddress]++;
                    callback.SuccessCallback?.Invoke(assetAddress, result, 0, data);
                    return;
                }
            }

            // Check for ongoing loading of the same asset to avoid duplication
            if (m_OngoingCallbacks.TryGetValue(assetAddress, out List<OnObjectLoadCallback> callbacks))
            {
                callbacks.Add(callback);
                return;
            }

            // Start new load operation
            m_OngoingCallbacks[assetAddress] = new List<OnObjectLoadCallback> { callback };

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetAddress);
            handle.Completed += operation =>
            {
                if (m_OngoingCallbacks.TryGetValue(assetAddress, out List<OnObjectLoadCallback> callbacks))
                {
                    if (operation.Status == AsyncOperationStatus.Succeeded)
                    {
                        m_LoadedHandles[assetAddress] = handle;
                        m_ReferenceCounts[assetAddress] = 1;

                        foreach (var callback in callbacks)
                        {
                            callback.SuccessCallback?.Invoke(assetAddress, operation.Result, 0, data);
                        }
                    }
                    else if (operation.Status == AsyncOperationStatus.Failed)
                    {
                        foreach (var callback in callbacks)
                        {
                            callback.FailureCallback?.Invoke(assetAddress, operation.OperationException?.Message, data);
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
                Debug.LogError($"[ImmoLiteFramework]-[ResourceManager] Asset address cannot be null or empty.");
                return null;
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
                Debug.LogError($"[ImmoLiteFramework]-[ResourceManager] Failed to load asset at address: {assetAddress}");
                return null;
            }
        }


        /// <summary>
        /// Unloads the asset at the specified address.
        /// </summary>
        /// <param name="assetAddress">Asset address</param>
        public void UnloadAsset(string assetAddress)
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
    }
}