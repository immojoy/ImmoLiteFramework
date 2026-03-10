namespace Immojoy.LiteFramework.Runtime
{
    // An object can be an asset, a scene, or any other loadable resource. This callback structure is designed to be flexible and can be used for various types of objects.

    /// <summary>
    /// Callback delegate for successful object loading.
    /// </summary>
    /// <param name="objectName">The name of the object to be loaded.</param>
    /// <param name="obj">The loaded object.</param>
    /// <param name="duration">The duration of the loading process.</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnObjectLoadSuccess(string objectName, object obj, float duration, object userData);


    /// <summary>
    /// Callback delegate for object loading progress.
    /// </summary>
    /// <param name="objectName">The name of the object being loaded.</param>
    /// <param name="progress">The progress of the loading process (0 to 1).</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnObjectLoadProgress(string objectName, float progress, object userData);


    /// <summary>
    /// Callback delegate for failed object loading.
    /// </summary>
    /// <param name="objectName">The name of the object that failed to load.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnObjectLoadFailure(string objectName, string errorMessage, object userData);


    public struct OnObjectLoadCallback
    {
        public OnObjectLoadSuccess SuccessCallback;
        public OnObjectLoadProgress ProgressCallback;
        public OnObjectLoadFailure FailureCallback;
    }
    
}