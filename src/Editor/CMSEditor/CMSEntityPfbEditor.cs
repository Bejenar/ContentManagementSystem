using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace src.Editor.CMSEditor
{
    [CustomEditor(typeof(CMSEntityPfb))]
    public class CMSEntityPfbEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            var entity = (CMSEntityPfb)target;
            var entitySprite = entity.GetSprite();

            if (entitySprite != null)
            {
                var spriteContainer = new VisualElement();
                spriteContainer.AddToClassList("sprite-preview-container");
                
                var label = new Label("Entity Icon");
                label.AddToClassList("sprite-preview-label");
                spriteContainer.Add(label);

                var pixelsPerUnit = entitySprite.pixelsPerUnit;
                var width = entitySprite.rect.width / pixelsPerUnit;
                var height = entitySprite.rect.height / pixelsPerUnit;
                var aspectRatio = width / height;
                var previewHeight = 124f;
                var previewWidth = previewHeight * aspectRatio;

                // Create an Image element for the sprite
                var spriteImage = new Image
                {
                    sprite = entitySprite,
                    scaleMode = ScaleMode.ScaleToFit
                };
                spriteImage.AddToClassList("sprite-preview-image");
                spriteImage.style.width = previewWidth;
                spriteImage.style.height = previewHeight;
                
                spriteContainer.Add(spriteImage);
                root.Add(spriteContainer);
            }

            // Add default inspector for remaining properties
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            
            return root;
        }
    }
}