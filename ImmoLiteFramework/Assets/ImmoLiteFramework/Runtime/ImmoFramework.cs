using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Central framework manager that controls all sub-managers' lifecycle.
    /// Place this component on the root "Immo Lite Framework" GameObject.
    /// Sub-managers are created and initialized automatically by this class.
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

            CreateManagers();
            InitializeManagers();
        }


        private void CreateManagers()
        {
            EventManager = CreateManagerGameObject<ImmoEventManager>("Event Manager");
            ResourceManager = CreateManagerGameObject<ImmoResourceManager>("Resource Manager");
            FsmManager = CreateManagerGameObject<ImmoFsmManager>("Fsm Manager");
            SceneManager = CreateManagerGameObject<ImmoSceneManager>("Scene Manager");
            UiManager = CreateManagerGameObject<ImmoUiManager>("UI Manager");
            ProcedureManager = CreateManagerGameObject<ImmoProcedureManager>("Procedure Manager");
        }


        private T CreateManagerGameObject<T>(string goName) where T : MonoBehaviour
        {
            GameObject go = new GameObject(goName);
            go.transform.SetParent(transform, false);
            return go.AddComponent<T>();
        }


        private void InitializeManagers()
        {
            // Initialize in dependency order: no-dependency managers first
            EventManager.Initialize();
            ResourceManager.Initialize();
            FsmManager.Initialize();

            SceneManager.Initialize(ResourceManager, EventManager);

            UiManager.Initialize(ResourceManager);

            ProcedureManager.Initialize(FsmManager);
        }


        private void OnDestroy()
        {
            if (m_Instance != this)
            {
                return;
            }

            // Dispose in reverse initialization order
            ProcedureManager?.Dispose();
            UiManager?.Dispose();
            SceneManager?.Dispose();
            FsmManager?.Dispose();
            ResourceManager?.Dispose();
            EventManager?.Dispose();

            m_Instance = null;
        }
    }
}
