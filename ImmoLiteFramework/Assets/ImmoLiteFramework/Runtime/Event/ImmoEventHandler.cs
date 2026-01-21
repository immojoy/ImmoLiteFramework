
using System;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Defines the priority levels for event handlers.
    /// </summary>
    public enum ImmoEventHandlerPriority
    {
        Lowest = 0,
        Low = 1,
        Normal = 2,
        High = 3,
        Highest = 4,
    }


    internal interface IImmoEventHandler
    {
        ImmoEventHandlerPriority Priority { get; }
        Type EventType { get; }
        public void HandleEvent(ImmoEvent e);
    }


    internal interface IImmoEventHandler<T> : IImmoEventHandler where T : ImmoEvent
    {
        public void HandleEvent(T e);
    }


    public abstract class ImmoEventHandler<T> : IImmoEventHandler<T> where T : ImmoEvent
    {
        public virtual ImmoEventHandlerPriority Priority { get; } = ImmoEventHandlerPriority.Normal;
        public Type EventType => typeof(T);

        public abstract void HandleEvent(T e);

        public void HandleEvent(ImmoEvent e)
        {
            if (e is T typedEvent)
            {
                HandleEvent(typedEvent);
            }
        }
    }
}