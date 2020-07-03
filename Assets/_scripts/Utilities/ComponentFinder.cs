using System;
using UnityEngine;

///<summary>
/// Helper functions to get a component relative to a GameObject: on peers, on parents, descendants, etc.
/// Some are wrappers around Unity functions but others require more complexity.
///</summary>
public static class ComponentFinder
{
    public static T GetOwnComponent<T>(Component targetObject) where T : Component
    {
        return targetObject.GetComponent<T>();
    }
    public static Component GetOwnComponent(Component targetObject, Type componentType)
    {
        return targetObject.GetComponent( componentType );
    }
    public static T GetSiblingComponent<T>(Component targetObject) where T : Component
    {
        return GetSiblingComponent( targetObject, typeof( T ) ) as T;
    }
    public static Component GetSiblingComponent(Component targetObject, Type componentType)
    {
        var parent = targetObject.transform.parent;
        var l = parent.childCount;
        for ( var i = 0; i < l; i++ )
        {
            var child = parent.GetChild( i );
            if ( child == targetObject.transform ) continue;
            var componentOnChild = child.GetComponent( componentType );
            if ( componentOnChild )
            {
                return componentOnChild;
            }
        }
        return null;
    }
    public static T GetParentComponent<T>(Component targetObject) where T : Component
    {
        return targetObject.transform.parent.GetComponent<T>();
    }
    public static Component GetParentComponent(Component targetObject, Type componentType)
    {
        return targetObject.transform.parent.GetComponent( componentType );
    }
    public static T GetChildComponent<T>(Component targetObject) where T : Component
    {
        return GetChildComponent( targetObject, typeof( T ) ) as T;
    }
    public static Component GetChildComponent(Component targetObject, Type componentType)
    {
        var l = targetObject.transform.childCount;
        for ( var i = 0; i < l; i++ )
        {
            var child = targetObject.transform.GetChild( i );
            var componentOnChild = child.GetComponent( componentType );
            if ( componentOnChild )
            {
                return componentOnChild;
            }
        }
        return null;
    }
    public static T GetAncestorComponent<T>(Component targetObject) where T : Component
    {
        return targetObject.GetComponentInParent<T>();
    }
    public static Component GetAncestorComponent(Component targetObject, Type componentType)
    {
        return targetObject.GetComponentInParent( componentType );
    }
    public static T GetDescendantComponent<T>(Component targetObject) where T : Component
    {
        return targetObject.GetComponentInChildren<T>();
    }
    public static Component GetDescendantComponent(Component targetObject, Type componentType)
    {
        return targetObject.GetComponentInChildren( componentType );
    }
}
