
using System;
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Procedure manager.</br>
    /// Procedure is a finite state machine that spans the entire lifecycle of the game runtime.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ImmoProcedureManager : MonoBehaviour
    {
        private ImmoFsmManager m_FsmManager;
        private IImmoFsm<ImmoProcedureManager> m_ProcedureFsm;


        /// <summary>
        /// Gets the current procedure.
        /// </summary>
        public ImmoProcedureBase CurrentProcedure
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    Debug.LogError("You must initialize procedure first.");
                    return null;
                }

                return (ImmoProcedureBase)m_ProcedureFsm.CurrentState;
            }
        }


        /// <summary>
        /// Gets the current procedure duration.
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    Debug.LogError("You must initialize procedure first.");
                    return 0f;
                }

                return m_ProcedureFsm.CurrentStateTime;
            }
        }


        /// <summary>
        /// Initializes the procedure manager with its required FSM manager dependency.
        /// </summary>
        /// <param name="fsmManager">The FSM manager used to create procedure state machines.</param>
        public void Initialize(ImmoFsmManager fsmManager)
        {
            m_FsmManager = fsmManager;
        }

        /// <summary>
        /// Disposes the procedure manager, destroying its state machine.
        /// </summary>
        public void Dispose()
        {
            if (m_ProcedureFsm != null && m_FsmManager != null)
            {
                m_FsmManager.DestroyFsm(m_ProcedureFsm);
                m_ProcedureFsm = null;
            }
        }

        /// <summary>
        /// Sets up the procedure manager with the provided procedures.
        /// </summary>
        /// <param name="procedures">Procedures contained in the procedure manager.</param>
        public void SetupProcedures(params ImmoProcedureBase[] procedures)
        {
            if (m_FsmManager == null)
            {
                Debug.LogError("FSM manager is invalid.");
                return;
            }

            if (procedures == null || procedures.Length == 0)
            {
                Debug.LogError("Procedures is invalid.");
                return;
            }

            m_ProcedureFsm = m_FsmManager.CreateFsm(this, procedures);

            if (m_ProcedureFsm == null)
            {
                Debug.LogError("Failed to create procedure FSM.");
            }
        }


        /// <summary>
        /// Start procedure.
        /// </summary>
        /// <typeparam name="T">Procedure type to start.</typeparam>
        public void StartProcedure<T>() where T : ImmoProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return;
            }

            m_ProcedureFsm.Start<T>();
        }


        /// <summary>
        /// Start procedure.
        /// </summary>
        /// <param name="procedureType">Procedure type to start.</param>
        public void StartProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return;
            }

            if (procedureType == null)
            {
                Debug.LogError("Procedure type is invalid.");
                return;
            }

            if (!typeof(ImmoProcedureBase).IsAssignableFrom(procedureType))
            {
                Debug.LogError($"Procedure type '{procedureType.FullName}' is invalid.");
                return;
            }

            m_ProcedureFsm.Start(procedureType);
        }


        /// <summary>
        /// Check if procedure exists.
        /// </summary>
        /// <typeparam name="T">Procedure type to check.</typeparam>
        /// <returns>Whether the procedure exists.</returns>
        public bool HasProcedure<T>() where T : ImmoProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return false;
            }

            return m_ProcedureFsm.HasState<T>();
        }


        /// <summary>
        /// Check if procedure exists.
        /// </summary>
        /// <param name="procedureType">Procedure type to check.</param>
        /// <returns>Whether the procedure exists.</returns>
        public bool HasProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return false;
            }

            if (procedureType == null)
            {
                Debug.LogError("Procedure type is invalid.");
                return false;
            }

            if (!typeof(ImmoProcedureBase).IsAssignableFrom(procedureType))
            {
                Debug.LogError($"Procedure type '{procedureType.FullName}' is invalid.");
                return false;
            }

            return m_ProcedureFsm.HasState(procedureType);
        }


        /// <summary>
        /// Get procedure.
        /// </summary>
        /// <typeparam name="T">Procedure type to get.</typeparam>
        /// <returns>The procedure to get.</returns>
        public T GetProcedure<T>() where T : ImmoProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return null;
            }

            return m_ProcedureFsm.GetState<T>();
        }


        /// <summary>
        /// Get procedure.
        /// </summary>
        /// <param name="procedureType">Procedure type to get.</param>
        /// <returns>The procedure to get.</returns>
        public ImmoProcedureBase GetProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return null;
            }

            if (procedureType == null)
            {
                Debug.LogError("Procedure type is invalid.");
                return null;
            }

            if (!typeof(ImmoProcedureBase).IsAssignableFrom(procedureType))
            {
                Debug.LogError($"Procedure type '{procedureType.FullName}' is invalid.");
                return null;
            }

            return (ImmoProcedureBase)m_ProcedureFsm.GetState(procedureType);
        }
    }
}
