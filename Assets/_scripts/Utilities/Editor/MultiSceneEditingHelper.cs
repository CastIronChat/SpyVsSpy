using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace MultiSceneEditingHelper
{
    public class MultiSceneEditingWindow : EditorWindow
    {
        [MenuItem( "Window/Multi-Scene Editing Helper" )]
        public static void ShowWindow()
        {
            GetWindow<MultiSceneEditingWindow>();
        }
        public MultiSceneEditingWindow()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if ( DisableAllButActiveScene )
            {
                if ( state == PlayModeStateChange.ExitingEditMode )
                {
                    // Disable all but the active scene
                    var active = EditorSceneManager.GetActiveScene();
                    if ( EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() )
                    {
                        for ( var i = 0; i < EditorSceneManager.sceneCount; i++ )
                        {
                            var scene = EditorSceneManager.GetSceneAt( i );
                            if ( scene.isLoaded && scene != active )
                            {
                                closedScenePaths.Add( scene.path );
                                EditorSceneManager.CloseScene( scene, false );
                            }
                        }
                    }
                }
                if ( state == PlayModeStateChange.EnteredEditMode )
                {
                    // Re-enable closed scenes
                    foreach ( var scenePath in closedScenePaths )
                    {
                        EditorSceneManager.OpenScene( scenePath, OpenSceneMode.Additive );
                    }
                    closedScenePaths.Clear();
                }
            }
        }
        public bool DisableAllButActiveScene = false;
        public List<string> closedScenePaths = new List<string>();
        void OnGUI()
        {
            GUILayout.Label( "Extra editor options for multi-scene editing." );
            GUILayout.Space( 10 );
            GUILayout.Label( "Check this box if you want \"Play\" mode to only run the active scene when using multi-scene editing." );
            GUILayout.Label( "When you click Play, all other scenes will be unloaded." );
            GUILayout.Label( "When you return to Edit mode, the will be re-loaded." );

            DisableAllButActiveScene = EditorGUILayout.Toggle( DisableAllButActiveScene );

            var active = EditorSceneManager.GetActiveScene();
            GUILayout.Label( "" );
            GUILayout.Label( $"Active scene: {describeScene( active )}" );
            GUILayout.Label( "" );
            if ( !Application.isPlaying )
            {
                if ( EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() )
                {
                    for ( var i = 0; i < EditorSceneManager.sceneCount; i++ )
                    {
                        var scene = EditorSceneManager.GetSceneAt( i );
                        GUILayout.Label( $"Scene: {describeScene( scene )}" );
                    }
                }
            }
        }

        private string describeScene(Scene scene)
        {
            return $"handle={scene.handle}, name={scene.name}, path={scene.path}";
        }
    }
}
