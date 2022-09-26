using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(RoundManager))]
public class RoundManagerEditor : Editor {

#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying) {
            StartSpawningUI();
        }
    }

    private void StartSpawningUI() {
        if (!RoundManager.Instance.Running && GUILayout.Button("Start Spawning")) {
            RoundManager.Instance.Run();
        }

        if (RoundManager.Instance.Running) {
            GUILayout.Label("Spawning started");
        }
    }
#endif
}
