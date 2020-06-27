using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Immutable data structure representing the "type" of a trap
/// Player inventory contains a certain quantity of each trap "type"
/// An instance of this class describes info about the type: what it does,
/// sprites for inventory, etc.
/// Declared as a MonoBehavior so that we can configure fields via the editor.
public class TrapType : MonoBehaviour
{
    public int id;
    public string name;
    public Sprite sprite;
}
