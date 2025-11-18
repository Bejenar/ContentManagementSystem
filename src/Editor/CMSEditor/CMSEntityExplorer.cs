using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using src.Editor.CMSEditor;
using src.Editor.CMSEditor.Templates;
using src.Editor.CMSEditor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.CMSEditor
{
    public enum ViewModeExplorer
    {
        DefaultView = 0,
        SearchView = 1
    }
    
    public class CMSEntityExplorer : EditorWindow
    {
        public const string TemplatesFolder = $"{CMSPaths.CMSRoot}/Templates";
        private const string SearchPath = $"{CMSPaths.ResourcesRoot}";

        private string _searchQuery = "";
        private List<SearchResult> _searchResults = new List<SearchResult>();
        private ViewModeExplorer _viewMode;
        
        // UI Toolkit elements
        private TextField _searchField;
        private Button _clearSearchButton;
        private VisualElement _treeContainer;
        private ListView _listView;
        private TreeView _treeView;
        
        private int _selectedIndex = -1;

        [MenuItem("CMS/CMS Entity Explorer #&c")]
        public static void ShowWindow()
        {
            var window = GetWindow<CMSEntityExplorer>();
            window.titleContent = new GUIContent("CMS Entity Explorer");
            window.Show();
        }

        private void OnEnable()
        {
            CMS.Init();
            _viewMode = ViewModeExplorer.DefaultView;
            PerformSearch();
            EditorApplication.projectChanged += OnProjectChanged;
        }
        
        private void OnDisable()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
        }
        
        private void OnProjectChanged()
        {
            if (_viewMode == ViewModeExplorer.DefaultView)
            {
                PerformSearch();
            }
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            // Load UXML
            var visualTree = EditorCustomTools.LoadUXML("CMSEntityExplorer");
            root.Add(visualTree);
            
            // Load stylesheet
            EditorCustomTools.LoadAndApplyStyleSheet(root, "CMSEditorStyles");

            // Get UI references
            _searchField = root.Q<TextField>("search-field");
            _clearSearchButton = root.Q<Button>("btn-clear-search");
            _treeContainer = root.Q<VisualElement>("tree-container");
            
            var addButton = root.Q<Button>("btn-add");
            var deleteButton = root.Q<Button>("btn-delete");
            var useTemplateButton = root.Q<Button>("btn-use-template");
            var saveTemplateButton = root.Q<Button>("btn-save-template");

            // Setup event handlers
            _searchField.RegisterValueChangedCallback(OnSearchChanged);
            _clearSearchButton.clicked += OnClearSearch;
            addButton.clicked += AddNewEntityFromSelection;
            deleteButton.clicked += DeleteSelectedEntity;
            useTemplateButton.clicked += BuildTemplateMenu;
            saveTemplateButton.clicked += OnSaveTemplate;

            // Handle Escape key
            root.RegisterCallback<KeyDownEvent>(OnKeyDown);

            // Build the tree/list view
            RebuildTreeView();
            
            // Focus search field initially
            _searchField.Focus();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                Close();
                evt.StopPropagation();
            }
            else if (evt.keyCode == KeyCode.DownArrow && _searchField.focusController?.focusedElement == _searchField)
            {
                // Move focus to tree when pressing down from search
                if (_viewMode == ViewModeExplorer.SearchView && _listView != null)
                {
                    _listView.Focus();
                    _listView.selectedIndex = 0;
                }
                else if (_treeView != null)
                {
                    _treeView.Focus();
                }
                evt.StopPropagation();
            }
        }

        private void OnSearchChanged(ChangeEvent<string> evt)
        {
            _searchQuery = evt.newValue;
            _clearSearchButton.style.display = string.IsNullOrEmpty(_searchQuery) ? DisplayStyle.None : DisplayStyle.Flex;
            PerformSearch();
            RebuildTreeView();
        }

        private void OnClearSearch()
        {
            _searchField.value = "";
            _searchField.Focus();
        }

        private void OnSaveTemplate()
        {
            var selected = GetSelectedEntity();
            if (selected != null)
            {
                SaveTemplate(selected);
            }
        }

        private void RebuildTreeView()
        {
            _treeContainer.Clear();

            if (_viewMode == ViewModeExplorer.SearchView)
            {
                BuildListView();
            }
            else
            {
                BuildTreeView();
            }
        }

        private void BuildListView()
        {
            _listView = new ListView
            {
                makeItem = MakeListItem,
                bindItem = BindListItem,
                itemsSource = _searchResults,
                selectionType = SelectionType.Single,
                fixedItemHeight = 32
            };
            
            _listView.style.flexGrow = 1;
            _listView.onSelectionChange += OnListSelectionChanged;
            _listView.onItemsChosen += OnListItemDoubleClicked;
            
            _treeContainer.Add(_listView);
        }

        private VisualElement MakeListItem()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingLeft = 4;
            container.style.paddingRight = 4;
            container.AddToClassList("entity-tree-item");

            var icon = new Image();
            icon.AddToClassList("entity-icon");
            icon.style.width = 28;
            icon.style.height = 28;
            icon.style.marginRight = 4;

            var label = new Label();
            label.AddToClassList("entity-label");

            container.Add(icon);
            container.Add(label);

            return container;
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (index < 0 || index >= _searchResults.Count) return;

            var result = _searchResults[index];
            var icon = element.Q<Image>();
            var label = element.Q<Label>();

            label.text = result.displayName;
            
            if (result.sprite != null)
            {
                icon.sprite = result.sprite;
                icon.style.display = DisplayStyle.Flex;
            }
            else
            {
                icon.style.display = DisplayStyle.None;
            }
        }

        private void OnListSelectionChanged(IEnumerable<object> selectedItems)
        {
            var selected = selectedItems.FirstOrDefault();
            if (selected != null)
            {
                var result = selected as SearchResult;
                if (result != null)
                {
                    EditorGUIUtility.PingObject(result.prefab);
                }
            }
        }

        private void OnListItemDoubleClicked(IEnumerable<object> selectedItems)
        {
            var selected = selectedItems.FirstOrDefault() as SearchResult;
            if (selected != null)
            {
                Selection.activeObject = selected.prefab;
                EditorUtility.OpenPropertyEditor(selected.entity);
            }
        }

        private void BuildTreeView()
        {
            // Build tree structure
            var treeItems = BuildTreeItems();
            
            _treeView = new TreeView
            {
                fixedItemHeight = 32,
                makeItem = MakeTreeItem,
                bindItem = BindTreeItem
            };
            
            _treeView.SetRootItems(treeItems);
            _treeView.style.flexGrow = 1;
            _treeView.Rebuild();
            
            // Expand all by default
            foreach (var item in treeItems)
            {
                _treeView.ExpandItem(item.id);
            }
            
            _treeContainer.Add(_treeView);
        }

        private List<TreeViewItemData<EntityTreeData>> BuildTreeItems()
        {
            var root = new List<TreeViewItemData<EntityTreeData>>();
            var pathToItem = new Dictionary<string, TreeViewItemData<EntityTreeData>>();
            var idCounter = 1;

            foreach (var result in _searchResults)
            {
                var assetPath = AssetDatabase.GetAssetPath(result.prefab);
                var relativePath = assetPath.Replace(CMSPaths.CMSPrefabs, "").Replace(".prefab", "");
                var parts = relativePath.Split('/');

                var currentPath = "";
                List<TreeViewItemData<EntityTreeData>> currentList = root;
                TreeViewItemData<EntityTreeData>? parentItem = null;

                for (var i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}/{part}";

                    if (!pathToItem.TryGetValue(currentPath, out var item))
                    {
                        var isLeaf = i == parts.Length - 1;
                        var data = new EntityTreeData
                        {
                            displayName = isLeaf ? result.displayName : part,
                            isFolder = !isLeaf,
                            sprite = isLeaf ? result.sprite : null,
                            prefab = isLeaf ? result.prefab : null,
                            entity = isLeaf ? result.entity : null,
                            path = assetPath
                        };

                        if (isLeaf)
                        {
                            item = new TreeViewItemData<EntityTreeData>(idCounter++, data);
                        }
                        else
                        {
                            var children = new List<TreeViewItemData<EntityTreeData>>();
                            item = new TreeViewItemData<EntityTreeData>(idCounter++, data, children);
                        }

                        pathToItem[currentPath] = item;
                        currentList.Add(item);
                    }

                    if (!item.data.isFolder)
                    {
                        currentList = null;
                    }
                    else
                    {
                        currentList = (List<TreeViewItemData<EntityTreeData>>)item.children;
                    }
                }
            }

            return root;
        }

        private VisualElement MakeTreeItem()
        {
            return MakeListItem();
        }

        private void BindTreeItem(VisualElement element, int index)
        {
            var itemData = _treeView.GetItemDataForIndex<EntityTreeData>(index);
            if (itemData == null) return;

            var icon = element.Q<Image>();
            var label = element.Q<Label>();

            label.text = itemData.displayName;
            
            if (itemData.isFolder)
            {
                // Show folder icon
                var folderIcon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
                icon.image = folderIcon;
                icon.style.width = 16;
                icon.style.height = 16;
                icon.style.display = DisplayStyle.Flex;
            }
            else if (itemData.sprite != null)
            {
                icon.sprite = itemData.sprite;
                icon.image = null;
                icon.style.width = 28;
                icon.style.height = 28;
                icon.style.display = DisplayStyle.Flex;
            }
            else
            {
                icon.style.display = DisplayStyle.None;
            }
        }

        private CMSEntityPfb GetSelectedEntity()
        {
            if (_viewMode == ViewModeExplorer.SearchView && _listView != null)
            {
                var index = _listView.selectedIndex;
                if (index >= 0 && index < _searchResults.Count)
                {
                    return _searchResults[index].entity;
                }
            }
            else if (_treeView != null)
            {
                var selected = _treeView.selectedItem;
                if (selected != null)
                {
                    var data = (EntityTreeData)selected;
                    return data.entity;
                }
            }
            return null;
        }

        private void AddNewEntityFromSelection()
        {
            string folderPath = CMSPaths.CMSRoot;

            if (_treeView != null && _treeView.selectedItem != null)
            {
                var data = (EntityTreeData)_treeView.selectedItem;
                if (data.prefab != null)
                {
                    var prefabPath = AssetDatabase.GetAssetPath(data.prefab);
                    folderPath = Path.GetDirectoryName(prefabPath);
                }
                else if (data.isFolder && !string.IsNullOrEmpty(data.path))
                {
                    folderPath = Path.GetDirectoryName(data.path);
                }
            }

            AddNewEntity(folderPath);
        }
        
        private void AddNewEntity(string folderPath)
        {
            var path = folderPath;
            var baseName = "NewEntity";
            var counter = 1;

            while (AssetDatabase.LoadAssetAtPath<GameObject>($"{path}/{baseName}{counter}.prefab") != null)
            {
                counter++;
            }

            var finalName = $"{baseName}{counter}";
            var assetPath = $"{path}/{finalName}.prefab";

            var go = new GameObject(finalName);
            var entity = go.AddComponent<CMSEntityPfb>();
            entity.name = finalName;
            CMSEntityIdSetter.UpdateEntityId(entity, assetPath);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, assetPath);
            EditorUtility.SetDirty(prefab);
            DestroyImmediate(go);

            AssetDatabase.Refresh();
            PerformSearch();
            RebuildTreeView();
        }

        private void DeleteSelectedEntity()
        {
            var entity = GetSelectedEntity();
            if (entity == null) return;

            var assetPath = AssetDatabase.GetAssetPath(entity);

            if (!EditorUtility.DisplayDialog("Delete Entity",
                    $"Are you sure you want to delete '{entity.name}'?", "Yes", "Cancel"))
                return;

            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.Refresh();
            PerformSearch();
            RebuildTreeView();
        }
        
        private void BuildTemplateMenu()
        {
            var buttonRect = _treeContainer.worldBound;
            var rect = new Rect(buttonRect.x + 90, buttonRect.y + 20, buttonRect.width, 0);
            TemplateDropdownWindow.Show(rect, templateName =>
            {
                var path = Path.Combine(TemplatesFolder, $"{templateName}.json");
                ApplyTemplateFromPath(path);
            });
        }
        
        private void ApplyTemplateFromPath(string path)
        {
            var json = File.ReadAllText(path);
            var template = JsonUtility.FromJson<EntityTemplate>(json);
            if (template == null) return;

            var folder = GetTargetFolderOrDefault();
            var baseName = template.templateName;
            var finalName = baseName;
            var counter = 1;

            while (AssetDatabase.LoadAssetAtPath<GameObject>($"{folder}/{finalName}.prefab") != null)
            {
                finalName = $"{baseName}{counter}";
                counter++;
            }

            var go = new GameObject(finalName);
            var entity = go.AddComponent<CMSEntityPfb>();
            entity.name = finalName;
            entity.Components = new List<EntityComponentDefinition>();

            foreach (var ser in template.components)
            {
                var type = Type.GetType(ser.type);
                if (type == null)
                {
                    Debug.LogWarning($"Unknown component type: {ser.type}");
                    continue;
                }

                var instance = (EntityComponentDefinition)JsonUtility.FromJson(ser.jsonData, type);
                entity.Components.Add(instance);
            }

            var prefabPath = $"{folder}/{finalName}.prefab";
            CMSEntityIdSetter.UpdateEntityId(entity, prefabPath);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            EditorUtility.SetDirty(prefab);
            DestroyImmediate(go);

            AssetDatabase.Refresh();
            PerformSearch();
            RebuildTreeView();
        }
        
        private string GetTargetFolderOrDefault()
        {
            if (_treeView != null && _treeView.selectedItem != null)
            {
                var data = (EntityTreeData)_treeView.selectedItem;
                if (data.prefab != null)
                {
                    return Path.GetDirectoryName(AssetDatabase.GetAssetPath(data.prefab));
                }
                else if (data.isFolder && !string.IsNullOrEmpty(data.path))
                {
                    var folderPath = Path.GetDirectoryName(data.path);
                    if (AssetDatabase.IsValidFolder(folderPath))
                        return folderPath;
                }
            }

            return CMSPaths.CMSRoot;
        }
        
        private void SaveTemplate(CMSEntityPfb entity)
        {
            if (!Directory.Exists(TemplatesFolder))
                Directory.CreateDirectory(TemplatesFolder);

            TemplateNamePopup.Show(templateName =>
            {
                var path = Path.Combine(TemplatesFolder, $"{templateName}.json");

                var template = new EntityTemplate
                {
                    templateName = templateName,
                    components = new List<SerializableComponent>()
                };
                
                foreach (var component in entity.Components)
                {
                    var type = component.GetType();
                    var json = JsonUtility.ToJson(component);

                    template.components.Add(new SerializableComponent
                    {
                        type = type.AssemblyQualifiedName,
                        jsonData = json
                    });
                }

                var jsonResult = JsonUtility.ToJson(template, true);
                File.WriteAllText(path, jsonResult);
                AssetDatabase.Refresh();
            });
        }

        public void FocusTreeViewAndReselect(int id)
        {
            Focus();
            // TODO: Implement selection by ID in UI Toolkit TreeView
        }

        private void PerformSearch()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] {SearchPath});
            var results = new List<SearchResult>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    var cmsEntity = prefab.GetComponent<CMSEntityPfb>();

                    if (cmsEntity != null)
                    {
                        if (string.IsNullOrEmpty(_searchQuery) ||
                            (cmsEntity.name.ToLower().Contains(_searchQuery.ToLower())))
                        {
                            results.Add(new SearchResult
                            {
                                prefab = prefab,
                                entity = cmsEntity,
                                displayName = $"{prefab.name}",
                                sprite = cmsEntity.GetSprite()
                            });
                        }
                    }
                }
            }

            _searchResults = results;
            _viewMode = !string.IsNullOrEmpty(_searchQuery) ? ViewModeExplorer.SearchView : ViewModeExplorer.DefaultView;
        }

        public void OnDestroy()
        {
            CMSMenuItems.CMSReload();
        }
    }

    public class SearchResult
    {
        public GameObject prefab;
        public CMSEntityPfb entity;
        public string displayName;
        public Sprite sprite;
    }

    public class EntityTreeData
    {
        public string displayName;
        public bool isFolder;
        public Sprite sprite;
        public GameObject prefab;
        public CMSEntityPfb entity;
        public string path;
    }
}