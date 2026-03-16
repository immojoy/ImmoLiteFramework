
using System;
using System.Collections.Generic;

using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Finite state machine.
    /// </summary>
    /// <typeparam name="T">Finite state machine owner type.</typeparam>
    internal sealed class ImmoFsm<T> : IImmoFsm<T> where T : class
    {
        private string m_Name;
        private T m_Owner;
        private readonly Dictionary<Type, ImmoFsmState<T>> m_States;
        private Dictionary<string, object> m_Data;
        private ImmoFsmState<T> m_CurrentState;
        private float m_CurrentStateTime;
        private bool m_IsDestroyed;


        /// <summary>
        /// Initializes a new instance of the finite state machine.
        /// </summary>
        public ImmoFsm()
        {
            m_Name = string.Empty;
            m_Owner = null;
            m_States = new Dictionary<Type, ImmoFsmState<T>>();
            m_CurrentState = null;
            m_CurrentStateTime = 0f;
            m_IsDestroyed = true;
        }


        /// <summary>
        /// Gets the finite state machine name.
        /// </summary>
        public string Name => m_Name;


        /// <summary>
        /// Gets the finite state machine owner.
        /// </summary>
        public T Owner => m_Owner;


        /// <summary>
        /// Gets the count of states in the finite state machine.
        /// </summary>
        public int FsmStateCount => m_States.Count;


        /// <summary>
        /// Gets whether the finite state machine is running.
        /// </summary>
        public bool IsRunning => m_CurrentState != null;


        /// <summary>
        /// Gets whether the finite state machine has been destroyed.
        /// </summary>
        public bool IsDestroyed => m_IsDestroyed;


        /// <summary>
        /// Gets the current finite state machine state.
        /// </summary>
        public ImmoFsmState<T> CurrentState => m_CurrentState;


        /// <summary>
        /// Gets the duration of the current finite state machine state.
        /// </summary>
        public float CurrentStateTime => m_CurrentStateTime;


        /// <summary>
        /// Creates a finite state machine.
        /// </summary>
        /// <param name="name">Finite state machine name.</param>
        /// <param name="owner">Finite state machine owner.</param>
        /// <param name="states">Finite state machine state collection.</param>
        /// <returns>The created finite state machine.</returns>
        public static ImmoFsm<T> Create(string name, T owner, params ImmoFsmState<T>[] states)
        {
            if (owner == null)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM owner is invalid.");
                return null;
            }

            if (states == null || states.Length < 1)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM states is invalid.");
                return null;
            }

            ImmoFsm<T> fsm = new ImmoFsm<T>();
            fsm.m_Name = name ?? string.Empty;
            fsm.m_Owner = owner;
            fsm.m_IsDestroyed = false;

            foreach (ImmoFsmState<T> state in states)
            {
                if (state == null)
                {
                    UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM states is invalid.");
                    return null;
                }

                Type stateType = state.GetType();
                if (fsm.m_States.ContainsKey(stateType))
                {
                    UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] FSM '{name}' state '{stateType.FullName}' is already exist.");
                    return null;
                }

                fsm.m_States.Add(stateType, state);
                state.OnInitialize(fsm);
            }

            return fsm;
        }


        /// <summary>
        /// Creates a finite state machine.
        /// </summary>
        /// <param name="name">Finite state machine name.</param>
        /// <param name="owner">Finite state machine owner.</param>
        /// <param name="states">Finite state machine state collection.</param>
        /// <returns>The created finite state machine.</returns>
        public static ImmoFsm<T> Create(string name, T owner, List<ImmoFsmState<T>> states)
        {
            if (owner == null)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM owner is invalid.");
                return null;
            }

            if (states == null || states.Count < 1)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM states is invalid.");
                return null;
            }

            ImmoFsm<T> fsm = new ImmoFsm<T>();
            fsm.m_Name = name ?? string.Empty;
            fsm.m_Owner = owner;
            fsm.m_IsDestroyed = false;

            foreach (ImmoFsmState<T> state in states)
            {
                if (state == null)
                {
                    UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM states is invalid.");
                    return null;
                }

                Type stateType = state.GetType();
                if (fsm.m_States.ContainsKey(stateType))
                {
                    UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] FSM '{name}' state '{stateType.FullName}' is already exist.");
                    return null;
                }

                fsm.m_States.Add(stateType, state);
                state.OnInitialize(fsm);
            }

            return fsm;
        }


        /// <summary>
        /// Clears the finite state machine.
        /// </summary>
        public void Clear()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnLeave(this, true);
            }

            foreach (KeyValuePair<Type, ImmoFsmState<T>> state in m_States)
            {
                state.Value.OnDispose(this);
            }

            m_Name = null;
            m_Owner = null;
            m_States.Clear();
            m_CurrentState = null;
            m_CurrentStateTime = 0f;
            m_IsDestroyed = true;
        }


        /// <summary>
        /// Starts the finite state machine.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to start.</typeparam>
        public void Start<TState>() where TState : ImmoFsmState<T>
        {
            if (IsRunning)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM is running, can not start again.");
                return;
            }

            ImmoFsmState<T> state = GetState<TState>();
            if (state == null)
            {
                UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] FSM '{m_Name}' can not start state '{typeof(TState).FullName}' which is not exist.");
                return;
            }

            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }


        /// <summary>
        /// Starts the finite state machine.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to start.</param>
        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] FSM is running, can not start again.");
                return;
            }

            if (stateType == null)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] State type is invalid.");
                return;
            }

            if (!typeof(ImmoFsmState<T>).IsAssignableFrom(stateType))
            {
                UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] State type '{stateType.FullName}' is invalid.");
                return;
            }

            ImmoFsmState<T> state = GetState(stateType);
            if (state == null)
            {
                UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] FSM '{m_Name}' can not start state '{stateType.FullName}' which is not exist.");
                return;
            }

            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }


        /// <summary>
        /// Checks whether a finite state machine state exists.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to check.</typeparam>
        /// <returns>Whether the finite state machine state exists.</returns>
        public bool HasState<TState>() where TState : ImmoFsmState<T>
        {
            return m_States.ContainsKey(typeof(TState));
        }


        /// <summary>
        /// Checks whether a finite state machine state exists.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to check.</param>
        /// <returns>Whether the finite state machine state exists.</returns>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] State type is invalid.");
                return false;
            }

            if (!typeof(ImmoFsmState<T>).IsAssignableFrom(stateType))
            {
                UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] State type '{stateType.FullName}' is invalid.");
                return false;
            }

            return m_States.ContainsKey(stateType);
        }


        /// <summary>
        /// Gets a finite state machine state.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to get.</typeparam>
        /// <returns>The finite state machine state to get.</returns>
        public TState GetState<TState>() where TState : ImmoFsmState<T>
        {
            ImmoFsmState<T> state = null;
            if (m_States.TryGetValue(typeof(TState), out state))
            {
                return (TState)state;
            }

            return null;
        }


        /// <summary>
        /// Gets a finite state machine state.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to get.</param>
        /// <returns>The finite state machine state to get.</returns>
        public ImmoFsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] State type is invalid.");
                return null;
            }

            if (!typeof(ImmoFsmState<T>).IsAssignableFrom(stateType))
            {
                UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] State type '{stateType.FullName}' is invalid.");
                return null;
            }

            ImmoFsmState<T> state = null;
            if (m_States.TryGetValue(stateType, out state))
            {
                return state;
            }

            return null;
        }


        /// <summary>
        /// Gets all states of the finite state machine.
        /// </summary>
        /// <returns>All states of the finite state machine.</returns>
        public ImmoFsmState<T>[] GetAllStates()
        {
            int index = 0;
            ImmoFsmState<T>[] results = new ImmoFsmState<T>[m_States.Count];
            foreach (KeyValuePair<Type, ImmoFsmState<T>> state in m_States)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        
        /// <summary>
        /// Gets a finite state machine data.
        /// </summary>
        /// <param name="name">The name of the data to get.</param>
        /// <returns>The data associated with the specified name.</returns>
        public object GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("[ImmoLiteFramework]-[StateMachine] Data name is invalid.");
                return null;
            }

            if (m_Data != null && m_Data.TryGetValue(name, out object data))
            {
                return data;
            }

            return null;
        }


        /// <summary>
        /// Sets a finite state machine data.
        /// </summary>
        /// <param name="name">The name of the data to set.</param>
        /// <param name="data">The data to set.</param>
        public void SetData(string name, object data)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("[ImmoLiteFramework]-[StateMachine] Data name is invalid.");
                return;
            }

            if (m_Data == null)
            {
                m_Data = new Dictionary<string, object>();
            }

            m_Data[name] = data;
        }

        
        /// <summary>
        /// Checks whether the finite state machine has a specific data.
        /// </summary>
        /// <param name="name">The name of the data to check.</param>
        /// <returns>True if the data exists; otherwise, false.</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("[ImmoLiteFramework]-[StateMachine] Data name is invalid.");
                return false;
            }

            return m_Data != null && m_Data.ContainsKey(name);
        }


        /// <summary>
        /// Finite state machine polling.
        /// </summary>
        /// <param name="elapseSeconds">Logical elapsed time in seconds.</param>
        /// <param name="realElapseSeconds">Real elapsed time in seconds.</param>
        internal void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_CurrentState == null)
            {
                return;
            }

            m_CurrentStateTime += elapseSeconds;
            m_CurrentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }


        /// <summary>
        /// Shuts down and clears the finite state machine.
        /// </summary>
        internal void Shutdown()
        {
            Clear();
        }


        /// <summary>
        /// Switches the current finite state machine state.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to switch to.</typeparam>
        internal void ChangeState<TState>() where TState : ImmoFsmState<T>
        {
            ChangeState(typeof(TState));
        }


        /// <summary>
        /// Switches the current finite state machine state.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to switch to.</param>
        internal void ChangeState(Type stateType)
        {
            if (m_CurrentState == null)
            {
                UnityEngine.Debug.LogError("[ImmoLiteFramework]-[StateMachine] Current state is invalid.");
                return;
            }

            ImmoFsmState<T> state = GetState(stateType);
            if (state == null)
            {
                UnityEngine.Debug.LogError($"[ImmoLiteFramework]-[StateMachine] FSM '{m_Name}' can not change state to '{stateType.FullName}' which is not exist.");
                return;
            }

            m_CurrentState.OnLeave(this, false);
            m_CurrentStateTime = 0f;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }
    }
}
