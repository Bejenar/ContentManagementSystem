using Editor.CMSEditor;
using src.Editor.CMSEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace src.Editor.CMSEditor
{
    public class CMSEntityInspectorWindow : EditorWindow
    {
        private Object _target;
        private CMSEntityExplorer _explorer;
        private int _selectedId;
        private VisualElement _inspectorContent;
        
        public static void ShowWindow(Object target, Rect anchorRect, CMSEntityExplorer explorer, int selectedId)
        {
            var window = CreateInstance<CMSEntityInspectorWindow>();
            window._target = target;
            window._explorer = explorer;
            window._selectedId = selectedId;
            window.titleContent = new GUIContent(target.name);
            window.position = new Rect(anchorRect.xMin - 400 - 10, anchorRect.yMin, 600, anchorRect.height);
            window.ShowUtility();
            window.Focus();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            // Load UXML
            var visualTree = EditorCustomTools.LoadUXML("CMSEntityInspectorWindow");
            root.Add(visualTree);
            
            // Load stylesheet
            EditorCustomTools.LoadAndApplyStyleSheet(root, "CMSEditorStyles");

            // Get reference to inspector content
            _inspectorContent = root.Q<VisualElement>("inspector-content");

            // Handle Escape key to close
            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            // Build the inspector
            BuildInspector();
        }

        private void BuildInspector()
        {
            _inspectorContent.Clear();

            if (_target == null)
            {
                var helpBox = new HelpBox("No target to inspect.", HelpBoxMessageType.Warning);
                _inspectorContent.Add(helpBox);
                return;
            }

            // Create an InspectorElement to display the target's inspector
            var editor = UnityEditor.Editor.CreateEditor(_target);
            var inspectorElement = new InspectorElement(editor);
            _inspectorContent.Add(inspectorElement);
        }
        
        private void OnDestroy()
        {
            if (_explorer != null && _selectedId != -1)
            {
                _explorer.FocusTreeViewAndReselect(_selectedId);
            }
        }
    }
}