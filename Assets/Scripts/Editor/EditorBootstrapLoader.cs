#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [InitializeOnLoad]
    public static class BootstrapSceneLoader
    {
        private const string BootstrapScenePath = "Assets/_Scenes/Bootstrap.unity"; 
        private const string BootstrapSceneName = "Bootstrap";
        private const string GameSceneName = "Game";

        static BootstrapSceneLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (SceneManager.GetActiveScene().name != BootstrapSceneName)
                {
                    var bootstrapSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);
                    if (bootstrapSceneAsset != null)
                    {
                        EditorSceneManager.playModeStartScene = bootstrapSceneAsset;
                    }
                    else
                    {
                        Debug.LogError($"Bootstrap scene asset not found at path: {BootstrapScenePath}");
                    }
                }
                else
                {
                    EditorSceneManager.playModeStartScene = null;
                }
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}
#endif