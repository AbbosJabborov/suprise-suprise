// ==================== DIALOGUE SERVICE ====================

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Services {
    public interface IDialogueService
    {
        /// <summary>
        /// Show dialogue and wait for completion
        /// </summary>
        UniTask ShowDialogue(DialogueData dialogue);
        
        /// <summary>
        /// Skip current dialogue
        /// </summary>
        void Skip();
        
        /// <summary>
        /// Is dialogue currently playing
        /// </summary>
        bool IsDialoguePlaying { get; }
    }
    
    [Serializable]
    public class DialogueData
    {
        public string characterName;
        public Sprite characterPortrait;
        public string[] lines;
        public AudioClip voiceClip;
    }
}