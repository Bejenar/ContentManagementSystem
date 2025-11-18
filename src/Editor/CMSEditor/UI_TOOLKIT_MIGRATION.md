# UI Toolkit Migration - CMSEditor

## Overview
This document describes the migration of the CMSEditor from IMGUI to UI Toolkit (UIElements), completed as part of the modernization effort for Unity 2021.3+ compatibility.

## Migration Summary

### Files Refactored

#### Editor Windows
1. **CMSEntityExplorer.cs** - Main entity browser window
   - Migrated from `OnGUI()` to `CreateGUI()`
   - Replaced IMGUI TreeView with UI Toolkit `TreeView` and `ListView`
   - Integrated search functionality with UI Toolkit TextField
   - Toolbar buttons now use UI Toolkit Button elements

2. **CMSEntityInspectorWindow.cs** - Entity inspector popup
   - Migrated from `OnGUI()` to `CreateGUI()`
   - Uses `InspectorElement` for rendering target properties
   - Keyboard shortcuts (Escape) handled via UI Toolkit events

3. **CMSSelectorPopup.cs** - Entity selection popup
   - Migrated from `OnGUI()` to `CreateGUI()`
   - Interactive list items with hover effects via USS classes
   - Search filtering with live updates

4. **TemplateNamePopup.cs** - Template naming dialog
   - Migrated from `OnGUI()` to `CreateGUI()`
   - Form-based UI with TextField and Buttons
   - Enter/Escape key handling

5. **TemplateDropdownWindow.cs** - Template selection dropdown
   - Migrated from `OnGUI()` to `CreateGUI()`
   - Dynamic list building with delete functionality
   - Hover effects and click handling

#### Custom Editors
1. **CMSEntityEditor.cs**
   - Migrated from `OnInspectorGUI()` to `CreateInspectorGUI()`
   - Component list with foldouts and delete buttons
   - Dynamic rebuilding on component add/remove

2. **CMSEntityPfbEditor.cs**
   - Migrated from `OnInspectorGUI()` to `CreateInspectorGUI()`
   - Sprite preview using UI Toolkit `Image` element
   - Uses `InspectorElement.FillDefaultInspector()` for remaining properties

#### Property Drawers
1. **CMSEntityPfbDrawer.cs**
   - Migrated from `OnGUI()` to `CreatePropertyGUI()`
   - Horizontal layout with ObjectField and selector button
   - Integrated with CMSSelectorPopup

2. **EntityComponentDefinitionDrawer.cs**
   - Migrated from `OnGUI()` to `CreatePropertyGUI()`
   - Simple PropertyField wrapper for all component types

#### Utility Classes
1. **EditorCustomTools.cs**
   - Added UI Toolkit helper methods
   - Legacy IMGUI methods marked as `[Obsolete]`
   - New methods:
     - `CreateBorderedContainer()` - Creates bordered visual elements
     - `CreateSeparator()` - Creates separator lines
     - `LoadAndApplyStyleSheet()` - Loads USS files
     - `LoadUXML()` - Loads UXML templates

2. **GlobalStyles.cs**
   - Kept for backwards compatibility
   - Styles now defined in CMSEditorStyles.uss

#### Deprecated Files
1. **EntityTreeView.cs**
   - Marked as `[Obsolete]`
   - TreeView functionality integrated directly into CMSEntityExplorer
   - Can be safely deleted in future cleanup

### New Files Created

#### UXML Templates
1. **CMSEntityExplorer.uxml** - Main explorer window layout
2. **CMSEntityInspectorWindow.uxml** - Inspector window layout
3. **CMSSelectorPopup.uxml** - Selector popup layout
4. **TemplateNamePopup.uxml** - Template naming dialog layout

#### Stylesheets
1. **CMSEditorStyles.uss** - Comprehensive stylesheet defining:
   - Window borders and separators
   - Template item styles
   - Toolbar styles
   - Search field styles
   - Entity tree view styles
   - Inspector styles
   - Component list styles
   - Popup window styles
   - Sprite preview styles

## Key Changes

### IMGUI → UI Toolkit Mappings

