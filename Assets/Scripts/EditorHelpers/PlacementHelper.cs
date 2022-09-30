using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class PlacementHelper : Editor {
    private GameObject[] targetGameObjects;

    private void OnEnable() {
        targetGameObjects = targets.Where(target => target is Placeable)
            .Select(target => (Placeable) target)
            .Select(placeable => placeable.GetGameObject())
            .ToArray();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (targetGameObjects.Length > 0 && !EditorApplication.isPlaying) {
            CreatePlacementButton();
        }
    }

    private void CreatePlacementButton() {
        if (GUILayout.Button("Adjust placement")) {
            foreach (GameObject targetGameObject in targetGameObjects) {
                if (Physics.Raycast(targetGameObject.transform.position, Vector3.down, out RaycastHit raycastHit, 500)) {
                    Undo.RecordObject(targetGameObject.transform, "Move Placeable");
                    targetGameObject.transform.position = raycastHit.point + Vector3.up * 0.1f;
                }
            }
        }
    }
}
#endif
