using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Core.Services
{
    // ==================== SAVE SERVICE ====================
    
    public interface ISaveService
    {
        /// <summary>
        /// Save data to disk
        /// </summary>
        void Save<T>(string key, T data) where T : class;
        
        /// <summary>
        /// Load data from disk
        /// </summary>
        T Load<T>(string key) where T : class;
        
        /// <summary>
        /// Check if save exists
        /// </summary>
        bool HasSave(string key);
        
        /// <summary>
        /// Delete save
        /// </summary>
        void DeleteSave(string key);
        
        /// <summary>
        /// Delete all saves
        /// </summary>
        void DeleteAllSaves();
    }

}