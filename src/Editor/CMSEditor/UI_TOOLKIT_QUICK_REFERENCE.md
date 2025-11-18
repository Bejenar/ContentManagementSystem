# UI Toolkit Quick Reference for CMSEditor

## For Developers Maintaining This Code

### Understanding the Architecture

The CMSEditor now uses **UI Toolkit (UIElements)** instead of IMGUI. Here's what you need to know:

## File Structure

```
CMSEditor/
├── Resources/                          # UI Resources
│   ├── CMSEditorStyles.uss            # Stylesheet (like CSS)
│   ├── CMSEntityExplorer.uxml         # Explorer window layout
│   ├── CMSEntityInspectorWindow.uxml  # Inspector layout
│   ├── CMSSelectorPopup.uxml          # Selector popup layout
│   └── TemplateNamePopup.uxml         # Template naming layout
├── Editor Windows (use CreateGUI())
│   ├── CMSEntityExplorer.cs
│   ├── CMSEntityInspectorWindow.cs
│   ├── CMSSelectorPopup.cs
│   └── Templates/
│       ├── TemplateNamePopup.cs
│       └── TemplateDropdownWindow.cs
├── Custom Editors (use CreateInspectorGUI())
│   ├── CMSEntityEditor.cs
│   └── CMSEntityPfbEditor.cs
├── Property Drawers (use CreatePropertyGUI())
│   ├── CMSEntityPfbDrawer.cs
│   └── EntityComponentDefinitionDrawer.cs
└── Utils/
    └── EditorCustomTools.cs            # Helper methods
```

## Common Tasks

### Adding a New Window

```csharp
public class MyWindow : EditorWindow
{
    private void CreateGUI()
    {
        var root = rootVisualElement;
        
        // Option 1: Load from UXML
        var visualTree = EditorCustomTools.LoadUXML("MyWindow");
        root.Add(visualTree);
        
        // Option 2: Build programmatically
        var button = new Button(() => Debug.Log("Clicked!"))
        {
            text = "Click Me"
        };
        root.Add(button);
        
        // Apply stylesheet
        EditorCustomTools.LoadAndApplyStyleSheet(root, "CMSEditorStyles");
    }
}
```

### Creating a Custom Editor

```csharp
[CustomEditor(typeof(MyType))]
public class MyEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        
        // Add property fields
        var myProperty = serializedObject.FindProperty("myField");
        var field = new PropertyField(myProperty);
        root.Add(field);
        
        return root;
    }
}
```

### Creating a Property Drawer

```csharp
[CustomPropertyDrawer(typeof(MyType))]
public class MyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        
        var label = new Label(property.displayName);
        var field = new PropertyField(property, "");
        
        container.Add(label);
        container.Add(field);
        
        return container;
    }
}
```

## Styling

### Using USS Classes

```csharp
// Add a CSS class to an element
element.AddToClassList("my-custom-style");

// Define in CMSEditorStyles.uss
// .my-custom-style {
//     background-color: rgba(64, 128, 255, 0.5);
//     padding: 4px;
// }
```

### Inline Styles

```csharp
element.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
element.style.paddingLeft = 10;
element.style.flexGrow = 1;
element.style.flexDirection = FlexDirection.Row;
```

## Event Handling

### Keyboard Events

```csharp
root.RegisterCallback<KeyDownEvent>(evt =>
{
    if (evt.keyCode == KeyCode.Escape)
    {
        Close();
        evt.StopPropagation();
    }
});
```

### Mouse Events

```csharp
element.RegisterCallback<MouseDownEvent>(evt =>
{
    Debug.Log("Clicked!");
    evt.StopPropagation();
});

element.RegisterCallback<MouseEnterEvent>(evt =>
{
    element.style.backgroundColor = Color.gray;
});

element.RegisterCallback<MouseLeaveEvent>(evt =>
{
    element.style.backgroundColor = Color.clear;
});
```

### Button Clicks

```csharp
var button = new Button(() => DoSomething())
{
    text = "Click Me"
};

// Or later
button.clicked += () => DoSomethingElse();
```

## Working with Lists and Trees

### ListView

