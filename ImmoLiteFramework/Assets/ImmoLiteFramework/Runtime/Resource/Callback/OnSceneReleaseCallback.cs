namespace Immojoy.LiteFramework.Runtime
{

    /// <summary>
    /// Callback delegate for successful scene release.
    /// </summary>
    /// <param name="sceneName">The name of the scene to be released.</param>
    /// <param name="duration">The duration of the release process.</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnSceneReleaseSuccess(string sceneName, float duration, object userData);
    

    /// <summary>
    /// Callback delegate for scene release progress.
    /// </summary>
    /// <param name="sceneName">The name of the scene being released.</param>
    /// <param name="progress">The progress of the release process (0 to 1).</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnSceneReleaseProgress(string sceneName, float progress, object userData);


    /// <summary>
    /// Callback delegate for failed scene release.
    /// </summary>
    /// <param name="sceneName">The name of the scene that failed to release.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnSceneReleaseFailure(string sceneName, string errorMessage, object userData);


    public struct OnSceneReleaseCallback
    {
        public OnSceneReleaseSuccess SuccessCallback;
        public OnSceneReleaseProgress ProgressCallback;
        public OnSceneReleaseFailure FailureCallback;
    }
    
}