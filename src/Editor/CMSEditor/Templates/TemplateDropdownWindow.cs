using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.CMSEditor;
using src.Editor.CMSEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace src.Editor.CMSEditor.Templates
{
    public class TemplateDropdownWindow : EditorWindow
    {
        private string _templatesFolder => CMSEntityExplorer.TemplatesFolder;
        private Action<string> _onTemplateSelected;
        private List<TemplateEntry> _entries;
        private ScrollView _scrollView;
        private VisualElement _contentContainer;
        private static Rect _alignTo;
        
        private class TemplateEntry
        {
            public string name;
            public Texture2D icon;
        }

        public static void Show(Rect alignTo, Action<string> onTemplateSelected)
        {
            var window = CreateInstance<TemplateDropdownWindow>();
            window._onTemplateSelected = onTemplateSelected;
            window.LoadTemplates();
            Resize(alignTo, window);
        }

        private static void Resize(Rect alignTo, TemplateDropdownWindow window)
        {
            _alignTo = alignTo;
            var height = CalculateWindowSize(window);
            window.ShowAsDropDown(alignTo, new Vector2(250, height));
        }

        private static float CalculateWindowSize(TemplateDropdownWindow window)
        {
            const float rowHeight = 25f;
            const int maxVisibleRows = 10;
            var count = window._entries?.Count ?? 0;
            var visibleRows = Mathf.Min(count, maxVisibleRows);
            var height = visibleRows * rowHeight + 20; // Add padding
            return Mathf.Max(height, 60); // Minimum height for "no templates" message
        }

        private void LoadTemplates()
        {
            if (!Directory.Exists(_templatesFolder))
                Directory.CreateDirectory(_templatesFolder);

            _entries = new List<TemplateEntry>();

            foreach (var path in Directory.GetFiles(_templatesFolder, "*.json"))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                _entries.Add(new TemplateEntry { name = fileName});
            }
        }
        
        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            // Create a bordered container
            var container = EditorCustomTools.CreateBorderedContainer();
            container.style.flexGrow = 1;
            root.Add(container);
            
            // Load stylesheet
            EditorCustomTools.LoadAndApplyStyleSheet(root, "CMSEditorStyles");

            if (_entries == null || _entries.Count == 0)
            {
                ShowNoTemplatesMessage(container);
                return;
            }

            _scrollView = new ScrollView();
            _scrollView.style.flexGrow = 1;
            container.Add(_scrollView);

            _contentContainer = new VisualElement();
            _scrollView.Add(_contentContainer);

            BuildTemplateList();
        }

        private void ShowNoTemplatesMessage(VisualElement container)
        {
            var label = new Label("No saved templates");
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            label.style.color = new Color(0.6f, 0.6f, 0.6f);
            label.style.flexGrow = 1;
            label.style.marginTop = 10;
            container.Add(label);
        }

        private void BuildTemplateList()
        {
            _contentContainer.Clear();

            foreach (var entry in _entries.ToList())
            {
                var row = CreateTemplateRow(entry);
                _contentContainer.Add(row);
                
                // Add separator
                var separator = EditorCustomTools.CreateSeparator();
                _contentContainer.Add(separator);
            }
        }

        private VisualElement CreateTemplateRow(TemplateEntry entry)
        {
            var row = new VisualElement();
            row.AddToClassList("template-item");
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.paddingLeft = 8;
            row.style.paddingRight = 8;
            row.style.paddingTop = 4;
            row.style.paddingBottom = 4;

            // Icon
            var prefabIcon = EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D;
            var icon = new Image { image = prefabIcon };
            icon.AddToClassList("template-icon");
            icon.style.width = 20;
            icon.style.height = 20;
            icon.style.marginRight = 8;
            row.Add(icon);

            // Label
            var label = new Label(entry.name);
            label.AddToClassList("template-label");
            label.style.flexGrow = 1;
            row.Add(label);

            // Delete button
            var deleteButton = new Button(() => DeleteTemplate(entry))
            {
                text = "×"
            };
            deleteButton.AddToClassList("cms-clear-button");
            deleteButton.style.width = 16;
            deleteButton.style.height = 16;
            row.Add(deleteButton);

            // Handle hover
            row.RegisterCallback<MouseEnterEvent>(evt =>
            {
                row.style.backgroundColor = new Color(0.2f, 0.4f, 0.6f, 0.3f);
            });

            row.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                row.style.backgroundColor = Color.clear;
            });

            // Handle click
            row.RegisterCallback<MouseDownEvent>(evt =>
            {
                _onTemplateSelected?.Invoke(entry.name);
                Close();
                evt.StopPropagation();
            });

            return row;
        }

        private void DeleteTemplate(TemplateEntry entry)
        {
            if (EditorUtility.DisplayDialog("Delete Template", $"Delete template '{entry.name}'?", "Yes", "Cancel"))
            {
                var path = Path.Combine(_templatesFolder, $"{entry.name}.json");
                File.Delete(path);
                AssetDatabase.Refresh();
                _entries.Remove(entry);
                
                if (_entries.Count == 0)
                {
                    // Rebuild UI to show "no templates" message
                    rootVisualElement.Clear();
                    CreateGUI();
                }
                else
                {
                    BuildTemplateList();
                }
            }
        }
    }
}