using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemySpawner)), CanEditMultipleObjects]
public class LaneCheckpointEditor : Editor {

    private EnemySpawner spawner;

    private void OnEnable() {
        spawner = target as EnemySpawner;
    }

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

        if (!EditorApplication.isPlaying) {
            NewLaneCheckpointButton();
            NewHoardCheckpointButton();
            CorrectNameAndOrderButton();
            CreatePlacementButton();
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
            var offset = Random.insideUnitCircle * 5;
            var instantiationPosition = instantiationTransform.position + new Vector3(offset.x, 0, offset.y);
            var newCheckpointObject = PrefabUtility.InstantiatePrefab(checkpointPrefab) as GameObject;
            newCheckpointObject.transform.SetPositionAndRotation(instantiationPosition, instantiationTransform.rotation);
            newCheckpointObject.transform.SetParent(spawner.transform);
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

    private void CreatePlacementButton() {
        if (GUILayout.Button("Adjust placement")) {
            PlacementHelper.MoveToGround(spawner.gameObject);

            HashSet<LaneCheckpoint> handledCheckpoints = new();
            var currentCheckPoint = spawner.FirstCheckpoint;
            while (currentCheckPoint != null && !handledCheckpoints.Contains(currentCheckPoint)) {
                PlacementHelper.MoveToGround(currentCheckPoint.gameObject);
                handledCheckpoints.Add(currentCheckPoint);
                currentCheckPoint = currentCheckPoint.NextCheckpoint;
            }
        }
    }
}
#endif
