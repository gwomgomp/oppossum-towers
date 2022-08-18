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
            DrawSpawnHandles();
        }

        EditorGUI.BeginChangeCheck();
        var typeToSpawn = _resourceSpawner.TypeToSpawn;
        if (EditorGUI.EndChangeCheck()) {            
            Undo.RecordObject(_resourceSpawner, "Spawner");
            var viewedModelFilter = ((MeshFilter)_resourceSpawner.GetComponent("MeshFilter"));
            var newMesh = Resources.Load<Mesh>($"Meshes/bigbrick");
            viewedModelFilter.mesh = newMesh;
        }
    }

    private void DrawSpawnHandles() {
        var spawnpoints = _resourceSpawner.GetComponentsInChildren<ResourceSpawnLocation>();
        foreach (var spawnpoint in spawnpoints) {
            EditorGUI.BeginChangeCheck();
            var newTargetPosition = Handles.PositionHandle(spawnpoint.transform.position, spawnpoint.transform.rotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spawnpoint.transform, "Move resource spawn location");
                spawnpoint.transform.position = newTargetPosition;
            }
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying) {
            StartSpawningUI();
        } else {
            NewSpawnpointButton();
            CleanupSpanwLocationsButton();
        }
    }

    /// <summary>
    /// Start the spawning of resources
    /// </summary>
    private void StartSpawningUI() {
        if (!_spawningStarted && GUILayout.Button("Start Spawning")) {
            _resourceSpawner.StartSpawning();
            _spawningStarted = true;
        }

        if (_spawningStarted) {
            GUILayout.Label("Resource Spawning started");
        }
    }

    /// <summary>
    /// Button to add a new Spawnlocation to the prefab
    /// </summary>
    private void NewSpawnpointButton() {
        NewSpawnpointButton("ResourceLocation");
    }

    /// <summary>
    /// Add a ResourceLocation to spawn from
    /// </summary>
    /// <param name="type">Type of the prefab to spawn and add to the Spawner</param>
    private void NewSpawnpointButton(string type) {
        if (GUILayout.Button($"Add {type}")) {

            var resourceLocationPrefab = Resources.Load<GameObject>($"Prefabs/{type}");
            var instantiationTransform = _resourceSpawner.transform;

            // Randomly place location around the prefab
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

            CleanUpSpawnlocations();

            _resourceSpawner.ResourceLocations.Add(newSpanwLocation);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_resourceSpawner);
        }
    }

    /// <summary>
    /// Button to clean Spawnlocations
    /// </summary>
    private void CleanupSpanwLocationsButton() {
        if (GUILayout.Button($"Cleanup Spawnlocations")) {
            CleanUpSpawnlocations();
        }
    }

    /// <summary>
    /// Cleanup deleted or "None" spawnpoints
    /// </summary>
    private void CleanUpSpawnlocations() {
        _resourceSpawner.ResourceLocations.RemoveAll(location => location == null || ReferenceEquals(location, null));
    }

    //private void OnDestroy() {
    //    if (!Application.IsPlaying(this)) {
    //        // Register Undo, so that when you ctrl-z the delete operation, it is re-added to the list
    //        UnityEditor.Undo.RecordObject(_resourceSpawner, "Removed from parent");

    //    }
    //}


#endif
}
