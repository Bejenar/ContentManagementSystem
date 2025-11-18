using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.CMSEditor
{
    [CustomPropertyDrawer(typeof(EntityComponentDefinition), true)]
    public class EntityComponentDefinitionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            // Create a property field that will automatically display all child properties
            var propertyField = new PropertyField(property);
            propertyField.BindProperty(property);
            
            container.Add(propertyField);
            
            return container;
        }
    }
}