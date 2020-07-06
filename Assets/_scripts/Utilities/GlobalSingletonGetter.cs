using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GlobalSingletonGetter<T> where T : Component
{
    public GlobalSingletonGetter(string gameObjectName = null) {
        this.gameObjectName = gameObjectName;
    }
    private T _instance;
    private string gameObjectName;
    public T instance
    {
        get
        {
            if ( _instance != null ) return _instance;
            _instance = findInstance();
            Assert.IsNotNull( _instance );
            return _instance;
        }
    }
    public T instanceOrNull
    {
        get
        {
            if ( _instance != null ) return _instance;
            _instance = findInstance();
            return _instance;
        }
    }
    public T cachedInstance
    {
        get => _instance;
    }

    private T findInstance()
    {
        if(gameObjectName != null) {
            var go = GameObject.Find( gameObjectName );
            if(go == null) return null;
            _instance = go.GetComponent<T>();
        }
        T found = null;
        int Count = 0;
        // GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        // foreach ( var rootGameObject in rootGameObjects )
        // {
        //     T[] components = rootGameObject.GetComponentsInChildren<T>();
        //     foreach ( var component in components )
        //     {
        //         if(found == null) found = component;
        //         Count++;
        //     }
        // }
        foreach(var obj in Resources.FindObjectsOfTypeAll<T>()) {
                // TODO make utility functions in Misc.cs that can safely be called in a game (probs return indicating that no, nothing is a prefab in a game)
                #if UNITY_EDITOR
                if(PrefabUtility.IsPartOfPrefabAsset(obj)) continue;
                #endif
                if(found == null) found = obj;
                Count++;
        }
        Assert.IsTrue( Count <= 1 , "Found multiple matching instances.  There should be at most one in the scene.");
        return found;
    }
}
