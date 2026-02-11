using System.IO;
using UnityEngine;

namespace Core.Services
{
    public class SaveService : ISaveService
    {
        private readonly string saveDirectory;
        
        public SaveService()
        {
            saveDirectory = Application.persistentDataPath + "/Saves/";
            
            // Create save directory if it doesn't exist
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            
            Debug.Log($"[SaveService] Save directory: {saveDirectory}");
        }
        
        public void Save<T>(string key, T data) where T : class
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                string filePath = GetFilePath(key);
                File.WriteAllText(filePath, json);
                
                Debug.Log($"[SaveService] Saved: {key}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveService] Failed to save {key}: {e.Message}");
            }
        }
        
        public T Load<T>(string key) where T : class
        {
            try
            {
                string filePath = GetFilePath(key);
                
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[SaveService] Save file not found: {key}");
                    return null;
                }
                
                string json = File.ReadAllText(filePath);
                T data = JsonUtility.FromJson<T>(json);
                
                Debug.Log($"[SaveService] Loaded: {key}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveService] Failed to load {key}: {e.Message}");
                return null;
            }
        }
        
        public bool HasSave(string key)
        {
            string filePath = GetFilePath(key);
            return File.Exists(filePath);
        }
        
        public void DeleteSave(string key)
        {
            try
            {
                string filePath = GetFilePath(key);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"[SaveService] Deleted: {key}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveService] Failed to delete {key}: {e.Message}");
            }
        }
        
        public void DeleteAllSaves()
        {
            try
            {
                if (Directory.Exists(saveDirectory))
                {
                    Directory.Delete(saveDirectory, true);
                    Directory.CreateDirectory(saveDirectory);
                    Debug.Log("[SaveService] All saves deleted");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveService] Failed to delete all saves: {e.Message}");
            }
        }
        
        private string GetFilePath(string key)
        {
            return Path.Combine(saveDirectory, $"{key}.json");
        }
    }
}