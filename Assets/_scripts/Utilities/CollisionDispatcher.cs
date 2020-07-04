using System;

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CollisionDispatch
{
    public class Trigger : Attribute
    {
        public Trigger() { }
        public Trigger(Type componentType)
        {
            this.type = componentType;
        }
        public Type type;
    }
    public class OnTriggerEnter2D : Trigger { }
    public class OnTriggerExit2D : Trigger { }
    public class OnTriggerStay2D : Trigger { }
    public class OnCollisionEnter2D : Trigger { }
    public class OnCollisionExit2D : Trigger { }
    public class OnCollisionStay2D : Trigger { }
    public class CollisionDispatcher
    {
        private static Dictionary<Type, CollisionDispatcher> cachedDispatchers = new Dictionary<Type, CollisionDispatcher>();
        public static CollisionDispatcher Get<T>() where T : MonoBehaviour
        {
            var behaviorType = typeof( T );
            var dispatcher = cachedDispatchers.GetOrDefault(behaviorType);
            if ( dispatcher == null )
            {
                dispatcher = new CollisionDispatcher( behaviorType );
                cachedDispatchers[behaviorType] = dispatcher;
            }
            return dispatcher;
        }
        private static Type monoBehaviorBaseType = typeof( MonoBehaviour );
        private Type behaviorType;
        public CollisionDispatcher(Type behaviorType)
        {
            this.behaviorType = behaviorType;
            var type = typeof( DummyType );
            Debug.Assert( monoBehaviorBaseType.IsAssignableFrom( behaviorType ) );
            Debug.Assert( behaviorType.IsPublic );
            Debug.Assert( behaviorType.IsClass );
            foreach ( var method in behaviorType.GetMethods( BindingFlags.Instance | BindingFlags.Public ) )
            {
                initMappings( typeof( OnTriggerEnter2D ), triggerEnter2DMappings, method );
                initMappings( typeof( OnTriggerExit2D ), triggerExit2DMappings, method );
                initMappings( typeof( OnTriggerStay2D ), triggerStay2DMappings, method );
                initMappings( typeof( OnCollisionEnter2D ), collisionEnter2DMappings, method );
                initMappings( typeof( OnCollisionExit2D ), collisionExit2DMappings, method );
                initMappings( typeof( OnCollisionStay2D ), collisionStay2DMappings, method );
            }
        }
        private void initMappings(Type triggerAnnotationType, Dictionary<Type, MethodInfo> mapping, MethodInfo method)
        {
            var attr = method.GetCustomAttribute( triggerAnnotationType );
            if ( attr != null )
            {
                mapping.Add( method.GetParameters()[0].ParameterType, method );
            }
        }

        public Dictionary<Type, MethodInfo> triggerEnter2DMappings = new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> triggerExit2DMappings = new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> triggerStay2DMappings = new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> collisionEnter2DMappings = new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> collisionExit2DMappings = new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> collisionStay2DMappings = new Dictionary<Type, MethodInfo>();
        private void Dispatch(MonoBehaviour self, Collider2D collider, Dictionary<Type, MethodInfo> mapping)
        {
            foreach ( KeyValuePair<Type, MethodInfo> pair in mapping )
            {
                var type = pair.Key;
                var method = pair.Value;
                var component = collider.GetComponent( type );
                if ( component != null )
                {
                    method.Invoke( self, new object[] { component } );
                }
            }
        }
        public void DispatchOnTriggerEnter2D(MonoBehaviour self, Collider2D collider)
        {
            Dispatch( self, collider, triggerEnter2DMappings );
        }
        public void DispatchOnTriggerExit2D(MonoBehaviour self, Collider2D collider)
        {
            Dispatch( self, collider, triggerExit2DMappings );
        }
        public void DispatchOnTriggerStay2D(MonoBehaviour self, Collider2D collider)
        {
            Dispatch( self, collider, triggerStay2DMappings );
        }
        public void DispatchOnCollisionEnter2D(MonoBehaviour self, Collider2D collider)
        {
            Dispatch( self, collider, collisionEnter2DMappings );
        }
        public void DispatchOnCollisionExit2D(MonoBehaviour self, Collider2D collider)
        {
            Dispatch( self, collider, collisionExit2DMappings );
        }
        public void DispatchOnCollisionStay2D(MonoBehaviour self, Collider2D collider)
        {
            Dispatch( self, collider, collisionStay2DMappings );
        }
    }
}