using System.Collections.Generic;
using Id = System.Int32;

/// Stores a collection of entities, assigning them unique IDs, so that networked RPC calls
/// can refer to them by ID.
/// The entities also store a reference to the registry.
public class Registry<R, E> : DefaultCanManipulateEntity<R, E>
where R : Registry<R, E>
where E: Entity<R, E> {
    public Registry() {
        idAllocator = new DefaultIdAllocator();
    }
    public Registry(IdAllocator idAllocator) {
        this.idAllocator = idAllocator;
    }
    private IdAllocator idAllocator;
    private Dictionary<Id, E> entities = new Dictionary<Id, E>();
    public E this[Id id] {
        get {
            return getEntity(id);
        }
    }
    public Dictionary<Id, E> __getDict() {return entities;}
    public bool hasEntity(Id id) {
        return entities.ContainsKey(id);
    }
    public E getEntity(Id id) {
        E entity;
        entities.TryGetValue(id, out entity);
        return entity;
    }
    public void addEntity(E entity) {
        Id id = idAllocator.nextId();
        setIdOfEntity(entity, id);
        entity.registry = (R)this;
        entities.Add(id, entity);
    }

    public void removeEntity(E entity) {
        entities.Remove(getIdOfEntity(entity));
        entity.registry = null;
    }
    public void removeEntity(Id id) {
        E entity;
        entities.TryGetValue(id, out entity);
        entities.Remove(id);
        entity.registry = null;
    }
}

/// Registries need to be able to get and set the ID of an entity being stored in the registry.
/// To make it generic, that ability is described via this interface.
interface CanManipulateEntity<R, E> {
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
    public void setIdOfEntity(E entity, Id id) {
        entity.uniqueId = id;
    }
    public Id getIdOfEntity(E entity) {
        return entity.uniqueId;
    }
    public R getRegistry(E entity) {
        return entity.registry;
    }
    public void setRegistry(E entity, R registry) {
        entity.registry = registry;
    }
}

/// Entities have an ID and a reference to their registry, in case they need to make RPC calls on the
/// registry or get access to Managers accessible from the registry.
public interface Entity<R, E>
where R : Registry<R, E>
where E : Entity<R, E> {
    Id uniqueId {get; set;}
    R registry {get; set;}
}

/// Every registry needs a way to allocate unique IDs.
public interface IdAllocator {
    Id nextId();
}

/// Default ID allocator which creates integer IDs in order, starting at 0.
public class DefaultIdAllocator : IdAllocator {

    private Id _nextId = 0;
    public Id nextId() {
        return _nextId++;
    }
}
