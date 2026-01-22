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
        [Header("Configuration")]
        [Tooltip("Optional: Custom UI layer configuration. If not set, default values will be used.")]
        [SerializeField] private ImmoUiLayerConfig m_LayerConfig;
        
        private static ImmoUiManager m_Instance;
        public static ImmoUiManager Instance => m_Instance;

        private Dictionary<string, ImmoUiView> m_CachedViews = new();
        private Dictionary<UiLayer, Transform> m_LayerRoots = new();
        
        // Default sort order for each UI layer (used when config is not set)
        private readonly Dictionary<UiLayer, int> m_DefaultLayerSortOrders = new()
        {
            { UiLayer.Background, 0 },
            { UiLayer.Normal, 100 },
            { UiLayer.Popup, 200 },
            { UiLayer.Top, 300 },
            { UiLayer.System, 400 }
        };
        
        
        /// <summary>
        /// Gets the sort order for a specific UI layer.
        /// Uses custom config if available, otherwise falls back to default values.
        /// </summary>
        private int GetLayerSortOrder(UiLayer layer)
        {
            if (m_LayerConfig != null)
            {
                return m_LayerConfig.GetSortOrder(layer);
            }
            
            return m_DefaultLayerSortOrders.TryGetValue(layer, out int sortOrder) ? sortOrder : 0;
        }


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
            
            // Set Canvas sort order for each layer to ensure proper rendering priority
            SetupLayerSortOrder();
        }
        
        
        private void SetupLayerSortOrder()
        {
            foreach (var layer in m_LayerRoots)
            {
                Canvas canvas = layer.Value.GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = layer.Value.gameObject.AddComponent<Canvas>();
                }
                
                // Set Canvas to Screen Space - Overlay or Camera based on your needs
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
                
                // Set sort order based on layer type
                canvas.sortingOrder = GetLayerSortOrder(layer.Key);
                
                // Ensure GraphicRaycaster exists for UI interaction
                if (layer.Value.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
                {
                    layer.Value.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }
            }
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