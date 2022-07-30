using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(ResourceSpawner)), CanEditMultipleObjects]
public class ResourceEditor : Editor {

    private ResourceSpawner _resourceSpawner;

    private bool _spawningStarted = false;

    private void OnEnable() {
        _resourceSpawner = target as ResourceSpawner;
    }

#if UNITY_EDITOR
    public void OnSceneGUI() {
        if (_resourceSpawner.transform == Selection.activeTransform) {
            DrawCheckpointHandles();
        }
    }

    private void DrawCheckpointHandles() {
        var checkpoints = _resourceSpawner.GetComponentsInChildren<ResourceSpawnLocation>();
        foreach (var checkpoint in checkpoints) {
            EditorGUI.BeginChangeCheck();
            var newTargetPosition = Handles.PositionHandle(checkpoint.transform.position, checkpoint.transform.rotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(checkpoint.transform, "Move resource spawn location");
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
        }
    }

    private void StartSpawningUI() {
        if (!_spawningStarted && GUILayout.Button("Start Spawning")) {
            _resourceSpawner.StartSpawning();
            _spawningStarted = true;
        }

        if (_spawningStarted) {
            GUILayout.Label("Resource Spawning started");
        }
    }

    private void NewLaneCheckpointButton() {
        NewCheckpointButton("ResourceLocation");
    }

    private void NewCheckpointButton(string type) {
        if (GUILayout.Button($"Add {type}")) {

            var resourceLocationPrefab = Resources.Load<GameObject>($"Prefabs/{type}");
            var instantiationTransform = _resourceSpawner.transform;

            var offset = Random.insideUnitCircle * 5;

            var instantiationPosition = instantiationTransform.position + new Vector3(offset.x, 0, offset.y);
            var newResourceLocation = Instantiate(
                resourceLocationPrefab,
                instantiationPosition,
                instantiationTransform.rotation,
                _resourceSpawner.transform
            );

            newResourceLocation.name = $"{type} {_resourceSpawner.ResourceLocations.Count + 1}";
            Undo.RegisterCreatedObjectUndo(newResourceLocation, "Create new spanw location");
            var newSpanwLocation = newResourceLocation.GetComponent<ResourceSpawnLocation>();
            _resourceSpawner.ResourceLocations.RemoveAll(location => ReferenceEquals(location, null) ? false : (location ? false : true));
            _resourceSpawner.ResourceLocations.Add(newSpanwLocation);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_resourceSpawner);
        }
    }

    private void OnDestroy() {
        if (!Application.IsPlaying(this)) {
            // Register Undo, so that when you ctrl-z the delete operation, it is re-added to the list
            UnityEditor.Undo.RecordObject(_resourceSpawner, "Remove from parent");

        }
    }


#endif
}
