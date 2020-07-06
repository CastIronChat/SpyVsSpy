using UnityEngine;

public class DescriptionAttribute : PropertyAttribute
{
    public DescriptionAttribute(string description) {
        this.description = Outdent.trim(description);
        // TODO disabled cuz I'm not sure I can safely wrap this in #if UNITY_EDITOR or if it'll break serialization.
        //type = MessageType.Info;
    }
    public readonly string description;
    //public readonly MessageType type;
}

