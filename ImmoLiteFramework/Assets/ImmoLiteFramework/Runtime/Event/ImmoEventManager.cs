
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Immojoy/Lite Framework/Manager/Immo Event Manager")]
    public sealed class ImmoEventManager : MonoBehaviour
    {
        private static ImmoEventManager m_Instance;
        public static ImmoEventManager Instance => m_Instance;


        private readonly Dictionary<Type, List<IImmoEventHandler>> m_EventHandlers = new();
        private readonly Queue<ImmoEvent> m_EventQueue = new();
        private readonly object m_Lock = new();



        /// <summary>
        /// Registers an event handler for a specific event type.
        /// </summary>
        /// <param name="handler">The event handler to register.</param>
        public void RegisterHandler<T>(ImmoEventHandler<T> handler) where T : ImmoEvent
        {
            Type eventType = typeof(T);
            if (!m_EventHandlers.ContainsKey(eventType))
            {
                m_EventHandlers[eventType] = new List<IImmoEventHandler>();
            }

            if (!m_EventHandlers[eventType].Contains(handler))
            {
                m_EventHandlers[eventType].Add(handler);
                m_EventHandlers[eventType].Sort((a, b) => b.Priority.CompareTo(a.Priority));
            }
        }


        /// <summary>
        /// Unregisters an event handler for a specific event type.
        /// </summary>
        /// <param name="handler">The event handler to unregister.</param>
        public void UnregisterHandler<T>(ImmoEventHandler<T> handler) where T : ImmoEvent
        {
            Type eventType = typeof(T);
            if (m_EventHandlers.ContainsKey(eventType))
            {
                m_EventHandlers[eventType].Remove(handler);
                if (m_EventHandlers[eventType].Count == 0)
                {
                    m_EventHandlers.Remove(eventType);
                }
            }
        }


        /// <summary>
        /// Triggers an event to be processed.</br>
        /// This method queues the event for processing in the next update cycle.
        /// </summary>
        /// <param name="e">The event to trigger.</param>
        public void TriggerEvent<T>(T e) where T : ImmoEvent
        {
            if (e == null || e.IsCancelled)
            {
                return;
            }

            lock (m_Lock)
            {
                m_EventQueue.Enqueue(e);
            }
        }


        /// <summary>
        /// Triggers an event to be processed immediately.
        /// </summary>
        /// <param name="e">The event to trigger.</param>
        public void TriggerEventImmediately<T>(T e) where T : ImmoEvent
        {
            if (e == null || e.IsCancelled)
            {
                return;
            }

            ProcessEvent(e);
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
            }
        }


        /// <summary>
        /// Updates the event module, processing all queued events.
        /// </summary>
        private void Update()
        {
            while (true)
            {
                ImmoEvent e = null;
                lock (m_Lock)
                {
                    if (m_EventQueue.Count > 0)
                    {
                        e = m_EventQueue.Dequeue();
                    }
                }

                if (e == null)
                {
                    break;
                }

                ProcessEvent(e);
            }
        }
        

        /// <summary>
        /// Shuts down the event module, clearing all queued events and handlers.
        /// </summary>
        private void Destroy()
        {
            lock (m_Lock)
            {
                m_EventQueue.Clear();
                m_EventHandlers.Clear();
            }
        }



        private void ProcessEvent(ImmoEvent e)
        {
            Type eventType = e.GetType();
            if (m_EventHandlers.ContainsKey(eventType))
            {
                List<IImmoEventHandler> handlers = new List<IImmoEventHandler>();
                handlers.AddRange(m_EventHandlers[eventType]);

                foreach (IImmoEventHandler handler in handlers)
                {
                    if (e.IsCancelled)
                    {
                        break;
                    }
                    handler.HandleEvent(e);
                }
            }
        }
    }
}