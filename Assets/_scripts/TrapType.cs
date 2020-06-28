using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTypeRegistry : Registry<TrapTypeRegistry, TrapType> {}

/// Immutable data structure representing the "type" of a trap
/// Player inventory contains a certain quantity of each trap "type"
/// An instance of this class describes info about the type: what it does,
/// sprites for inventory, etc.
/// Declared as a MonoBehavior so that we can configure fields via the editor.
public class TrapType : MonoBehaviour, Entity<TrapTypeRegistry, TrapType>
{
    public TrapTypeRegistry registry { get; set; }
    public int uniqueId { get; set; }
    public string name;
    public Sprite sprite;
    public Vector4 color {
        get => new Vector4(0.9f * uniqueId, 0.1f * uniqueId, 0.6f,1.0f);
    }
}
