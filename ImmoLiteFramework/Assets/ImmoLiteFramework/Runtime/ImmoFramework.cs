using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Central framework manager that manages all sub-managers' lifecycle.
    /// Place this component on the root "Immo Lite Framework" GameObject.
    /// All sub-managers should be on child GameObjects.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Immojoy/Lite Framework/Immo Framework")]
    public sealed class ImmoFramework : MonoBehaviour
    {
        private static ImmoFramework m_Instance;
        public static ImmoFramework Instance => m_Instance;


        public ImmoEventManager EventManager { get; private set; }
        public ImmoResourceManager ResourceManager { get; private set; }
        public ImmoFsmManager FsmManager { get; private set; }
        public ImmoSceneManager SceneManager { get; private set; }
        public ImmoUiManager UiManager { get; private set; }
        public ImmoProcedureManager ProcedureManager { get; private set; }


        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            m_Instance = this;
            DontDestroyOnLoad(gameObject);

            DiscoverManagers();
        }


        private void DiscoverManagers()
        {
            EventManager = GetComponentInChildren<ImmoEventManager>();
            ResourceManager = GetComponentInChildren<ImmoResourceManager>();
            FsmManager = GetComponentInChildren<ImmoFsmManager>();
            SceneManager = GetComponentInChildren<ImmoSceneManager>();
            UiManager = GetComponentInChildren<ImmoUiManager>();
            ProcedureManager = GetComponentInChildren<ImmoProcedureManager>();
        }


        private void OnDestroy()
        {
            if (m_Instance == this)
            {
                m_Instance = null;
            }
        }
    }
}
