using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Id = System.Int32;

/// Stores a collection of entities, assigning them unique IDs, so that networked RPC calls
/// can refer to them by ID.
/// The entities also store a reference to the registry.
public class Registry<R, E> : DefaultCanManipulateEntity<R, E>, IEnumerable<E>, NonGenericRegistry
where R : Registry<R, E>
where E : Entity<R, E>
{
    public Registry()
    {
        idAllocator = new DefaultIdAllocator();
        id = RegistryHelper.registryIdAllocator.nextId();
        RegistryHelper.registries.Add( id, new WeakReference<object>( this ) );
    }
    public int id { get; set; }
    public Registry(IdAllocator idAllocator)
    {
        this.idAllocator = idAllocator;
    }
    private IdAllocator idAllocator;

    /// Use a sorted dictionary so that entities are stored in Id order, like we did with the List<> prior.
    /// Sometimes sorting is important to make sure inventory items appear in predictable order.
    private SortedDictionary<Id, E> entities = new SortedDictionary<Id, E>();
    public int Count
    {
        get => entities.Count;
    }

    public E this[Id id]
    {
        get
        {
            return getEntity( id );
        }
    }
    public SortedDictionary<Id, E> __getDict() { return entities; }
    public bool hasEntity(Id id)
    {
        return entities.ContainsKey( id );
    }
    public E getEntity(Id id)
    {
        E entity;
        entities.TryGetValue( id, out entity );
        return entity;
    }
    public object getEntityAsObject(Id id)
    {
        return getEntity( id );
    }
    public IEnumerable __getEntityEnumerator()
    {
        foreach(KeyValuePair<Id, E> pair in entities) {
            yield return pair;
        }
    }
    public object __getEntitiesAsObject()
    {
        return entities;
    }
    public void addEntity(E entity)
    {
        Id id = idAllocator.nextId();
        setIdOfEntity( entity, id );
        entity.registry = (R)this;
        entities.Add( id, entity );
    }

    public void removeEntity(E entity)
    {
        entities.Remove( getIdOfEntity( entity ) );
        entity.registry = null;
    }
    public void removeEntity(Id id)
    {
        E entity;
        entities.TryGetValue( id, out entity );
        entities.Remove( id );
        entity.registry = null;
    }

    public IEnumerator<E> GetEnumerator()
    {
        return entities.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return entities.Values.GetEnumerator();
    }
}
public static class RegistryHelper
{
    // Based on samples from https://doc.photonengine.com/en-us/pun/current/reference/serialization-in-photon#streambuffer_method
    // Entity references are serialized and deserialized as:
    // 1x byte: is it null?  (1 for not null, 0 for null)
    // 4x bytes: Registry Id as int32, or 0 if null
    // 4x bytes: Id as int32, or 0 if null (valid ID can be zero)

    public static IdAllocator registryIdAllocator = new DefaultIdAllocator();
    public static Dictionary<Id, WeakReference<object>> registries = new Dictionary<Id, WeakReference<object>>();
    private static readonly byte[] memId = new byte[9];
    private static readonly short memLength = 9;
    public static object DeserializeEntityReference(StreamBuffer inStream, short length)
    {
        byte notNull;
        Id registryId;
        Id entityId;
        lock ( memId )
        {
            inStream.Read( memId, 0, memLength );
            int index = 0;
            notNull = memId[0];
            index++;
            Protocol.Deserialize( out registryId, memId, ref index );
            Protocol.Deserialize( out entityId, memId, ref index );
        }
        var registryReference = RegistryHelper.registries[registryId];
        object registryObject;
        registryReference.TryGetTarget( out registryObject );
        var registry = (NonGenericRegistry)registryObject;
        var entity = registry.getEntityAsObject( entityId );
        return entity;
    }
    public static short SerializeEntityReference(StreamBuffer outStream, object customobject)
    {
        NonGenericEntity entity = (NonGenericEntity)customobject;
        lock ( memId )
        {
            byte[] bytes = memId;
            int index = 1;
            if ( entity != null )
            {
                bytes[0] = 1;
                Protocol.Serialize( ((NonGenericRegistry)entity.registry).id, bytes, ref index );
                Protocol.Serialize( entity.uniqueId, bytes, ref index );
            }
            else
            {
                bytes[0] = 0;
                Protocol.Serialize( (Id)0, bytes, ref index );
                Protocol.Serialize( (Id)0, bytes, ref index );
            }
            outStream.Write( bytes, 0, memLength );
        }

        return memLength;
    }
}

/// Registries need to be able to get and set the ID of an entity being stored in the registry.
/// To make it generic, that ability is described via this interface.
interface CanManipulateEntity<R, E>
{
    void setIdOfEntity(E entity, Id id);
    Id getIdOfEntity(E entity);
    void setRegistry(E entity, R registry);
    R getRegistry(E entity);
}

/// Default implementation of CanGetSetEntityIds which uses the uniqueId field.
/// The only reason you'd want to avoid using this is if you needed a single kind of object
/// to be stored in 2 different kinds of registries *and* to use different IDs for them.
public class DefaultCanManipulateEntity<R, E> : CanManipulateEntity<R, E>
where R : Registry<R, E>
where E : Entity<R, E>
{
    public void setIdOfEntity(E entity, Id id)
    {
        entity.uniqueId = id;
    }
    public Id getIdOfEntity(E entity)
    {
        return entity.uniqueId;
    }
    public R getRegistry(E entity)
    {
        return (R)entity.registry;
    }
    public void setRegistry(E entity, R registry)
    {
        entity.registry = registry;
    }
}

/// Entities have an ID and a reference to their registry, in case they need to make RPC calls on the
/// registry or get access to Managers accessible from the registry.
public interface Entity<R, E> : NonGenericEntity
where R : Registry<R, E>
where E : Entity<R, E>
{ }

/// Every registry needs a way to allocate unique IDs.
public interface IdAllocator
{
    Id nextId();
}

/// Default ID allocator which creates integer IDs in order, starting at 0.
public class DefaultIdAllocator : IdAllocator
{

    private Id _nextId = 0;
    public Id nextId()
    {
        return _nextId++;
    }
}

public interface NonGenericRegistry
{
    object getEntityAsObject(Id id);
    IEnumerable __getEntityEnumerator();
    object __getEntitiesAsObject();

    Id id { get; }
}
public interface NonGenericEntity
{
    Id uniqueId { get; set; }
    object registry { get; set; }
}
