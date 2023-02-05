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
        var roundManager = ManagerProvider.Instance.GetManager<RoundManager>();
        if (!roundManager.Running && GUILayout.Button("Start Spawning")) {
            roundManager.Run();
        }

        if (roundManager.Running) {
            GUILayout.Label("Spawning started");
        }
    }
#endif
}
