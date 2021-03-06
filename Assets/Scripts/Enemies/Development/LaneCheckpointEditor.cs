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
            NewLaneCheckpointButton();
            NewHoardCheckpointButton();
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

    private void NewLaneCheckpointButton() {
        NewCheckpointButton("Checkpoint");
    }

    private void NewHoardCheckpointButton() {
        NewCheckpointButton("Hoard");
    }

    private void NewCheckpointButton(string type) {
        if (GUILayout.Button($"Add {type}")) {
            var checkpointPrefab = Resources.Load<GameObject>($"Prefabs/{type}");
            (LaneCheckpoint lastCheckpoint, int checkpointCount, int hoardCount) = spawner.GetLastCheckpoint();
            var instantiationTransform = spawner.transform;
            if (lastCheckpoint != null) {
                instantiationTransform = lastCheckpoint.transform;
            }
            var randomOffset = Random.insideUnitCircle * 5; // make new checkpoint not overlap previous one
            var instantiationPosition = instantiationTransform.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            var newCheckpointObject = Instantiate(
                checkpointPrefab,
                instantiationPosition,
                instantiationTransform.rotation,
                spawner.transform
            );
            newCheckpointObject.name = $"{type} {(type == "Hoard" ? hoardCount : checkpointCount) + 1}";
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
            var hoardCount = 0;
            while (currentCheckPoint != null && !handledCheckpoints.Contains(currentCheckPoint)) {
                count++;
                if (currentCheckPoint is Hoard) {
                    hoardCount++;
                    currentCheckPoint.gameObject.name = $"Hoard {hoardCount}";
                } else {
                    currentCheckPoint.gameObject.name = $"Checkpoint {count - hoardCount}";
                }
                currentCheckPoint.transform.SetSiblingIndex(count);
                handledCheckpoints.Add(currentCheckPoint);
                var nextCheckpoint = currentCheckPoint.NextCheckpoint;
                currentCheckPoint = nextCheckpoint;
            }
        }
    }
#endif
}
