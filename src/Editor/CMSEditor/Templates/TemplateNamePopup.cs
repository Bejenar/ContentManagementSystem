using System;
using src.Editor.CMSEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace src.Editor.CMSEditor.Templates
{
    public class TemplateNamePopup : EditorWindow
    {
        private string _templateName = "NewTemplate";
        private Action<string> _onConfirm;
        private TextField _nameField;

        public static void Show(Action<string> onConfirm)
        {
            var window = CreateInstance<TemplateNamePopup>();
            window.titleContent = new GUIContent("Save Template");
            window._onConfirm = onConfirm;
            window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 300, 100);
            window.ShowUtility();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            // Load UXML
            var visualTree = EditorCustomTools.LoadUXML("TemplateNamePopup");
            root.Add(visualTree);
            
            // Load stylesheet
            EditorCustomTools.LoadAndApplyStyleSheet(root, "CMSEditorStyles");

            // Get references
            _nameField = root.Q<TextField>("template-name-field");
            var saveButton = root.Q<Button>("btn-save");
            var cancelButton = root.Q<Button>("btn-cancel");

            // Set initial value
            _nameField.value = _templateName;

            // Setup event handlers
            saveButton.clicked += () =>
            {
                _onConfirm?.Invoke(_nameField.value);
                Close();
            };

            cancelButton.clicked += Close;

            // Handle Enter key
            _nameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    _onConfirm?.Invoke(_nameField.value);
                    Close();
                    evt.StopPropagation();
                }
            });

            // Handle Escape key
            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            // Focus the text field
            _nameField.Focus();
            _nameField.SelectAll();
        }
    }
}