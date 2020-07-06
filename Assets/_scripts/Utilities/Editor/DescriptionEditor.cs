using UnityEditor;
using UnityEngine;

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
        EditorGUI.HelpBox(position, descriptionAttribute.description.Trim(), /*descriptionAttribute.type*/MessageType.Info);
    }
}
