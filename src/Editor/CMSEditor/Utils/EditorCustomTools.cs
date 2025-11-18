using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace src.Editor.CMSEditor.Utils
{
    public static class EditorCustomTools
    {
        /// <summary>
        /// Creates a visual element with a border (UI Toolkit version)
        /// </summary>
        public static VisualElement CreateBorderedContainer()
        {
            var container = new VisualElement();
            container.AddToClassList("cms-window-border");
            return container;
        }

        /// <summary>
        /// Creates a separator line (UI Toolkit version)
        /// </summary>
        public static VisualElement CreateSeparator()
        {
            var separator = new VisualElement();
            separator.AddToClassList("cms-separator");
            return separator;
        }

        /// <summary>
        /// Legacy IMGUI method - kept for backwards compatibility during migration
        /// </summary>
        [System.Obsolete("Use CreateBorderedContainer() for UI Toolkit instead")]
        public static void DrawWindowBorder(this EditorWindow editorWindow)
        {
            var borderColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            var thickness = 1f;
            var rect = new Rect(0, 0, editorWindow.position.width, editorWindow.position.height);

            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), borderColor); // top
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), borderColor); // bottom
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), borderColor); // left
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), borderColor); // right
        }

        /// <summary>
        /// Legacy IMGUI method - kept for backwards compatibility during migration
        /// </summary>
        [System.Obsolete("Use CreateSeparator() for UI Toolkit instead")]
        public static void DrawLineBetween(this EditorWindow editorWindow)
        {
            var lineRect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(lineRect, new Color(0.25f, 0.25f, 0.25f, 1f));
        }

        /// <summary>
        /// Load and apply stylesheet to a visual element
        /// </summary>
        public static void LoadAndApplyStyleSheet(VisualElement root, string styleSheetName)
        {
            var styleSheet = Resources.Load<StyleSheet>(styleSheetName);
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }
            else
            {
                Debug.LogWarning($"Could not load stylesheet: {styleSheetName}");
            }
        }

        /// <summary>
        /// Load UXML and clone it into a root element
        /// </summary>
        public static VisualElement LoadUXML(string uxmlName)
        {
            var visualTree = Resources.Load<VisualTreeAsset>(uxmlName);
            if (visualTree != null)
            {
                return visualTree.CloneTree();
            }
            else
            {
                Debug.LogWarning($"Could not load UXML: {uxmlName}");
                return new VisualElement();
            }
        }
    }
}