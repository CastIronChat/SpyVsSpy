using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class DescriptionAttribute : PropertyAttribute
{
    public DescriptionAttribute(string description) {
        this.description = Outdent.trim(description);
        type = MessageType.Info;
    }
    public readonly string description;
    public readonly MessageType type;
}

[CustomPropertyDrawer( typeof( DescriptionAttribute ) )]
class DescriptionDrawer : DecoratorDrawer
{
    private DescriptionAttribute descriptionAttribute { get => (DescriptionAttribute)attribute; }

    public override float GetHeight() {
        var content = new GUIContent(descriptionAttribute.description);
        return EditorStyles.helpBox.CalcHeight(content, EditorGUIUtility.currentViewWidth - 52);
    }

    public override void OnGUI(Rect position)
    {
        EditorGUI.HelpBox(position, descriptionAttribute.description.Trim(), descriptionAttribute.type);
    }
}
