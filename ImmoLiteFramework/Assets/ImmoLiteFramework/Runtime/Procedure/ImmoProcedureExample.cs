
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Procedure usage example.</br>
    /// Demonstrates how to use the procedure manager to manage different game states.
    /// </summary>
    [AddComponentMenu("Immojoy/Lite Framework/Procedure/Immo Procedure Example")]
    public class ImmoProcedureExample : MonoBehaviour
    {
        private void Start()
        {
            // Ensure procedure manager exists
            if (ImmoProcedureManager.Instance == null)
            {
                Debug.LogError("ImmoProcedureManager is not found in the scene.");
                return;
            }

            // Create procedure instances
            ImmoLaunchProcedure launchProcedure = new ImmoLaunchProcedure();
            ImmoMainMenuProcedure mainMenuProcedure = new ImmoMainMenuProcedure();
            ImmoGamePlayProcedure gamePlayProcedure = new ImmoGamePlayProcedure();

            // Initialize procedure manager
            ImmoProcedureManager.Instance.Initialize(launchProcedure, mainMenuProcedure, gamePlayProcedure);

            // Set initial data
            ImmoProcedureManager.Instance.GetProcedure<ImmoLaunchProcedure>()?.SetData("GameVersion", "1.0.0");

            // Start procedure
            ImmoProcedureManager.Instance.StartProcedure<ImmoLaunchProcedure>();
        }


        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Current Procedure: {ImmoProcedureManager.Instance.CurrentProcedure?.GetType().Name ?? "None"}");
            GUILayout.Label($"Procedure Time: {ImmoProcedureManager.Instance.CurrentProcedureTime:F2}s");
            GUILayout.EndArea();
        }
    }


    /// <summary>
    /// Launch procedure.</br>
    /// Responsible for initialization work when the game starts.
    /// </summary>
    public class ImmoLaunchProcedure : ImmoProcedureBase
    {
        private string m_GameVersion;


        protected internal override void OnInit(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            Debug.Log("Launch Procedure: OnInit");
        }


        protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("Launch Procedure: OnEnter - Game launching...");

            // Try to get game version from FSM data
            // if (procedureOwner.TryGetData("GameVersion", out string version))
            // {
            //     m_GameVersion = version;
            //     Debug.Log($"Game Version: {m_GameVersion}");
            // }

            // Set launch completed flag
            // procedureOwner.SetData("LaunchCompleted", false);
        }


        protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // Simulate launch process (complete after 3 seconds)
            if (procedureOwner.CurrentStateTime >= 3f)
            {
                // procedureOwner.SetData("LaunchCompleted", true);
                Debug.Log("Launch Procedure: Launch completed, switching to main menu");
                ChangeState<ImmoMainMenuProcedure>(procedureOwner);
            }
        }


        protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Debug.Log("Launch Procedure: OnLeave");
        }


        protected internal override void OnDestroy(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnDestroy(procedureOwner);
            Debug.Log("Launch Procedure: OnDestroy");
        }


        public void SetData(string key, string value)
        {
            if (key == "GameVersion")
            {
                m_GameVersion = value;
            }
        }
    }


    /// <summary>
    /// Main menu procedure.</br>
    /// Responsible for displaying the main menu interface.
    /// </summary>
    public class ImmoMainMenuProcedure : ImmoProcedureBase
    {
        protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("Main Menu Procedure: OnEnter - Entering main menu");

            // Check if launch is completed
            // if (procedureOwner.TryGetData("LaunchCompleted", out bool isCompleted))
            // {
            //     Debug.Log($"Launch Completed: {isCompleted}");
            // }
        }


        protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // Detect key press, simulate start game
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Main Menu Procedure: Start game");
                ChangeState<ImmoGamePlayProcedure>(procedureOwner);
            }
        }


        protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Debug.Log("Main Menu Procedure: OnLeave");
        }
    }


    /// <summary>
    /// Game play procedure.</br>
    /// Responsible for game play logic.
    /// </summary>
    public class ImmoGamePlayProcedure : ImmoProcedureBase
    {
        private float m_GameTime;


        protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Debug.Log("Game Play Procedure: OnEnter - Start game");

            m_GameTime = 0f;
            // procedureOwner.SetData("GameStartTime", Time.time);
        }


        protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            m_GameTime += elapseSeconds;

            // Press Escape to return to main menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log($"Game Play Procedure: Game time {m_GameTime:F2} seconds, return to main menu");
                ChangeState<ImmoMainMenuProcedure>(procedureOwner);
            }
        }


        protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Debug.Log("Game Play Procedure: OnLeave");

            // Remove game data
            // procedureOwner.RemoveData("GameStartTime");
        }
    }
}
