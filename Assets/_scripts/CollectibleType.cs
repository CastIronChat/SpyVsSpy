using System.Collections;
using System.Collections.Generic;
using CastIronChat.EntityRegistry;
using UnityEngine;

public class CollectibleTypeRegistry : Registry<CollectibleType>
{
    public CollectibleTypeRegistry() : base( id: RegistryIds.CollectibleType, name: "collectibles", validIdsStartAt: 0 )
    {
    }
}

public class CollectibleType : MonoBehaviour, Entity
{
    new public string name;
    public Sprite sprite;

    public int uniqueId { get; set; } = -1;
    public BaseRegistry registry { get; set; }
}
