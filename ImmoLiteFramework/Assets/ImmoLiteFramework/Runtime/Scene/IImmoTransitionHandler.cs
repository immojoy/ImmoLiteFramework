using System.Threading.Tasks;


namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Interface for handling scene transition visual effects (e.g., fade-to-black).
    /// </summary>
    public interface IImmoTransitionHandler
    {
        /// <summary>
        /// Fades in the transition cover (screen becomes obscured).
        /// </summary>
        Task FadeInAsync();

        /// <summary>
        /// Fades out the transition cover (screen becomes visible).
        /// </summary>
        Task FadeOutAsync();
    }
}