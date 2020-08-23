using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.Assertions;
using Id = System.Int32;

namespace CastIronChat.EntityRegistry
{
    /// <summary>
    /// Fixed registry IDs to make sure each client uses the same one.
    /// Every client must use the same registry ID for its registry
    /// storing a particular type of entity.  For example, the TrapType registry
    /// on all clients must use ID 1.
    /// </summary>
    public static class RegistryIds
    {
        public static Id TrapType = 1;
        public static Id CollectibleType = 2;
        public static Id Player = 3;
        public static Id HidingSpot = 4;
        public static Id Doors = 5;

        // Used by ID allocator to allocate new ones for other registries
        public static Id unclaimedIdsStartAt = 100;
    }

    /// <summary>
    /// Most XyzManagers need to keep track of a List<> of objects, where each object can be referred to by its index in the list.
    /// Each object needs to know its own index, and some mechanism needs to exist to put the object into the list at the right spot.
    ///
    /// Registry<> is meant to make this concept abstract and wrap it up behind a nice API.  A `Registry<>` stores multiple `Entity`s.
    /// Each Entity has its own ID, and the Registry will assign an ID if the Entity doesn't already have one.  The registry will also
    /// throw helpful errors if `Entity`s IDs ever collide, because if this happens, it indicates a flaw in our code elsewhere.
    ///
    /// A Registry can also, optionally, be registered with Photon so that objects in the registry can be passed directly to RPC calls.
    /// The object will be serialized as 2x numbers: the registry's ID and the entity's ID.  The first ID tells the
    /// receiver which registry to check, and which index in the List<> to lookup.  For example, sending the numbers 1, 1 will tell
    /// the client to check registry 1 (by convention, holds TrapTypes thanks to the `RegistryIds` enumeration above) and get entity 1
    /// out of that registry.
    ///
    /// By default, IDs are allocated starting with 0.  For a registry containing a constant set of items, this will behave like a
    /// List<> and you can use a for() loop to iterate over the items. Alternatively, you can set validIdsStartAt to 1, which matches
    /// the rules for Photon ViewIDs where IDs must be >= 1.  This could be useful if you want your Entities to start with the default
    /// ID of 0 and be assigned one when added to the registry.
    /// </summary>
    public class Registry<E> : Experimental.DefaultEntityManipulator<E>, IEnumerable<E>, BaseRegistry
        where E : class, Entity
    {
        public Registry(int validIdsStartAt, Id id = 0, string name = null, IdAllocationMode idAllocationMode = IdAllocationMode.IfNotSet)
        {
            this.validIdsStartAt = validIdsStartAt;
            this.idAllocator = new DefaultIdAllocator(validIdsStartAt);
            Assert.IsTrue( id >= 0 );
            this.id = id > 0 ? id : RegistryManager.registryIdAllocator.nextId();
            this.name = name ?? $"<registry{id}>";
            this.idAllocationMode = idAllocationMode;
        }

        /// <summary>
        /// Registry's ID, used to uniquely identify this registry across the network.
        /// See the `RegistryIds` enumeration above for well-known IDs.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// If set, will be used to make diagnostic logging and error messages more readable.
        /// </summary>
        public readonly string name;

        private readonly IdAllocationMode idAllocationMode;

        private IdAllocator idAllocator;

        /// <summary>
        /// If an incoming entity has an ID less than this value, it's considered "unset" and we will allocate it a new ID.
        /// </summary>
        private int validIdsStartAt;

        /// <summary>
        /// Tell Photon to use this instance as the serializer / deserializer, bumping off any previously-registered Registry
        /// </summary>
        public void installAsSerializer()
        {
            // TODO If there is a previously-registered registry, should we do something to it
            // to make it "disabled" so that accidentally using entities from it will throw an error?
            RegistryManager.registries[id] = new WeakReference<BaseRegistry>( this );
            // Tell photon that when it needs to serialize or deserialize type E, it should use our mechanism.
            PhotonPeer.RegisterType(typeof(E), 2, RegistryManager.SerializeEntityReference, RegistryManager.DeserializeEntityReference);
        }

        /// <summary>
        /// Like a List.  Has ordering, get, and set operations.
        /// </summary>
        private SortedDictionary<Id, E> entities = new SortedDictionary<Id, E>();

        public int Count
        {
            get => entities.Count;
        }

        /// <summary>
        /// Get the entity with an ID, or null if no entity is stored for that ID.
        /// Meant to behave like an infinitely-long `List<>` that has `null` in the empty slots, so you never have to write
        /// extra code checking the length.
        /// </summary>
        public E this[Id id]
        {
            get { return get( id ); }
        }

        public bool contains(Id id)
        {
            return entities.ContainsKey( id );
        }

        /// <summary>
        /// Same as subscripting: `var entity = thisRegistry[id]`
        /// </summary>
        public E get(Id id)
        {
            E entity;
            if ( entities.TryGetValue( id, out entity ) ) return entity;
            return null;
        }

        public object getAsObject(Id id)
        {
            return get( id );
        }

        public void add(E entity)
        {
            add(entity, idAllocationMode  );
        }
        public void add(E entity, IdAllocationMode idAllocationMode)
        {
            Id id = getId( entity );
            if ( idAllocationMode == IdAllocationMode.Always || (idAllocationMode != IdAllocationMode.Never && id < validIdsStartAt ))
            {
                id = idAllocator.nextId();
                if ( id < validIdsStartAt )
                {
                    throw new Exception($"Error adding entity to registry: the ID allocator gave us an ID which is lower than the minimum value.  Allocator gave us {id} but the minimum allowed ID is {validIdsStartAt}");
                }
                setId( entity, id );
            }

            if ( idAllocationMode == IdAllocationMode.Never && id < validIdsStartAt )
            {
                throw new Exception($"Entity being added to registry does not have a valid ID and the registry is set to Never allocate IDs.  Entity ID is {id} but it must be greater than or equal to {validIdsStartAt}");
            }

            if ( contains( id ) )
            {
                throw new Exception(
                    $"Error: adding entity to registry #{id} (named \"{name}\"): registry already has an entity with ID {id}." );
            }

            entity.registry = this;
            entities.Add( id, entity );
        }

        public void remove(E entity)
        {
            if ( entity == null )
            {
                throw new Exception( "Error: trying to remove `null` entity from a registry." );
            }

            var id = getId( entity );
            var storedEntity = get( id );
            if ( !System.Object.ReferenceEquals( storedEntity, entity ) )
            {
                throw new Exception(
                    $"Error: trying to remove an entity that is not in the registry.  Entity ID={id}" );
            }

            entities.Remove( id );
            entity.registry = null;
        }

        public void remove(Id id)
        {
            E entity = get( id );
            if ( entity == null )
            {
                throw new Exception( $"Error: Registry does not contain an entity with ID {id} to remove." );
            }
            entities.Remove( id );
            entity.registry = null;
        }

        public void clear()
        {
            foreach ( E entity in entities.Values )
            {
                entity.registry = null;
            }
            entities.Clear();
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

    /// <summary>
    /// Minimum, non-generic interface for a Registry.
    /// </summary>
    public interface BaseRegistry
    {
        object getAsObject(Id id);

        Id id { get; }
    }

    /// <summary>
    /// How a Registry should auto-assign IDs to entities when they're added to the registry.
    /// </summary>
    public enum IdAllocationMode
    {
        /// <summary>
        /// Never assign an ID.  The entity must already have a valid ID.
        /// </summary>
        Never = 1,
        /// <summary>
        /// Always assign a new ID, ignoring the one the entity already has.
        /// </summary>
        Always = 2,
        /// <summary>
        /// Only assign an ID if the entity does not have one.
        /// </summary>
        IfNotSet = 4
    }

    /// <summary>
    /// Keeps track of all installed registries.
    /// Handles serializing and deserializing Entities passed over the network.
    /// </summary>
    public static class RegistryManager
    {
        // Based on samples from https://doc.photonengine.com/en-us/pun/current/reference/serialization-in-photon#streambuffer_method
        // Entity references are serialized and deserialized as:
        // 1x byte: is it null?  (1 for not null, 0 for null)
        // 4x bytes: Registry Id as int32, or 0 if null
        // 4x bytes: Id as int32, or 0 if null (valid ID can be zero)

        /// <summary>
        /// When we create a registry and don't specify an id, this allocator will create a new one.
        /// Probably never used, since we'll always use the fixed IDs in the enumeration at the top of this file.
        /// </summary>
        public static IdAllocator registryIdAllocator = new DefaultIdAllocator( RegistryIds.unclaimedIdsStartAt );

        /// <summary>
        /// Maps from registry ID to registry, used to deserialize objects.
        /// </summary>
        public static Dictionary<Id, WeakReference<BaseRegistry>> registries =
            new Dictionary<Id, WeakReference<BaseRegistry>>();

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

            if ( notNull == 0 ) return null;

            // Lookup the registry by ID.  It's a WeakReference, so we need to get the reference, then get the value it points to.
            WeakReference<BaseRegistry> registryWeakReference;
            if ( !registries.TryGetValue( registryId, out registryWeakReference ) )
            {
                throw new Exception(
                    $"Attempting to deserialize incoming Entity #{entityId} for registry #${registryId}: we don't have a registry with that ID.  Did we forget to install the registry?" );
            }
            BaseRegistry registry;
            registryWeakReference.TryGetTarget( out registry );
            if ( registry == null )
                throw new Exception(
                    $"Attempting to deserialize incoming Entity #{entityId} for registry #${registryId}: we don't have a registry with that ID.  Did we forget to install the registry?" );
            var entity = registry.getAsObject( entityId );
            if ( entity == null )
            {
                throw new Exception(
                    $"Attempting to deserialize incoming Entity #{entityId} for registry #${registryId}: registry does not contain an entity with that ID." );
            }
            return entity;
        }

        private static string serializeEntityExceptionPrefix = "Attempting to serializing outgoing entity: ";

        public static short SerializeEntityReference(StreamBuffer outStream, object customobject)
        {
            Entity entity = (Entity) customobject;

            // Debug logging.  Theoretically can be removed for a production build, but helps debugging registry-related issues
            // by throwing helpful exceptions.
            if ( entity != null )
            {
                BaseRegistry r = entity.registry;
                if ( r == null )
                {
                    throw new Exception($"{serializeEntityExceptionPrefix}entity.registry is null.  This should be set automatically when the entity is added to a registry; did we forget to do that?");
                }
                WeakReference<BaseRegistry> storedRegistryWeakRef;
                if ( !registries.TryGetValue( r.id, out storedRegistryWeakRef ) )
                {
                    throw new Exception(
                        $"{serializeEntityExceptionPrefix}We don't have an installed registry with ID: {entity.registry.id}.  Did we forget to install it?" );
                }

                BaseRegistry storedRegistry;
                if ( !storedRegistryWeakRef.TryGetTarget( out storedRegistry ) )
                {
                    throw new Exception(
                        $"{serializeEntityExceptionPrefix}It looks like the entity's registry was never installed, because we don't know of a registry with its ID: {entity.registry.id}." );
                }

                if ( storedRegistry != r )
                {
                    throw new Exception(
                        $"{serializeEntityExceptionPrefix}The installed registry for ID {r.id} does not match the entity's registry.  Maybe we forgot to install the registry, or maybe we accidentally used the same ID for multiple registries." );
                }
            }

            lock ( memId )
            {
                byte[] bytes = memId;
                int index = 1;
                if ( entity != null )
                {
                    bytes[0] = 1;
                    Protocol.Serialize( entity.registry.id, bytes, ref index );
                    Protocol.Serialize( entity.uniqueId, bytes, ref index );
                }
                else
                {
                    bytes[0] = 0;
                    Protocol.Serialize( 0, bytes, ref index );
                    Protocol.Serialize( 0, bytes, ref index );
                }

                outStream.Write( bytes, 0, memLength );
            }

            return memLength;
        }
    }

    /// Entities have an ID and a reference to their registry, in case they need to make RPC calls on the
    /// registry or get access to Managers accessible from the registry.
    public interface Entity
    {
        Id uniqueId { get; set; }
        BaseRegistry registry { get; set; }
    }

    /// <summary>
    /// Every registry needs a way to allocate unique IDs.
    /// </summary>
    public interface IdAllocator
    {
        Id nextId();
    }

    /// Default ID allocator which creates integer IDs in order, starting at 0.
    public class DefaultIdAllocator : IdAllocator
    {
        public DefaultIdAllocator(Id startAt = 1)
        {
            _nextId = startAt;
        }

        private Id _nextId;

        public Id nextId()
        {
            return _nextId++;
        }
    }

    namespace Experimental
    {
        /// <summary>
        /// *NOTE* Just ignore this interface; you can't use alternative implementations.  I tried it as an experiment, decided it
        /// wasn't useful, I just haven't removed it yet.  Some code bypasses this interface, so trying to use a different
        /// implementation would break.
        ///
        /// Registries need to be able to get and set the ID of an entity being stored in the registry.
        /// To make it generic, that ability is described via this interface.
        ///
        /// TODO Is this abstraction *ever* going to be useful?  Only if a single entity needs to live in multiple registries,
        /// and needs to have different IDs in each.
        /// </summary>
        interface EntityManipulator<E>
        {
            void setId(E entity, Id id);
            Id getId(E entity);
            void setRegistry(E entity, BaseRegistry registry);
            BaseRegistry getRegistry(E entity);
        }

        /// Default implementation of EntityManipulator which uses the uniqueId field.
        /// The only reason you'd want to avoid using this is if you needed a single kind of object
        /// to be stored in 2 different kinds of registries *and* to use different IDs for them.
        public class DefaultEntityManipulator<E> : EntityManipulator<E>
            where E : Entity
        {
            public void setId(E entity, Id id)
            {
                entity.uniqueId = id;
            }

            public Id getId(E entity)
            {
                return entity.uniqueId;
            }

            public BaseRegistry getRegistry(E entity)
            {
                return entity.registry;
            }

            public void setRegistry(E entity, BaseRegistry registry)
            {
                entity.registry = registry;
            }
        }
    }
}