| IMGUI | UI Toolkit |
|-------|-----------|
| `OnGUI()` | `CreateGUI()` |
| `OnInspectorGUI()` | `CreateInspectorGUI()` |
| `OnGUI(Rect, SerializedProperty, GUIContent)` | `CreatePropertyGUI(SerializedProperty)` |
| `EditorGUILayout.BeginHorizontal()` | `VisualElement { flexDirection = Row }` |
| `EditorGUILayout.TextField()` | `TextField` |
| `GUILayout.Button()` | `Button` |
| `EditorGUILayout.BeginScrollView()` | `ScrollView` |
| `EditorGUILayout.PropertyField()` | `PropertyField` |
| `EditorGUILayout.Foldout()` | `Foldout` |
| `EditorGUI.DrawRect()` | `element.style.backgroundColor` |
| `GUIStyle` | USS classes and inline styles |
| IMGUI `TreeView` | UI Toolkit `TreeView` / `ListView` |
| `Event.current` | `KeyDownEvent`, `MouseDownEvent`, etc. |

### TreeView Migration

The IMGUI `TreeView` implementation (`EntityTreeView.cs`) has been replaced with UI Toolkit's native controls:

- **Search Mode**: Uses `ListView` for flat list display
- **Default Mode**: Uses `TreeView` with hierarchical structure
- Both support:
  - Custom item rendering with sprites
  - Selection handling
  - Double-click to open
  - Keyboard navigation

### Event Handling

IMGUI's global event system has been replaced with UI Toolkit's element-specific callbacks:

```csharp
// IMGUI
if (Event.current.keyCode == KeyCode.Escape) { }

// UI Toolkit
root.RegisterCallback<KeyDownEvent>(evt => {
    if (evt.keyCode == KeyCode.Escape) { }
});
```

### Styling Approach

Styles are now defined in USS files and applied via class names:

```csharp
// IMGUI
var style = new GUIStyle { fontSize = 12 };
EditorGUILayout.Label(text, style);

// UI Toolkit
label.AddToClassList("entity-label");
// .entity-label { font-size: 12px; } in USS
```

## Testing Checklist

- [ ] CMSEntityExplorer opens and displays entities
- [ ] Search functionality filters entities correctly
- [ ] Add/Delete entity buttons work
- [ ] Template dropdown shows saved templates
- [ ] Template saving works
- [ ] Template deletion works
- [ ] Double-click opens entity in inspector
- [ ] Entity inspector window displays properties
- [ ] Component add/remove functionality works
- [ ] CMSEntityPfb selector popup works
- [ ] Sprite previews display correctly
- [ ] Keyboard shortcuts work (Escape, Enter, etc.)
- [ ] Hover effects display properly
- [ ] All custom property drawers render correctly

## Known Limitations

1. **TreeView Selection by ID**: The `FocusTreeViewAndReselect(int id)` method in CMSEntityExplorer needs implementation for UI Toolkit's TreeView
2. **Renaming**: IMGUI TreeView's rename functionality has not been ported (F2 key)
3. **Right-click Context Menus**: Not yet implemented in UI Toolkit version

## Future Improvements

1. Implement rename functionality using UI Toolkit
2. Add right-click context menus
3. Create custom USS themes for light/dark mode
4. Add animations for hover/selection states
5. Implement drag-and-drop reordering
6. Add more keyboard shortcuts
7. Optimize TreeView performance for large datasets

## Resources

- [Unity UI Toolkit Documentation](https://docs.unity3d.com/Manual/UIElements.html)
- [USS Syntax Reference](https://docs.unity3d.com/Manual/UIE-USS.html)
- [UXML Format Reference](https://docs.unity3d.com/Manual/UIE-UXML.html)
- [Migrating from IMGUI to UI Toolkit](https://docs.unity3d.com/Manual/UIE-migrate-from-imgui.html)

## Version History

- **v1.0** - Initial UI Toolkit migration (2025-11-18)
  - Migrated all editor windows
  - Migrated all custom editors
  - Migrated all property drawers
  - Created USS stylesheets
  - Created UXML templates

