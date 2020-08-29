using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadInitSceneIfNeeded : MonoBehaviour
{
    [Description(@"
        Immediately sends the game to the init scene if it's not already loaded.
        Remembers the name of this scene so it can return you afterward.
    ")]
    public static GlobalSingletonGetter<GameConstants> gameConstantsGetter = new GlobalSingletonGetter<GameConstants>();
    public string initSceneName = "init";
    private string nextScene = "";
    public bool destroySelfAfter = false;
    public bool destroyGameObjectAfter = true;
    // [ReadOnly]
    // public bool leftInitScene = false;
    void Awake() {
        if(gameConstantsGetter.instanceOrNull == null) {
            DontDestroyOnLoad(gameObject);
            nextScene = SceneManager.GetActiveScene().name;
            Debug.Log($"Sending us to init scene. Departing from scene {SceneManager.GetActiveScene().name}, and will return to {nextScene} after init loads.", this);
            SceneManager.LoadScene(initSceneName);
            StartCoroutine(AfterInitSceneLoaded());
        }
    }
    IEnumerator AfterInitSceneLoaded() {
        yield return null;
        SceneManager.LoadScene(nextScene);
        if(destroyGameObjectAfter)  {
            Destroy(this.gameObject);
        } else if(destroySelfAfter)  {
            Destroy(this);
        }
    }

}
