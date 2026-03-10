using System.Threading.Tasks;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Interface for handling loading screen display during scene transitions.
    /// </summary>
    public interface IImmoLoadingHandler
    {
        /// <summary>
        /// Shows the loading screen.
        /// </summary>
        Task ShowLoadingScreenAsync();

        /// <summary>
        /// Hides the loading screen.
        /// </summary>
        Task HideLoadingScreenAsync();

        /// <summary>
        /// Updates the loading progress.
        /// </summary>
        /// <param name="progress">Progress value from 0 to 1.</param>
        void UpdateLoadingProgress(float progress);
    }
}