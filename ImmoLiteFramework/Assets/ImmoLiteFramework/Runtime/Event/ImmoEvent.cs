
using System;


namespace Immojoy.LiteFramework.Runtime
{
    public abstract class ImmoEvent
    {
        /// <summary>
        /// Indicates whether the event has been cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }


        /// <summary>
        /// The time when the event was created.
        /// </summary>
        public DateTime Timestamp { get; private set; }


        /// <summary>
        /// The source object that triggered the event.
        /// </summary>
        public object Source { get; private set; }


        protected ImmoEvent(object source)
        {
            Source = source;
            Timestamp = DateTime.Now;
            IsCancelled = false;
        }


        /// <summary>
        /// Cancels the event if it is cancellable.
        /// </summary>
        public void Cancel()
        {
            if (!IsCancellable())
            {
                return;
            }
            IsCancelled = true;
        }


        /// <summary>
        /// Determines whether the event can be cancelled.    
        /// </summary>
        /// <returns><b>true</b> if the event can be cancelled; otherwise, <b>false</b>.</returns>
        protected virtual bool IsCancellable()
        {
            return true;
        }
    }
}