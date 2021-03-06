using System.Collections;
using System.Collections.Generic;
using CastIronChat.EntityRegistry;
using UnityEngine;

public class TrapTypeRegistry : Registry<TrapType> {
    public TrapTypeRegistry() : base(id: RegistryIds.TrapType, name: "trap types", validIdsStartAt: 0) {}
}

/// Immutable data structure representing the "type" of a trap
/// Player inventory contains a certain quantity of each trap "type"
/// An instance of this class describes info about the type: what it does,
/// sprites for inventory, etc.
/// Declared as a MonoBehavior so that we can configure fields via the editor.
public class TrapType : MonoBehaviour, Entity
{
    public BaseRegistry registry { get; set; }
    public int uniqueId { get; set; } = -1;
    new public string name;
    public Sprite sprite;

    /// True if this is the special "None" trap type.  Useful, e.g., so we can still
    /// describe how to render it in the UI, for example, as a bare hands icon in the trap selection HUD.
    public bool isNoneTrap;

    public GameObject trapEffect;
    public bool onlyHitTarget, hasKnockback, spawnOnPlayer; //or center in the room
    public float lifeTime, knockbackForce, dmgPerSecond,inputLockOut;
    public int oneTimeDamage;
    /// Can this trap be used?  For example, if it's the "None" trap then it cannot.
    public bool isUsable {
        get => !isNoneTrap;
    }
    public Vector4 color {
        get => new Vector4(0.9f * uniqueId, 0.1f * uniqueId, 0.6f,1.0f);
    }
}
