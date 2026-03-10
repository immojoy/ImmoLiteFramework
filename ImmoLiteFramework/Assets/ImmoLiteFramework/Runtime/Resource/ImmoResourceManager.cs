
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Immojoy.LiteFramework.Runtime
{

    [DisallowMultipleComponent]
    public sealed partial class ImmoResourceManager : MonoBehaviour
    {
        private readonly Dictionary<string, AsyncOperationHandle> m_LoadedHandles = new();
        private readonly Dictionary<string, int> m_ReferenceCounts = new();
        private readonly Dictionary<string, List<OnObjectLoadCallback>> m_OngoingCallbacks = new();


        /// <summary>
        /// Initializes the resource manager.
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// Disposes the resource manager, releasing all loaded assets.
        /// </summary>
        public void Dispose()
        {
            foreach (var handle in m_LoadedHandles.Values)
            {
                Addressables.Release(handle);
            }
            m_LoadedHandles.Clear();
            m_ReferenceCounts.Clear();
            m_OngoingCallbacks.Clear();
        }
    }
}