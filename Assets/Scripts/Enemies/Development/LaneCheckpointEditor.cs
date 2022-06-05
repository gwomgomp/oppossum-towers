using UnityEngine;
using System.Collections.Generic;

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
            DrawCheckpointHandles();
        }
    }

    private void DrawCheckpointHandles() {
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

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying) {
            StartSpawningUI();
        } else {
            NewCheckpointButton();
            CorrectNameAndOrderButton();
        }
    }

    private void StartSpawningUI() {
        if (!spawningStarted && GUILayout.Button("Start Spawning")) {
            spawner.StartSpawning();
            spawningStarted = true;
        }

        if (spawningStarted) {
            GUILayout.Label("Spawning started");
        }
    }

    private void NewCheckpointButton() {
        if (GUILayout.Button("Add lane checkpoint")) {
            var checkpointPrefab = Resources.Load<GameObject>("Prefabs/Checkpoint");
            (LaneCheckpoint lastCheckpoint, int checkpointCount) = spawner.GetLastCheckpoint();
            var instantiationTransform = spawner.transform;
            if (lastCheckpoint != null) {
                instantiationTransform = lastCheckpoint.transform;
            }
            var offset = Random.insideUnitCircle * 5;
            var instantiationPosition = instantiationTransform.position + new Vector3(offset.x, 0, offset.y);
            var newCheckpointObject = Instantiate(
                checkpointPrefab,
                instantiationPosition,
                instantiationTransform.rotation,
                spawner.transform
            );
            newCheckpointObject.name = "Checkpoint " + (checkpointCount + 1);
            Undo.RegisterCreatedObjectUndo(newCheckpointObject, "Create new checkpoint");
            var newCheckpoint = newCheckpointObject.GetComponent<LaneCheckpoint>();
            if (lastCheckpoint != null) {
                Undo.RecordObject(lastCheckpoint, "Change next checkpoint");
                lastCheckpoint.NextCheckpoint = newCheckpoint;
            }
            if (spawner.FirstCheckpoint == null) {
                Undo.RecordObject(spawner, "Change first checkpoint");
                spawner.FirstCheckpoint = newCheckpoint;
                PrefabUtility.RecordPrefabInstancePropertyModifications(spawner);
            }
        }
    }

    private void CorrectNameAndOrderButton() {
        if (GUILayout.Button("Correct checkpoint names and order")) {
            HashSet<LaneCheckpoint> handledCheckpoints = new();
            var currentCheckPoint = spawner.FirstCheckpoint;
            var count = 0;
            while (currentCheckPoint != null && !handledCheckpoints.Contains(currentCheckPoint)) {
                count++;
                currentCheckPoint.gameObject.name = "Checkpoint " + count;
                currentCheckPoint.transform.SetSiblingIndex(count);
                handledCheckpoints.Add(currentCheckPoint);
                var nextCheckpoint = currentCheckPoint.NextCheckpoint;
                currentCheckPoint = nextCheckpoint;
            }
        }
    }
#endif
}
