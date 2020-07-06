using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDescription : MonoBehaviour
{
    [Description("Description")]
    [TextArea(30, 60)]
    public string description;
}
