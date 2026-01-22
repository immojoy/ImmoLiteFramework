using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Immojoy/Lite Framework/Manager/Immo UI Manager")]
    public class ImmoUiManager : MonoBehaviour
    {
        private static ImmoUiManager m_Instance;
        public static ImmoUiManager Instance => m_Instance;

        private Dictionary<string, ImmoUiView> m_CachedViews = new();
        private Dictionary<UiLayer, Transform> m_LayerRoots = new();


        // TODO: Currently, only singular views are supported. Consider adding support for multiple instances if needed.
        public void ShowUi(string assetAddress, object args = null)
        {
            if (m_CachedViews.TryGetValue(assetAddress, out ImmoUiView cachedView))
            {
                cachedView.OnShow(args);
            }
            else
            {
                ImmoResourceManager.Instance.LoadAssetAsyncWithCallback<GameObject>(assetAddress, OnUiLoadSuccess, args);
            }
        }


        public void HideUi(string assetAddress)
        {
            if (m_CachedViews.TryGetValue(assetAddress, out ImmoUiView cachedView))
            {
                cachedView.OnHide();
            }
        }


        public void DestroyUi(string viewName)
        {
            if (m_CachedViews.TryGetValue(viewName, out ImmoUiView view))
            {
                view.OnDestroy();
                m_CachedViews.Remove(viewName);

                ImmoResourceManager.Instance.ReleaseAsset(viewName);
            }
        }


        private void InitializeLayers()
        {
            m_LayerRoots[UiLayer.Background] = GameObject.Find("UI Background").transform;
            m_LayerRoots[UiLayer.Normal] = GameObject.Find("UI Normal").transform;
            m_LayerRoots[UiLayer.Popup] = GameObject.Find("UI Popup").transform;
            m_LayerRoots[UiLayer.Top] = GameObject.Find("UI Top").transform;
            m_LayerRoots[UiLayer.System] = GameObject.Find("UI System").transform;
        }


        private void OnUiLoadSuccess(string address, object uiPrefab, float duration, object args)
        {
            GameObject uiObject = Instantiate(uiPrefab as GameObject);
            ImmoUiView uiView = uiObject.GetComponent<ImmoUiView>();

            if (uiView != null)
            {
                uiView.OnCreate();
                uiView.OnShow(args);
                m_CachedViews[address] = uiView;

                // Set parent based on UiLayerAttribute
                UiLayer layer = GetUiLayer(uiView.GetType()) ?? UiLayer.Normal;  // Fallback to default layer if not specified
                uiObject.transform.SetParent(m_LayerRoots[layer], false);
            }
            else
            {
                Debug.LogError($"The loaded UI prefab at {address} does not have an ImmoUiView component.");
                Destroy(uiObject);
            }
        }


        private UiLayer? GetUiLayer(Type viewType)
        {
            var attributes = viewType.GetCustomAttributes(typeof(UiLayerAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (var attr in attributes)
                {
                    if (attr is UiLayerAttribute attribute)
                    {
                        return attribute.LayerType;
                    }
                }
            }
            return null;
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

                InitializeLayers();
            }
        }


        // Start is called before the first frame update
        private void Start()
        {

        }


        // Update is called once per frame
        private void Update()
        {

        }
    }
}