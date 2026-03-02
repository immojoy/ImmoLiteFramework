
namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Callback delegate for successful asset loading.
    /// </summary>
    /// <param name="assetName">The name of the asset to be loaded.</param>
    /// <param name="asset">The loaded asset.</param>
    /// <param name="duration">The duration of the loading process.</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnAssetLoadSuccess(string assetName, object asset, float duration, object userData);


    /// <summary>
    /// Callback delegate for asset loading progress.
    /// </summary>
    /// <param name="assetName">The name of the asset being loaded.</param>
    /// <param name="progress">The progress of the loading process (0 to 1).</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnAssetLoadProgress(string assetName, float progress, object userData);


    /// <summary>
    /// Callback delegate for failed asset loading.
    /// </summary>
    /// <param name="assetName">The name of the asset that failed to load.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <param name="userData">User-defined data.</param>
    public delegate void OnAssetLoadFailure(string assetName, string errorMessage, object userData);


    public struct OnAssetLoadCallback
    {
        public OnAssetLoadSuccess SuccessCallback;
        public OnAssetLoadProgress ProgressCallback;
        public OnAssetLoadFailure FailureCallback;
    }
}