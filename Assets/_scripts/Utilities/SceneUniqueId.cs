using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUniqueId : MonoBehaviour
{
    [Description(@"
        Auto-assigns a numeric ID that is guaranteed unique within this Scene *in the editor*.

        This can be used, for example, if we leave and return to a scene, and certain objects
        have persisted.

        Unity will try to re-create duplicates of these objects from the scene.  The persisted
        object and the duplicate will have the same ID so the new one can choose to destroy itself.
    ")]
    public int id = 0;
    // TODO when scene is deleted, should delete hashset too.  WeakRef magic?
    static Dictionary<Scene, SceneIds> state = new Dictionary<Scene, SceneIds>();
    #if UNITY_EDITOR
    void OnValidate() {
        if(PrefabStageUtility.GetCurrentPrefabStage() != null || PrefabUtility.IsPartOfPrefabAsset(this)) {
            id = 0;
        } else {
            var scene = SceneManager.GetActiveScene();
            SceneIds idsInScene = null;
            if(!state.TryGetValue(scene, out idsInScene)) {
                state[scene] = idsInScene = new SceneIds();
            }
            if(id == 0 || !idsInScene.tryClaimId(id)) {
                id = idsInScene.getUnclaimedId();
            }
            // NOTE once an ID is claimed, if the object ever tries to get a different ID, the original ID is not freed.
            // This should be ok since state is often reset in the editor.
            // Once all objects in a scene have unique IDs, those IDs are serialized and a reload will allow them all to claim their IDs.
            // Only when new objects are added, or when existing objects are copy-pasted, will collisions be detected
            // and one of the object's ID will change.
        }
    }
    #endif
    private class SceneIds {
        HashSet<int> ids = new HashSet<int>();
        int nextIdForAllocation = 1;
        public int getUnclaimedId() {
            while(true) {
                var nextId = nextIdForAllocation++;
                if(tryClaimId(nextId)) {
                    return nextId;
                }
            }
        }
        public bool idIsTaken(int id) {
            return ids.Contains(id);
        }
        public bool tryClaimId(int id) {
            if(idIsTaken(id)) return false;
            ids.Add(id);
            return true;
        }
    }
}
