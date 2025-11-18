using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.CMSEditor
{
    [CustomPropertyDrawer(typeof(CMSEntityPfb), true)]
    public class CMSEntityPfbDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            var objRef = property.objectReferenceValue as CMSEntityPfb;

            // Label
            var label = new Label(property.displayName);
            label.style.minWidth = 120;
            container.Add(label);

            // Object field (disabled)
            var objectField = new ObjectField
            {
                objectType = typeof(CMSEntityPfb),
                value = objRef
            };
            objectField.SetEnabled(false);
            objectField.style.flexGrow = 1;
            container.Add(objectField);

            // Selector button
            var selectorButton = new Button(() =>
            {
                var prefabs = AssetDatabase.FindAssets("t:GameObject", new[] { CMSPaths.CMSRoot });
                var allPrefabs = prefabs
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path))
                    .Where(go => go != null)
                    .Select(go => go.GetComponent<CMSEntityPfb>())
                    .Where(p => p != null)
                    .ToList();

                CMSSelectorPopup.Show(allPrefabs, objRef, selected =>
                {
                    property.objectReferenceValue = selected;
                    property.serializedObject.ApplyModifiedProperties();
                    objectField.value = selected;
                });
            })
            {
                text = "🗂️"
            };
            selectorButton.style.width = 22;
            container.Add(selectorButton);

            return container;
        }
    }
}