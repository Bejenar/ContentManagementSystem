using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.CMSEditor
{
    [CustomEditor(typeof(CMSEntity))]
    public class CMSEntityEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            // Card State Section
            var cardStateLabel = new Label("Card State");
            cardStateLabel.AddToClassList("cms-section-header");
            root.Add(cardStateLabel);
            
            var idProperty = serializedObject.FindProperty("id");
            var idField = new PropertyField(idProperty);
            root.Add(idField);
            
            // Spacing
            root.Add(new VisualElement { style = { height = 10 } });
            
            // Components Section
            var componentsLabel = new Label("Components");
            componentsLabel.AddToClassList("cms-section-header");
            root.Add(componentsLabel);
            
            var componentsContainer = new VisualElement();
            componentsContainer.AddToClassList("component-container");
            root.Add(componentsContainer);
            
            BuildComponentsList(componentsContainer);
            
            // Add Component Button
            var addButton = new Button(() => ShowAddComponentMenu(componentsContainer))
            {
                text = "+ Add Component"
            };
            addButton.style.marginTop = 4;
            componentsContainer.Add(addButton);
            
            return root;
        }

        private void BuildComponentsList(VisualElement container)
        {
            // Clear existing components (except add button if exists)
            container.Clear();
            
            var componentsProperty = serializedObject.FindProperty("components");
            
            for (int i = 0; i < componentsProperty.arraySize; i++)
            {
                var index = i; // Capture for lambda
                var element = componentsProperty.GetArrayElementAtIndex(i);
                var typeName = element.managedReferenceFullTypename?.Split(' ').Last() ?? "Unknown";
                
                var componentItem = new VisualElement();
                componentItem.AddToClassList("component-header");
                
                var foldout = new Foldout
                {
                    text = $"[{index}] {typeName}",
                    value = element.isExpanded
                };
                foldout.AddToClassList("component-foldout");
                
                // Property field for the component
                var propertyField = new PropertyField(element, "");
                propertyField.BindProperty(element);
                
                // Sync foldout state with property
                foldout.RegisterValueChangedCallback(evt =>
                {
                    element.isExpanded = evt.newValue;
                    serializedObject.ApplyModifiedProperties();
                });
                
                // Delete button
                var deleteButton = new Button(() =>
                {
                    componentsProperty.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    BuildComponentsList(container);
                    
                    // Re-add the Add Component button
                    var addButton = new Button(() => ShowAddComponentMenu(container))
                    {
                        text = "+ Add Component"
                    };
                    addButton.style.marginTop = 4;
                    container.Add(addButton);
                })
                {
                    text = "X"
                };
                deleteButton.AddToClassList("component-delete-button");
                
                var headerContainer = new VisualElement();
                headerContainer.style.flexDirection = FlexDirection.Row;
                headerContainer.style.alignItems = Align.Center;
                headerContainer.Add(foldout);
                headerContainer.Add(deleteButton);
                
                container.Add(headerContainer);
                
                if (element.isExpanded)
                {
                    var contentContainer = new VisualElement();
                    contentContainer.style.marginLeft = 15;
                    contentContainer.Add(propertyField);
                    container.Add(contentContainer);
                }
            }
        }

        private void ShowAddComponentMenu(VisualElement container)
        {
            GenericMenu menu = new GenericMenu();

            Type baseType = typeof(EntityComponentDefinition);
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p != baseType && !p.IsAbstract);

            foreach (Type type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    var newComponent = Activator.CreateInstance(type) as EntityComponentDefinition;

                    var componentsProperty = serializedObject.FindProperty("components");
                    componentsProperty.serializedObject.Update();
                    componentsProperty.arraySize++;
                    componentsProperty.GetArrayElementAtIndex(componentsProperty.arraySize - 1).managedReferenceValue = newComponent;
                    componentsProperty.serializedObject.ApplyModifiedProperties();
                    
                    BuildComponentsList(container);
                    
                    // Re-add the Add Component button
                    var addButton = new Button(() => ShowAddComponentMenu(container))
                    {
                        text = "+ Add Component"
                    };
                    addButton.style.marginTop = 4;
                    container.Add(addButton);
                });
            }

            menu.ShowAsContext();
        }
    }
}