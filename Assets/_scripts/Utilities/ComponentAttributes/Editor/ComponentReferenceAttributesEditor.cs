using ComponentReferenceAttributes;
using UnityEditor;
using UnityEngine;

namespace ComponentReferenceAttributesEditor
{
    [CustomPropertyDrawer( typeof( ComponentReferenceAttribute ) )]
    [CustomPropertyDrawer( typeof( OwnComponentAttribute ) )]
    [CustomPropertyDrawer( typeof( ChildComponentAttribute ) )]
    [CustomPropertyDrawer( typeof( ParentComponentAttribute ) )]
    [CustomPropertyDrawer( typeof( DescendantComponentAttribute ) )]
    [CustomPropertyDrawer( typeof( AncestorComponentAttribute ) )]
    public class ComponentReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var componentReferenceAttribute = attribute as ComponentReferenceAttribute;

            var targetObject = property.serializedObject.targetObject as Component;

            EditorGUI.BeginProperty( position, label, property );
            var controlsRect = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var buttonWidth = 70;
            var fieldRect = new Rect( controlsRect.x, controlsRect.y, controlsRect.width - buttonWidth, controlsRect.height );
            var buttonRect = new Rect( controlsRect.x + controlsRect.width - buttonWidth, controlsRect.y, buttonWidth, controlsRect.height );

            if ( targetObject == null )
            {
                EditorGUI.HelpBox( position, "Invalid serialized object", MessageType.Warning );
                return;
            }

            // if (component != null)
            // {
            bool wasEnabled = GUI.enabled;
            // GUI.enabled = false;
            EditorGUI.PropertyField( fieldRect, property, GUIContent.none, true );
            GUI.enabled = wasEnabled;
            // }
            // else
            //     EditorGUI.HelpBox(position, $"Missing {fieldInfo.FieldType}", MessageType.Warning);
            var desiredComponentType = fieldInfo.FieldType;

            if ( GUI.Button( buttonRect, "Auto-set" ) )
            {
                var serializedComponent = property.objectReferenceValue as Component;
                var component = serializedComponent;
                if ( componentReferenceAttribute is OwnComponentAttribute )
                {
                    component = ComponentFinder.GetOwnComponent( targetObject, desiredComponentType );
                }
                else if ( componentReferenceAttribute is DescendantComponentAttribute )
                {
                    component = ComponentFinder.GetDescendantComponent( targetObject, desiredComponentType );
                }
                else if ( componentReferenceAttribute is AncestorComponentAttribute )
                {
                    component = ComponentFinder.GetAncestorComponent( targetObject, desiredComponentType );
                }
                else if ( componentReferenceAttribute is ChildComponentAttribute )
                {
                    component = ComponentFinder.GetChildComponent( targetObject, desiredComponentType );
                }
                else if ( componentReferenceAttribute is ParentComponentAttribute )
                {
                    component = ComponentFinder.GetParentComponent( targetObject, desiredComponentType );
                }
                else if ( componentReferenceAttribute is SiblingComponentAttribute )
                {
                    component = ComponentFinder.GetSiblingComponent( targetObject, desiredComponentType );
                }
                if ( component != serializedComponent )
                {
                    property.objectReferenceValue = component;
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
