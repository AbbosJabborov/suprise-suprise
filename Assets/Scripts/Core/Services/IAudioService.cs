using UnityEngine;

namespace Core.Services
{
    public interface IAudioService
    {
        /// <summary>
        /// Play background music
        /// </summary>
        void PlayMusic(AudioClip clip, bool loop = true, float fadeInDuration = 0f);
        
        /// <summary>
        /// Stop current music
        /// </summary>
        void StopMusic(float fadeOutDuration = 0f);
        
        /// <summary>
        /// Play sound effect
        /// </summary>
        void PlaySFX(AudioClip clip, float volume = 1f);
        
        /// <summary>
        /// Play sound effect at position
        /// </summary>
        void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volume = 1f);
        
        /// <summary>
        /// Set master volume
        /// </summary>
        void SetMasterVolume(float volume);
        
        /// <summary>
        /// Set music volume
        /// </summary>
        void SetMusicVolume(float volume);
        
        /// <summary>
        /// Set SFX volume
        /// </summary>
        void SetSFXVolume(float volume);
    }
}