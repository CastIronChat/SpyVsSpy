using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTypeRegistry : Registry<CollectibleTypeRegistry, CollectibleType> {}
public class CollectibleType : MonoBehaviour, Entity<CollectibleTypeRegistry, CollectibleType>
{
    public string name;
    public Sprite sprite;

    public int uniqueId { get; set; }
    public CollectibleTypeRegistry registry { get; set; }
}
