using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(Spawner)), CanEditMultipleObjects]
public class LaneCheckpointEditor : Editor {

    private Spawner spawner;

    private bool spawningStarted = false;

    private void OnEnable() {
        spawner = target as Spawner;
    }

    #if UNITY_EDITOR
    public void OnSceneGUI() {
        if (spawner.transform == Selection.activeTransform) {
            var checkpoints = spawner.GetComponentsInChildren<LaneCheckpoint>();
            foreach (var checkpoint in checkpoints) {
                EditorGUI.BeginChangeCheck();
                var newTargetPosition = Handles.PositionHandle(checkpoint.transform.position, checkpoint.transform.rotation);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(checkpoint.transform, "Move checkpoint");
                    checkpoint.transform.position = newTargetPosition;
                }
            }
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying) {
            if (GUILayout.Button("Add lane checkpoint")) {
                var lastCheckpoint = spawner.GetLastCheckpoint();
                var checkpointPrefab = Resources.Load<GameObject>("Prefabs/Checkpoint");
                var newCheckpoint = Instantiate(checkpointPrefab, spawner.transform);
                Undo.RegisterCreatedObjectUndo(newCheckpoint, "Create new checkpoint");
                Undo.RecordObject(lastCheckpoint, "Change next checkpoint");
                lastCheckpoint.NextCheckpoint = newCheckpoint.GetComponent<LaneCheckpoint>();
            }
        } else {
            if (!spawningStarted && GUILayout.Button("Start Spawning")) {
                spawner.StartSpawning();
                spawningStarted = true;
            }

            if (spawningStarted) {
                GUILayout.Label("Spawning started");
            }
        }
    }
    #endif
}    
