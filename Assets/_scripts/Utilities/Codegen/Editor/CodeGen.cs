using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis;

namespace Codegen
{
    // [InitializeOnLoad]
    public class Codegen
    {
        // [DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            var comment = "";
            var classBodyText = "";
            var monoBehaviorBaseType = typeof( MonoBehaviour );
            Type[] triggerAnnotationTypes = { typeof( OnTriggerEnter2D ), typeof( OnTriggerStay2D ), typeof( OnTriggerExit2D ) };
            var type = typeof( Codegen );
            foreach ( var behaviorType in type.Assembly.GetTypes() )
            {
                if ( monoBehaviorBaseType.IsAssignableFrom( behaviorType ) && behaviorType.IsPublic && behaviorType.IsClass )
                {
                    foreach ( var triggerAnnotationType in triggerAnnotationTypes )
                    {
                        comment += behaviorType + "\n";
                        var shouldGenerateHelperMethod = true;
                        var helperMethodText = @"  public static void __" + triggerAnnotationType.Name + "(this " + behaviorType.FullName + @" self, Collider2D collider) {
                ";
                        foreach ( var method in behaviorType.GetMethods( BindingFlags.Instance | BindingFlags.Public ) )
                        {
                            var attr = method.GetCustomAttribute( triggerAnnotationType );
                            if ( attr != null )
                            {
                                var methodFirstParamType = method.GetParameters()[0].ParameterType;
                                shouldGenerateHelperMethod = true;
                                helperMethodText += @"
                            var target = collider.GetComponent<" + methodFirstParamType.FullName + @">();
                            if(target != null) {
                                self." + method.Name + @"(target);
                            }
                        ";
                            }
                        }
                        helperMethodText += @"}
                ";
                        if ( shouldGenerateHelperMethod )
                        {
                            classBodyText += helperMethodText;
                        }
                    }
                }
            }

            var rootDir = Directory.GetCurrentDirectory();
            var outputPath = Path.Combine( rootDir, "Assets", "_scripts", "__generated__.cs" );
            System.IO.File.WriteAllText( outputPath, @"
            using UnityEngine;
            public static class GeneratedHelpers {
                " + classBodyText + @"
                /* stuff goes here
                " + comment + @"
                */
            }
        " );
            Debug.Log( "Write codegen output to " + outputPath );
        }

        static int counter = 0;
        static int counter2 = 0;
        // static constructor
        static Codegen() {
            counter ++;
            //Debug.Log($"Initialized Codegen watcher. Counters are {counter}, {counter2} and random number is {UnityEngine.Random.Range(0, 10)}");
        }
        [DidReloadScripts]
        static void didReloadScripts() {
            counter2 ++;
            //Debug.Log($"didReloadScripts for codegen watcher. Counters are {counter}, {counter2} and random number is {UnityEngine.Random.Range(0, 10)}");
        }
    }
}
/*
[OnChildTriggerEnter2D]
void OnInteractorHits(Player player) {

}

void __OnChildTriggerEnter2D(Collider2D collider) {

}

public partial class DoorInteractor : MonoBehaviour {
    Start() {
        target = transform.parent.GetComponent<Door>();
    }
    OnTriggerEnter2D(Collider2D collider) {
        OnTriggerEnter2D_Delegate(collider, this);
    }
}
*/
