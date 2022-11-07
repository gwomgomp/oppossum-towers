using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class PlacementHelperEditor : Editor {
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
                PlacementHelper.MoveToGround(targetGameObject);
            }
        }
    }
}
#endif