```csharp
var listView = new ListView
{
    makeItem = () => new Label(),
    bindItem = (element, index) =>
    {
        ((Label)element).text = items[index].name;
    },
    itemsSource = items,
    selectionType = SelectionType.Single,
    fixedItemHeight = 32
};

listView.onSelectionChange += (selectedItems) =>
{
    // Handle selection
};

listView.onItemsChosen += (chosenItems) =>
{
    // Handle double-click
};
```

### TreeView

```csharp
var treeItems = BuildTreeItems(); // Returns List<TreeViewItemData<T>>

var treeView = new TreeView
{
    fixedItemHeight = 32,
    makeItem = MakeTreeItem,
    bindItem = BindTreeItem
};

treeView.SetRootItems(treeItems);
treeView.Rebuild();
```

## Loading Resources

### Load UXML

```csharp
var visualTree = Resources.Load<VisualTreeAsset>("MyWindow");
var root = visualTree.CloneTree();
```

### Load USS

```csharp
var styleSheet = Resources.Load<StyleSheet>("CMSEditorStyles");
root.styleSheets.Add(styleSheet);
```

### Query Elements

```csharp
// By name
var button = root.Q<Button>("my-button-name");

// By class
var elements = root.Query<Button>(className: "my-class").ToList();

// By type
var allButtons = root.Query<Button>().ToList();
```

## Common Patterns

### Creating a Labeled Field

```csharp
var container = new VisualElement();
container.style.flexDirection = FlexDirection.Row;
container.style.alignItems = Align.Center;

var label = new Label("My Field:");
label.style.minWidth = 120;

var field = new TextField();
field.style.flexGrow = 1;

container.Add(label);
container.Add(field);
```

### Creating a Toolbar

```csharp
var toolbar = new VisualElement();
toolbar.AddToClassList("cms-toolbar");
toolbar.style.flexDirection = FlexDirection.Row;

var button1 = new Button(OnButton1Clicked) { text = "Action 1" };
var button2 = new Button(OnButton2Clicked) { text = "Action 2" };

toolbar.Add(button1);
toolbar.Add(button2);
```

### Creating a Scrollable Container

```csharp
var scrollView = new ScrollView();
scrollView.style.flexGrow = 1;

var content = new VisualElement();
// Add items to content

scrollView.Add(content);
```

## Debugging Tips

1. **Use UI Debugger**: Window > UI Toolkit > Debugger
2. **Check element hierarchy**: The debugger shows all visual elements
3. **Inspect styles**: See which USS rules are applied
4. **Test responsiveness**: Resize windows to test layout

## Migration Checklist

When converting IMGUI code to UI Toolkit:

- [ ] Replace `OnGUI()` with `CreateGUI()`
- [ ] Replace `OnInspectorGUI()` with `CreateInspectorGUI()`
- [ ] Replace `OnGUI(Rect, SerializedProperty, GUIContent)` with `CreatePropertyGUI(SerializedProperty)`
- [ ] Convert `EditorGUILayout.BeginHorizontal()` to `flexDirection = Row`
- [ ] Convert `EditorGUILayout.BeginVertical()` to `flexDirection = Column` (default)
- [ ] Replace `GUILayout.Button()` with `Button`
- [ ] Replace `EditorGUILayout.TextField()` with `TextField`
- [ ] Replace `EditorGUILayout.PropertyField()` with `PropertyField`
- [ ] Replace `Event.current` with specific event callbacks
- [ ] Move styles from `GUIStyle` to USS files
- [ ] Test all functionality thoroughly

## Resources

- **Unity Documentation**: https://docs.unity3d.com/Manual/UIElements.html
- **USS Properties**: https://docs.unity3d.com/Manual/UIE-USS-Properties-Reference.html
- **UXML Elements**: https://docs.unity3d.com/Manual/UIE-ElementRef.html
- **API Reference**: https://docs.unity3d.com/ScriptReference/UIElements.html

## Support

For questions or issues with the UI Toolkit implementation, refer to:
1. This quick reference
2. UI_TOOLKIT_MIGRATION.md for detailed migration notes
3. Unity's UI Toolkit documentation
4. Existing code in CMSEditor for examples

