using Cysharp.Threading.Tasks;

namespace Core.Services
{
    public interface ISceneLoaderService
    {
        /// <summary>
        /// Load scene additively
        /// </summary>
        UniTask LoadSceneAdditive(string sceneName);
        
        /// <summary>
        /// Unload scene
        /// </summary>
        UniTask UnloadScene(string sceneName);
        
        /// <summary>
        /// Load scene with transition
        /// </summary>
        UniTask LoadSceneWithTransition(string sceneName, float transitionDuration = 0.5f);
        
        /// <summary>
        /// Transition between scenes (unload old, load new)
        /// </summary>
        UniTask TransitionBetweenScenes(string fromScene, string toScene, float transitionDuration = 0.5f);
        
        /// <summary>
        /// Get currently loaded scenes
        /// </summary>
        string[] GetLoadedScenes();
        
        /// <summary>
        /// Is scene currently loaded
        /// </summary>
        bool IsSceneLoaded(string sceneName);
    }
}