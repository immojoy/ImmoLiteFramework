
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Finite state machine manager.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Immojoy/Lite Framework/Manager/Immo Fsm Manager")]
    public sealed class ImmoFsmManager : MonoBehaviour
    {
        private static ImmoFsmManager m_Instance;
        public static ImmoFsmManager Instance => m_Instance;


        private readonly Dictionary<string, object> m_Fsms = new();
        private readonly List<object> m_TempFsms = new();


        /// <summary>
        /// Gets the count of finite state machines.
        /// </summary>
        public int Count => m_Fsms.Count;


        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this);
                return;
            }

            m_Instance = this;
        }


        private void Update()
        {
            m_TempFsms.Clear();
            if (m_Fsms.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<string, object> fsm in m_Fsms)
            {
                m_TempFsms.Add(fsm.Value);
            }

            float elapseSeconds = Time.deltaTime;
            float realElapseSeconds = Time.unscaledDeltaTime;

            foreach (object fsm in m_TempFsms)
            {
                if (fsm == null)
                {
                    continue;
                }

                System.Type fsmType = fsm.GetType();
                System.Reflection.MethodInfo updateMethod = fsmType.GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (updateMethod != null)
                {
                    updateMethod.Invoke(fsm, new object[] { elapseSeconds, realElapseSeconds });
                }
            }
        }


        private void OnDestroy()
        {
            m_TempFsms.Clear();
            foreach (KeyValuePair<string, object> fsm in m_Fsms)
            {
                if (fsm.Value == null)
                {
                    continue;
                }

                System.Type fsmType = fsm.Value.GetType();
                System.Reflection.MethodInfo shutdownMethod = fsmType.GetMethod("Shutdown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (shutdownMethod != null)
                {
                    shutdownMethod.Invoke(fsm.Value, null);
                }
            }

            m_Fsms.Clear();
        }


        /// <summary>
        /// Checks whether a finite state machine exists.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <returns>Whether the finite state machine exists.</returns>
        public bool HasFsm<T>() where T : class
        {
            return HasFsm<T>(string.Empty);
        }


        /// <summary>
        /// Checks whether a finite state machine exists.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="name">Finite state machine name.</param>
        /// <returns>Whether the finite state machine exists.</returns>
        public bool HasFsm<T>(string name) where T : class
        {
            string fullName = GetFullName<T>(name);
            return m_Fsms.ContainsKey(fullName);
        }


        /// <summary>
        /// Gets a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <returns>The finite state machine to get.</returns>
        public IImmoFsm<T> GetFsm<T>() where T : class
        {
            return GetFsm<T>(string.Empty);
        }


        /// <summary>
        /// Gets a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="name">Finite state machine name.</param>
        /// <returns>The finite state machine to get.</returns>
        public IImmoFsm<T> GetFsm<T>(string name) where T : class
        {
            string fullName = GetFullName<T>(name);
            if (m_Fsms.TryGetValue(fullName, out object fsm))
            {
                return (IImmoFsm<T>)fsm;
            }

            return null;
        }


        /// <summary>
        /// Creates a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="owner">Finite state machine owner.</param>
        /// <param name="states">Finite state machine state collection.</param>
        /// <returns>The finite state machine to create.</returns>
        public IImmoFsm<T> CreateFsm<T>(T owner, params ImmoFsmState<T>[] states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }


        /// <summary>
        /// Creates a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="name">Finite state machine name.</param>
        /// <param name="owner">Finite state machine owner.</param>
        /// <param name="states">Finite state machine state collection.</param>
        /// <returns>The finite state machine to create.</returns>
        public IImmoFsm<T> CreateFsm<T>(string name, T owner, params ImmoFsmState<T>[] states) where T : class
        {
            string fullName = GetFullName<T>(name);
            if (HasFsm<T>(name))
            {
                Debug.LogError($"Already exist FSM '{fullName}'.");
                return null;
            }

            ImmoFsm<T> fsm = ImmoFsm<T>.Create(name, owner, states);
            if (fsm == null)
            {
                return null;
            }

            m_Fsms.Add(fullName, fsm);
            return fsm;
        }


        /// <summary>
        /// Creates a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="owner">Finite state machine owner.</param>
        /// <param name="states">Finite state machine state collection.</param>
        /// <returns>The finite state machine to create.</returns>
        public IImmoFsm<T> CreateFsm<T>(T owner, List<ImmoFsmState<T>> states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }


        /// <summary>
        /// Creates a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="name">Finite state machine name.</param>
        /// <param name="owner">Finite state machine owner.</param>
        /// <param name="states">Finite state machine state collection.</param>
        /// <returns>The finite state machine to create.</returns>
        public IImmoFsm<T> CreateFsm<T>(string name, T owner, List<ImmoFsmState<T>> states) where T : class
        {
            string fullName = GetFullName<T>(name);
            if (HasFsm<T>(name))
            {
                Debug.LogError($"Already exist FSM '{fullName}'.");
                return null;
            }

            ImmoFsm<T> fsm = ImmoFsm<T>.Create(name, owner, states);
            if (fsm == null)
            {
                return null;
            }

            m_Fsms.Add(fullName, fsm);
            return fsm;
        }


        /// <summary>
        /// Destroys a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <returns>Whether the finite state machine was successfully destroyed.</returns>
        public bool DestroyFsm<T>() where T : class
        {
            return DestroyFsm<T>(string.Empty);
        }


        /// <summary>
        /// Destroys a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="name">Finite state machine name.</param>
        /// <returns>Whether the finite state machine was successfully destroyed.</returns>
        public bool DestroyFsm<T>(string name) where T : class
        {
            string fullName = GetFullName<T>(name);
            if (m_Fsms.TryGetValue(fullName, out object fsm))
            {
                if (fsm != null)
                {
                    System.Type fsmType = fsm.GetType();
                    System.Reflection.MethodInfo shutdownMethod = fsmType.GetMethod("Shutdown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (shutdownMethod != null)
                    {
                        shutdownMethod.Invoke(fsm, null);
                    }
                }

                return m_Fsms.Remove(fullName);
            }

            return false;
        }


        /// <summary>
        /// Destroys a finite state machine.
        /// </summary>
        /// <typeparam name="T">Finite state machine owner type.</typeparam>
        /// <param name="fsm">The finite state machine to destroy.</param>
        /// <returns>Whether the finite state machine was successfully destroyed.</returns>
        public bool DestroyFsm<T>(IImmoFsm<T> fsm) where T : class
        {
            if (fsm == null)
            {
                Debug.LogError("FSM is invalid.");
                return false;
            }

            return DestroyFsm<T>(fsm.Name);
        }


        private string GetFullName<T>(string name) where T : class
        {
            Type ownerType = typeof(T);
            string typeName = ownerType.FullName;
            return string.IsNullOrEmpty(name) ? typeName : $"{typeName}.{name}";
        }
    }
}
