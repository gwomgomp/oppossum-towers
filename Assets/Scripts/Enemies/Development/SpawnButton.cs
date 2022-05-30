using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Spawner))]
public class SpawnButton : Editor {
    private Spawner spawner;

    private bool spawningStarted = false;

    private void OnEnable() {
        spawner = target as Spawner;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying) {
            GUILayout.Space(30);
            if (!spawningStarted && GUILayout.Button("Start Spawning")) {
                spawner.StartSpawning();
                spawningStarted = true;
            }

            if (spawningStarted) {
                GUILayout.Label("Spawning started");
            }
        }
    }
}