
namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Finite state machine interface.
    /// </summary>
    /// <typeparam name="T">Finite state machine owner type.</typeparam>
    public interface IImmoFsm<T> where T : class
    {
        /// <summary>
        /// Gets the finite state machine name.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets the finite state machine owner.
        /// </summary>
        T Owner { get; }


        /// <summary>
        /// Gets the count of states in the finite state machine.
        /// </summary>
        int FsmStateCount { get; }


        /// <summary>
        /// Gets whether the finite state machine is running.
        /// </summary>
        bool IsRunning { get; }


        /// <summary>
        /// Gets whether the finite state machine has been destroyed.
        /// </summary>
        bool IsDestroyed { get; }


        /// <summary>
        /// Gets the current finite state machine state.
        /// </summary>
        ImmoFsmState<T> CurrentState { get; }


        /// <summary>
        /// Gets the duration of the current finite state machine state.
        /// </summary>
        float CurrentStateTime { get; }


        /// <summary>
        /// Starts the finite state machine.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to start.</typeparam>
        void Start<TState>() where TState : ImmoFsmState<T>;


        /// <summary>
        /// Starts the finite state machine.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to start.</param>
        void Start(System.Type stateType);


        /// <summary>
        /// Checks whether a finite state machine state exists.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to check.</typeparam>
        /// <returns>Whether the finite state machine state exists.</returns>
        bool HasState<TState>() where TState : ImmoFsmState<T>;


        /// <summary>
        /// Checks whether a finite state machine state exists.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to check.</param>
        /// <returns>Whether the finite state machine state exists.</returns>
        bool HasState(System.Type stateType);


        /// <summary>
        /// Gets a finite state machine state.
        /// </summary>
        /// <typeparam name="TState">Finite state machine state type to get.</typeparam>
        /// <returns>The finite state machine state to get.</returns>
        TState GetState<TState>() where TState : ImmoFsmState<T>;


        /// <summary>
        /// Gets a finite state machine state.
        /// </summary>
        /// <param name="stateType">Finite state machine state type to get.</param>
        /// <returns>The finite state machine state to get.</returns>
        ImmoFsmState<T> GetState(System.Type stateType);


        /// <summary>
        /// Gets all states of the finite state machine.
        /// </summary>
        /// <returns>All states of the finite state machine.</returns>
        ImmoFsmState<T>[] GetAllStates();
    }
}
