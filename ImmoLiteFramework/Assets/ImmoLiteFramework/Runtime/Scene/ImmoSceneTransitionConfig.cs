using System.Collections.Generic;


namespace Immojoy.LiteFramework.Runtime
{
    public delegate void OnFadeInComplete();

    public delegate void OnFadeOutComplete();

    public delegate void OnSceneLoadProgress(string sceneName, float progress, float totalProgress);

    public delegate void OnSceneLoadSuccess(string sceneName, float loadDuration);

    public delegate void OnSceneLoadFail(string sceneName, string errorMessage);

    public delegate void OnSceneTransitionComplete();


    /// <summary>
    /// Configuration for a scene transition, specifying which scenes to load/unload
    /// and how the transition should be presented to the user.
    /// </summary>
    public class ImmoSceneTransitionConfig
    {
        /// <summary>
        /// Optional handler for visual transition effects (e.g., fade-to-black).
        /// </summary>
        public IImmoTransitionHandler TransitionHandler { get; set; }

        /// <summary>
        /// Optional handler for displaying a loading screen with progress.
        /// Only used when <see cref="UseLoadingScreen"/> is true.
        /// </summary>
        public IImmoLoadingHandler LoadingHandler { get; set; }

        /// <summary>
        /// Whether to show a loading screen during the transition.
        /// Requires <see cref="LoadingHandler"/> to be set.
        /// </summary>
        public bool UseLoadingScreen { get; set; } = false;

        /// <summary>
        /// When loading screen is not used, this sets the minimum display time
        /// for the transition cover, preventing abrupt transitions when loading is fast.
        /// </summary>
        public float MinTransitionTime { get; set; } = 0.3f;

        /// <summary>
        /// Addressable addresses of scenes to load.
        /// The first scene in the list will be set as the active scene.
        /// </summary>
        public List<string> ScenesToLoad { get; set; } = new();

        /// <summary>
        /// Addressable addresses of scenes to unload.
        /// Unloading occurs after new scenes are loaded and activated.
        /// </summary>
        public List<string> ScenesToUnload { get; set; } = new();

        /// <summary>
        /// Addressable addresses of additional assets to preload during the transition.
        /// </summary>
        public List<string> AssetsToPreload { get; set; } = new();


        #region Callbacks

        /// <summary>
        /// Callback invoked when the fade-in transition is complete.
        /// </summary>
        /// 
        public OnFadeInComplete FadeInCompleteCallback { get; set; }
        /// <summary>
        /// Callback invoked when the fade-out transition is complete.
        /// </summary>
        /// 
        public OnFadeOutComplete FadeOutCompleteCallback { get; set; }
        /// <summary>
        /// Callback invoked to report the progress of a scene load operation.
        /// </summary>
        /// 
        public OnSceneLoadProgress SceneLoadProgressCallback { get; set; }

        /// <summary>
        /// Callback invoked when a scene is successfully loaded.
        /// </summary>
        public OnSceneLoadSuccess SceneLoadSuccessCallback { get; set; }

        /// <summary>
        /// Callback invoked when a scene fails to load.
        /// </summary>
        public OnSceneLoadFail SceneLoadFailCallback { get; set; }

        /// <summary>
        /// Callback invoked when the entire scene transition process is complete (all scenes loaded/unloaded and transitions finished).
        /// </summary>
        public OnSceneTransitionComplete SceneTransitionCompleteCallback { get; set; }

        #endregion
    }
}