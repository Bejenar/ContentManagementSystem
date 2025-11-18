using System;
using System.Collections.Generic;
using System.Linq;
using src.Editor.CMSEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.CMSEditor
{
    public class CMSSelectorPopup : EditorWindow
    {
        private static CMSSelectorPopup _currentWindow;
        
        private List<CMSEntityPfb> _prefabs;
        private string _searchQuery = "";
        private Action<CMSEntityPfb> _onSelected;
        private CMSEntityPfb _currentSelected;
        private TextField _searchField;
        private ScrollView _scrollView;
        private VisualElement _contentContainer;

        public static void Show(List<CMSEntityPfb> prefabs, CMSEntityPfb currentSelection, Action<CMSEntityPfb> onSelected)
        {
            if (_currentWindow != null)
                _currentWindow.Close();
            
            var window = CreateInstance<CMSSelectorPopup>();
            window._prefabs = prefabs;
            window._currentSelected = currentSelection;
            window._onSelected = onSelected;
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            window.position = new Rect(mousePos.x - 100, mousePos.y, 400, 400);
            window.titleContent = new GUIContent("Select CMS Prefab");
            window.ShowPopup();
            
            _currentWindow = window;
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            // Load UXML
            var visualTree = EditorCustomTools.LoadUXML("CMSSelectorPopup");
            root.Add(visualTree);
            
            // Load stylesheet
            EditorCustomTools.LoadAndApplyStyleSheet(root, "CMSEditorStyles");

            // Get references
            _searchField = root.Q<TextField>("search-field");
            _scrollView = root.Q<ScrollView>("scroll-view");
            _contentContainer = root.Q<VisualElement>("content-container");
            var closeButton = root.Q<Button>("btn-close");

            // Setup event handlers
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _searchQuery = evt.newValue;
                RebuildList();
            });

            closeButton.clicked += Close;

            // Handle Escape key
            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            // Initial build
            RebuildList();
            
            // Focus search field
            _searchField.Focus();
        }

        private void RebuildList()
        {
            _contentContainer.Clear();

            var filtered = string.IsNullOrEmpty(_searchQuery)
                ? _prefabs
                : _prefabs.Where(p => p.name.ToLower().Contains(_searchQuery.ToLower())).ToList();

            foreach (var prefab in filtered)
            {
                CreateSelectableRow(prefab);
            }
        }

        private void CreateSelectableRow(CMSEntityPfb prefab)
        {
            var row = new VisualElement();
            row.AddToClassList("selector-row");
            
            if (_currentSelected == prefab)
            {
                row.AddToClassList("selector-row-selected");
            }

            var label = new Label(prefab.name);
            label.AddToClassList("template-label");
            row.Add(label);

            // Handle click
            row.RegisterCallback<MouseDownEvent>(evt =>
            {
                _onSelected?.Invoke(prefab);
                Close();
                evt.StopPropagation();
            });

            // Handle hover
            row.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (_currentSelected != prefab)
                {
                    row.AddToClassList("selector-row");
                }
            });

            row.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                if (_currentSelected != prefab)
                {
                    row.RemoveFromClassList("selector-row");
                }
            });

            _contentContainer.Add(row);
            
            // Add separator
            var separator = EditorCustomTools.CreateSeparator();
            _contentContainer.Add(separator);
        }
        
        private void OnDestroy()
        {
            if (_currentWindow == this)
            {
                _currentWindow = null;
            }
        }
    }
}