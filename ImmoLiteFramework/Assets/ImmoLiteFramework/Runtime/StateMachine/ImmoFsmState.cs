
namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Finite state machine state base class.
    /// </summary>
    /// <typeparam name="T">Finite state machine owner type.</typeparam>
    public abstract class ImmoFsmState<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the finite state machine state base class.
        /// </summary>
        public ImmoFsmState() { }


        /// <summary>
        /// Called when finite state machine state is initialized.
        /// </summary>
        /// <param name="fsm">Finite state machine reference.</param>
        protected internal virtual void OnInit(IImmoFsm<T> fsm) { }


        /// <summary>
        /// Called when entering finite state machine state.
        /// </summary>
        /// <param name="fsm">Finite state machine reference.</param>
        protected internal virtual void OnEnter(IImmoFsm<T> fsm) { }


        /// <summary>
        /// Called when finite state machine state is polled.
        /// </summary>
        /// <param name="fsm">Finite state machine reference.</param>
        /// <param name="elapseSeconds">Logical elapsed time in seconds.</param>
        /// <param name="realElapseSeconds">Real elapsed time in seconds.</param>
        protected internal virtual void OnUpdate(IImmoFsm<T> fsm, float elapseSeconds, float realElapseSeconds) { }


        /// <summary>
        /// Called when leaving finite state machine state.
        /// </summary>
        /// <param name="fsm">Finite state machine reference.</param>
        /// <param name="isShutdown">Whether triggered by shutting down the finite state machine.</param>
        protected internal virtual void OnLeave(IImmoFsm<T> fsm, bool isShutdown) { }


        /// <summary>
        /// Called when finite state machine state is destroyed.
        /// </summary>
        /// <param name="fsm">Finite state machine reference.</param>
        protected internal virtual void OnDestroy(IImmoFsm<T> fsm) { }


        /// <summary>
        /// Switch current finite state machine state.
        /// </summary>
        /// <typeparam name="TState">State type to switch to.</typeparam>
        /// <param name="fsm">Finite state machine reference.</param>
        protected void ChangeState<TState>(IImmoFsm<T> fsm) where TState : ImmoFsmState<T>
        {
            ImmoFsm<T> fsmImplement = (ImmoFsm<T>)fsm;
            if (fsmImplement == null)
            {
                UnityEngine.Debug.LogError("FSM is invalid.");
                return;
            }

            fsmImplement.ChangeState<TState>();
        }


        /// <summary>
        /// Switch current finite state machine state.
        /// </summary>
        /// <param name="fsm">Finite state machine reference.</param>
        /// <param name="stateType">State type to switch to.</param>
        protected void ChangeState(IImmoFsm<T> fsm, System.Type stateType)
        {
            ImmoFsm<T> fsmImplement = (ImmoFsm<T>)fsm;
            if (fsmImplement == null)
            {
                UnityEngine.Debug.LogError("FSM is invalid.");
                return;
            }

            if (stateType == null)
            {
                UnityEngine.Debug.LogError("State type is invalid.");
                return;
            }

            if (!typeof(ImmoFsmState<T>).IsAssignableFrom(stateType))
            {
                UnityEngine.Debug.LogError($"State type '{stateType.FullName}' is invalid.");
                return;
            }

            fsmImplement.ChangeState(stateType);
        }
    }
}
