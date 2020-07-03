using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EntityRegistryGlobalStateInspector : MonoBehaviour
{}

[CustomEditor(typeof(EntityRegistryGlobalStateInspector))]
public class EntityRegistryGlobalStateInspector_Editor : Editor {
    public override void OnInspectorGUI() {
        StringBuilder log = new StringBuilder();
        try {
            foreach(KeyValuePair<int, WeakReference<object>> pair in RegistryHelper.registries) {
                NonGenericRegistry reg;
                object obj;
                pair.Value.TryGetTarget(out obj);
                reg = (NonGenericRegistry)obj;
                log.Append($@"
    Key: {pair.Key}
    Value:
        Id: {(reg == null ? "null" : reg.id.ToString())}
        Entries:");
                var entities = reg.__getEntityEnumerator();
                if(entities == null) {
                    log.Append("null");
                } else {
                    foreach(KeyValuePair<int, object> entPair in entities) {
                        NonGenericEntity ent = entPair.Value as NonGenericEntity;
                        log.Append($@"
            - Key: {entPair.Key}
                Value:
                uniqueId: {ent.uniqueId}
                GetType(): {ent.GetType()}
                        ");
                    }
                }
            }
        } catch (Exception e) {
            log.Append(e);
        }
        EditorGUILayout.TextArea(log.ToString());
    }
}
