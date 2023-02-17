using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(ResourceSpawner)), CanEditMultipleObjects]
public class ResourceEditor : Editor {

    private ResourceSpawner resourceSpawner;

    private bool spawningStarted = false;

    private void OnEnable() {
        resourceSpawner = target as ResourceSpawner;

    }

#if UNITY_EDITOR
    public void OnSceneGUI() {
        if (resourceSpawner.transform == Selection.activeTransform) {
            DrawSpawnHandles();
        }
    }

    private void DrawSpawnHandles() {
        var spawnpoints = resourceSpawner.GetComponentsInChildren<ResourceSpawnLocation>();
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
            CleanupSpawnLocationsButton();
        }
    }

    /// <summary>
    /// Start the spawning of resources
    /// </summary>
    private void StartSpawningUI() {
        if (!spawningStarted && GUILayout.Button("Start Spawning")) {
            resourceSpawner.StartSpawning(0);
            spawningStarted = true;
        }

        if (spawningStarted) {
            GUILayout.Label("Resource Spawning started");
        }
    }

    /// <summary>
    /// Button to add a new SpawnLocation to the prefab
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
            var instantiationTransform = resourceSpawner.transform;

            // Randomly place location around the prefab
            var offset = Random.insideUnitCircle * 5;
            var instantiationPosition = instantiationTransform.position + new Vector3(offset.x, 0, offset.y);

            var newResourceLocation = Instantiate(
                resourceLocationPrefab,
                instantiationPosition,
                instantiationTransform.rotation,
                resourceSpawner.transform
            );

            newResourceLocation.name = $"{type} {resourceSpawner.ResourceLocations.Count + 1}";
            Undo.RegisterCreatedObjectUndo(newResourceLocation, "Create new spawn location");
            var newSpawnLocation = newResourceLocation.GetComponent<ResourceSpawnLocation>();

            CleanUpSpawnLocations();

            resourceSpawner.ResourceLocations.Add(newSpawnLocation);
            PrefabUtility.RecordPrefabInstancePropertyModifications(resourceSpawner);
        }
    }

    /// <summary>
    /// Button to clean SpawnLocations
    /// </summary>
    private void CleanupSpawnLocationsButton() {
        if (GUILayout.Button($"Cleanup SpawnLocations")) {
            CleanUpSpawnLocations();
            PrefabUtility.RecordPrefabInstancePropertyModifications(resourceSpawner);
        }
    }

    /// <summary>
    /// Cleanup deleted or "None" spawnpoints
    /// </summary>
    private void CleanUpSpawnLocations() {
        resourceSpawner.ResourceLocations.RemoveAll(location => location == null || ReferenceEquals(location, null));
    }

#endif
}
