using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Core.Services
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly IAudioService audioService;
        
        public SceneLoaderService(IAudioService audioService)
        {
            this.audioService = audioService;
        }
        
        public async UniTask LoadSceneAdditive(string sceneName)
        {
            Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
            
            if (IsSceneLoaded(sceneName))
            {
                Debug.LogWarning($"[SceneLoader] Scene {sceneName} is already loaded");
                return;
            }
            
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            Debug.Log($"[SceneLoader] Scene loaded: {sceneName}");
        }
        
        public async UniTask UnloadScene(string sceneName)
        {
            Debug.Log($"[SceneLoader] Unloading scene: {sceneName}");
            
            if (!IsSceneLoaded(sceneName))
            {
                Debug.LogWarning($"[SceneLoader] Scene {sceneName} is not loaded");
                return;
            }
            
            await SceneManager.UnloadSceneAsync(sceneName);
            
            await Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            Debug.Log($"[SceneLoader] Scene unloaded: {sceneName}");
        }
        
        public async UniTask LoadSceneWithTransition(string sceneName, float transitionDuration = 0.5f)
        {
            // Fade out
            await FadeOut(transitionDuration);
            
            // Load scene
            await LoadSceneAdditive(sceneName);
            
            // Fade in
            await FadeIn(transitionDuration);
        }
        
        public async UniTask TransitionBetweenScenes(string fromScene, string toScene, float transitionDuration = 0.5f)
        {
            Debug.Log($"[SceneLoader] Transitioning from {fromScene} to {toScene}");
            
            // Fade out
            await FadeOut(transitionDuration);
            
            // Unload old scene
            if (!string.IsNullOrEmpty(fromScene) && IsSceneLoaded(fromScene))
            {
                await UnloadScene(fromScene);
            }
            
            // Load new scene
            await LoadSceneAdditive(toScene);
            
            // Fade in
            await FadeIn(transitionDuration);
            
            Debug.Log($"[SceneLoader] Transition complete");
        }
        
        public string[] GetLoadedScenes()
        {
            List<string> loadedScenes = new List<string>();
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    loadedScenes.Add(scene.name);
                }
            }
            
            return loadedScenes.ToArray();
        }
        
        public bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
        
        private async UniTask FadeOut(float duration)
        {
            // Fade out music
            audioService.StopMusic(duration);
            
            // TODO: Fade out screen (implement FadeCanvas or use DOTween)
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
        }
        
        private async UniTask FadeIn(float duration)
        {
            // TODO: Fade in screen
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
        }
    }
}