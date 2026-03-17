
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Event triggered when a fade-in transition completes.
    /// </summary>
    public sealed class ImmmoFadeInCompleteEvent : ImmoEvent
    {
        public ImmmoFadeInCompleteEvent(object source) : base(source) { }
    }


    /// <summary>
    /// Event triggered when a fade-out transition completes.
    /// </summary>
    public sealed class ImmmoFadeOutCompleteEvent : ImmoEvent
    {
        public ImmmoFadeOutCompleteEvent(object source) : base(source) { }
    }


    /// <summary>
    /// Event triggered to report the progress of a scene load operation.
    /// </summary>
    public sealed class ImmoSceneLoadProgressEvent : ImmoEvent
    {
        /// <summary>
        /// The scene currently being loaded
        /// </summary>
        public string SceneName { get; }
        /// <summary>
        /// The progress of the current scene load operation (0 to 1)
        /// </summary>
        public float Progress { get; }
        /// <summary>
        /// The total progress of all scene load operations (0 to 1)
        /// </summary>
        public float TotalProgress { get; }

        public ImmoSceneLoadProgressEvent(object source, string sceneName, float progress, float totalProgress) : base(source)
        {
            SceneName = sceneName;
            Progress = progress;
            TotalProgress = totalProgress;
        }
    }


    /// <summary>
    /// Event triggered when a scene is successfully loaded.
    /// </summary>
    public sealed class ImmoSceneLoadSuccessEvent : ImmoEvent
    {
        /// <summary>
        /// The name of the scene that was successfully loaded.
        /// </summary>
        public string SceneName { get; }

        /// <summary>
        /// The duration it took to load the scene.
        /// </summary>
        public float LoadDuration { get; }

        public ImmoSceneLoadSuccessEvent(object source, string sceneName, float loadDuration) : base(source)
        {
            SceneName = sceneName;
            LoadDuration = loadDuration;
        }
    }


    /// <summary>
    /// Event triggered when a scene fails to load.
    /// </summary>
    public sealed class ImmoSceneLoadFailureEvent : ImmoEvent
    {
        /// <summary>
        /// The name of the scene that failed to load.
        /// </summary>
        public string SceneName { get; }

        /// <summary>
        /// The error message associated with the scene load failure.
        /// </summary>
        public string ErrorMessage { get; }

        public ImmoSceneLoadFailureEvent(object source, string sceneName, string errorMessage) : base(source)
        {
            SceneName = sceneName;
            ErrorMessage = errorMessage;
        }
    }


    /// <summary>
    /// Event triggered when a scene transition completes.
    /// </summary>
    public sealed class ImmoSceneTransitionCompleteEvent : ImmoEvent
    {
        public ImmoSceneTransitionCompleteEvent(object source) : base(source) { }
    }
}