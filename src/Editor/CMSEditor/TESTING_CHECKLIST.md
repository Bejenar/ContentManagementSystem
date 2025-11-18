# CMSEditor UI Toolkit Testing Checklist

Use this checklist to verify that all functionality works correctly after the UI Toolkit migration.

## Pre-Testing Setup
- [ ] Open Unity Editor
- [ ] Ensure no compilation errors in Console
- [ ] Verify all .meta files are present for new resources

## CMSEntityExplorer Window

### Window Opening
- [ ] Open via menu: CMS > CMS Entity Explorer (#&c shortcut)
- [ ] Window opens without errors
- [ ] Window displays correctly
- [ ] Toolbar is visible
- [ ] Search field is visible

### Search Functionality
- [ ] Type in search field
- [ ] Results filter as you type
- [ ] Clear button (×) appears when text is entered
- [ ] Clear button clears search and refocuses field
- [ ] Search switches to list view
- [ ] Empty search returns to tree view

### Entity List/Tree Display
- [ ] Entities display with sprites (if available)
- [ ] Folder structure shows correctly in default view
- [ ] Entity names are readable
- [ ] Selection highlights items
- [ ] Hover effects work

### Toolbar Actions
- [ ] **Add Button**: Creates new entity in correct folder
- [ ] **Delete Button**: Shows confirmation dialog
- [ ] **Delete Button**: Removes entity after confirmation
- [ ] **Use Template Button**: Opens template dropdown
- [ ] **Save Template Button**: Opens name dialog (only with selection)

### Template Management
- [ ] Template dropdown shows saved templates
- [ ] Template dropdown shows "No saved templates" if empty
- [ ] Can select template from dropdown
- [ ] Template applies correctly (creates new entity with components)
- [ ] Can delete template from dropdown
- [ ] Delete shows confirmation dialog
- [ ] Save template dialog opens
- [ ] Can enter template name
- [ ] Save button creates template file
- [ ] Cancel button closes without saving
- [ ] Enter key saves template
- [ ] Escape key cancels

### Navigation & Keyboard Shortcuts
- [ ] Click to select entity
- [ ] Double-click opens entity in separate inspector
- [ ] Down arrow from search field moves to first item
- [ ] Up/Down arrows navigate list
- [ ] Escape closes window
- [ ] Selection persists when switching views

### Entity Creation & Deletion
- [ ] New entities appear in correct folder
- [ ] New entities have unique names (NewEntity1, NewEntity2, etc.)
- [ ] Deleted entities are removed from view
- [ ] Project refreshes after add/delete

## CMSEntityInspectorWindow

### Window Opening
- [ ] Opens when double-clicking entity in explorer
- [ ] Positions correctly relative to explorer window
- [ ] Shows entity name in title bar
- [ ] Has close button (X)

### Inspector Display
- [ ] Entity properties are visible
- [ ] Properties are editable
- [ ] Changes save correctly
- [ ] Scroll view works for long content

### Keyboard Shortcuts
- [ ] Escape closes window
- [ ] Closing refocuses explorer window

## CMSSelectorPopup

### Window Opening
- [ ] Opens from property drawer (🗂️ button)
- [ ] Positions near mouse cursor
- [ ] Has search field
- [ ] Has close button (X)

### Entity Selection
- [ ] Shows all CMSEntityPfb prefabs
- [ ] Search filters results
- [ ] Click selects entity and closes popup
- [ ] Selected entity updates in property field
- [ ] Escape closes popup

### Visual Feedback
- [ ] Current selection is highlighted
- [ ] Hover effects work
- [ ] Separators between items

## Custom Editors

### CMSEntityEditor
- [ ] Opens when selecting CMSEntity asset
- [ ] Shows "Card State" section
- [ ] Shows ID field
- [ ] Shows "Components" section
- [ ] Components are listed
- [ ] Can expand/collapse components
- [ ] Component properties are editable
- [ ] Delete button (X) removes component
- [ ] "Add Component" button shows menu
- [ ] Can select component type from menu
- [ ] New component is added to list
- [ ] Changes save to asset

### CMSEntityPfbEditor
- [ ] Opens when selecting CMSEntityPfb prefab
- [ ] Shows sprite preview (if sprite exists)
- [ ] Sprite displays correctly
- [ ] Shows remaining properties below sprite
- [ ] Properties are editable
- [ ] Changes save to prefab

## Property Drawers

### CMSEntityPfbDrawer
- [ ] Displays in inspector for CMSEntityPfb fields
- [ ] Shows label
- [ ] Shows object field (disabled)
- [ ] Shows selector button (🗂️)
- [ ] Clicking button opens CMSSelectorPopup
- [ ] Selected entity updates field
- [ ] Changes save correctly

### EntityComponentDefinitionDrawer
- [ ] Displays correctly in component lists
- [ ] Shows all component properties
- [ ] Properties are editable
- [ ] Nested properties work
- [ ] Arrays/Lists work

## Template Dropdown Window

### Window Opening
- [ ] Opens from "Use Template" button
- [ ] Positions relative to button
- [ ] Shows template list or "No templates" message

### Template List
- [ ] Shows prefab icon for each template
- [ ] Shows template name
- [ ] Shows delete button (×) for each
- [ ] Hover highlights row
- [ ] Click selects template
- [ ] Delete shows confirmation
- [ ] Delete removes template after confirmation
- [ ] Window closes after selection

## Visual & Style Checks

### General Appearance
- [ ] Consistent font sizes
- [ ] Proper spacing and padding
- [ ] Colors match Unity theme
- [ ] Borders and separators visible
- [ ] Icons display correctly

### Hover States
- [ ] Buttons highlight on hover
- [ ] List items highlight on hover
- [ ] Template items highlight on hover

### Selected States
- [ ] Selected items have distinct appearance
- [ ] Selection persists correctly

### Responsive Layout
- [ ] Resizing windows doesn't break layout
- [ ] Scroll views work when content overflows
- [ ] Elements align correctly at different sizes

## Performance

- [ ] Windows open quickly
- [ ] Search results update smoothly
- [ ] No lag when scrolling
- [ ] No memory leaks (test by opening/closing repeatedly)
- [ ] Large entity lists display efficiently

## Error Handling

- [ ] No console errors when opening windows
- [ ] No errors when clicking buttons
- [ ] No errors when typing in search
- [ ] Graceful handling of missing entities
- [ ] Graceful handling of missing sprites
- [ ] Proper error messages for invalid operations

## Cross-Testing

### Workflow Test 1: Create New Entity
1. [ ] Open CMSEntityExplorer
2. [ ] Click Add button
3. [ ] New entity appears
4. [ ] Select new entity
5. [ ] Click Save Template
6. [ ] Enter template name
7. [ ] Template is saved

### Workflow Test 2: Use Template
1. [ ] Open CMSEntityExplorer
2. [ ] Click Use Template
3. [ ] Select template
4. [ ] New entity created with components
5. [ ] Verify components are correct

### Workflow Test 3: Edit Entity
1. [ ] Open CMSEntityExplorer
2. [ ] Double-click entity
3. [ ] Inspector opens
4. [ ] Modify properties
5. [ ] Close inspector
6. [ ] Changes are saved

### Workflow Test 4: Delete Entity
1. [ ] Open CMSEntityExplorer
2. [ ] Select entity
3. [ ] Click Delete
4. [ ] Confirm deletion
5. [ ] Entity is removed
6. [ ] Asset is deleted from project

## Compatibility

- [ ] Works in Unity 2021.3+
- [ ] Works in Unity 2022.x
- [ ] Works in Unity 2023.x
- [ ] Works on Windows
- [ ] Works on macOS (if applicable)
- [ ] Works on Linux (if applicable)

## Regression Testing

Compare behavior with IMGUI version (if available):
- [ ] All original features still work
- [ ] No functionality was lost
- [ ] Performance is same or better
- [ ] User experience is same or better

## Known Issues

Document any issues found:

| Issue | Severity | Steps to Reproduce | Expected | Actual |
|-------|----------|-------------------|----------|--------|
| | | | | |

## Sign-Off

- [ ] All critical features work
- [ ] All visual elements display correctly
- [ ] No console errors
- [ ] Performance is acceptable
- [ ] Documentation is accurate

**Tested By**: _______________
**Date**: _______________
**Unity Version**: _______________
**Result**: [ ] PASS / [ ] FAIL

---

## Notes

Add any additional observations or comments here:

