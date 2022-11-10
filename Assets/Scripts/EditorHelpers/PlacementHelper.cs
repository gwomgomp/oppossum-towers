using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public static class PlacementHelper {
    public static void MoveToGround(GameObject toMove) {
        if (Physics.Raycast(toMove.transform.position, Vector3.down, out RaycastHit raycastHit, 500)) {
            Undo.RecordObject(toMove.transform, "Move To Ground");
            toMove.transform.position = raycastHit.point + Vector3.up * 0.1f;
        }
    }
}
#endif
