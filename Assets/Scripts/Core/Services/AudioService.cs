using UnityEngine;
using System.Collections;

namespace Core.Services
{
    public class AudioService : IAudioService
    {
        private readonly AudioSource musicSource;
        private readonly AudioSource sfxSource;
        private readonly GameObject audioObject;
        
        private float masterVolume = 1f;
        private float musicVolume = 0.7f;
        private float sfxVolume = 1f;
        
        public AudioService()
        {
            // Create persistent audio object
            audioObject = new GameObject("AudioService");
            Object.DontDestroyOnLoad(audioObject);
            
            // Create music source
            musicSource = audioObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume * masterVolume;
            
            // Create SFX source
            sfxSource = audioObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume * masterVolume;
            
            Debug.Log("[AudioService] Initialized");
        }
        
        public void PlayMusic(AudioClip clip, bool loop = true, float fadeInDuration = 0f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioService] Attempted to play null music clip");
                return;
            }
            
            musicSource.loop = loop;
            
            if (fadeInDuration > 0f)
            {
                CoroutineRunner.Instance.StartCoroutine(FadeInMusic(clip, fadeInDuration));
            }
            else
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();
            }
            
            Debug.Log($"[AudioService] Playing music: {clip.name}");
        }
        
        public void StopMusic(float fadeOutDuration = 0f)
        {
            if (fadeOutDuration > 0f)
            {
                CoroutineRunner.Instance.StartCoroutine(FadeOutMusic(fadeOutDuration));
            }
            else
            {
                musicSource.Stop();
            }
            
            Debug.Log("[AudioService] Stopped music");
        }
        
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioService] Attempted to play null SFX clip");
                return;
            }
            
            sfxSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
        
        public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioService] Attempted to play null SFX clip at point");
                return;
            }
            
            AudioSource.PlayClipAtPoint(clip, position, volume * sfxVolume * masterVolume);
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        private void UpdateVolumes()
        {
            musicSource.volume = musicVolume * masterVolume;
            sfxSource.volume = sfxVolume * masterVolume;
        }
        
        private IEnumerator FadeInMusic(AudioClip clip, float duration)
        {
            musicSource.clip = clip;
            musicSource.volume = 0f;
            musicSource.Play();
            
            float elapsed = 0f;
            float targetVolume = musicVolume * masterVolume;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
                yield return null;
            }
            
            musicSource.volume = targetVolume;
        }
        
        private IEnumerator FadeOutMusic(float duration)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
            
            musicSource.Stop();
            musicSource.volume = musicVolume * masterVolume;
        }
    }
    
    // Simple coroutine runner for services
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
    }
}