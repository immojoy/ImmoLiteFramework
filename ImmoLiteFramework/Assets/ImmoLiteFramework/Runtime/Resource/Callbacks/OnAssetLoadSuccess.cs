
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
}