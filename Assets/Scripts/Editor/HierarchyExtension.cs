#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class HierarchyExtension
    {
        /// <summary>
        /// Initializer <see cref="HierarchyExtension"/> class.
        /// </summary>
        static HierarchyExtension()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
        }

        /// <summary>
        /// Editor delegate callback
        /// </summary>
        /// <param name="instanceID">İnstance id.</param>
        /// <param name="selectionRect">Selection rect.</param>
        static void HierarchWindowOnGui(int instanceID, Rect selectionRect)
        {
            // make rectangle
            var r = new Rect(selectionRect)
            {
                x = 35,
                width = 18
            };

            // get objects
            var o = EditorUtility.InstanceIDToObject(instanceID);
            var g = o as GameObject;

            // drag toggle gui
            if(g == null) return;
            bool prev = g.activeSelf;
            g.SetActive(GUI.Toggle(r, g.activeSelf, string.Empty));

            if (g.activeSelf != prev)
            {
                EditorUtility.SetDirty(g);
            }
        }
    }
}

#endif