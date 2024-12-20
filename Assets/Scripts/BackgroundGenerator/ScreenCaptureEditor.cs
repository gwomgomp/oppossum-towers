using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(ScreenCaptureHelper), true)]
public class ScreenCaptureEditor : Editor {
#if UNITY_EDITOR
    public override void OnInspectorGUI() {
        if (EditorApplication.isPlaying) {
            ScreenCaptureUI();
        }
    }

    private void ScreenCaptureUI() {
        if (GUILayout.Button("Capture Screen")) {
            ScreenCapture.CaptureScreenshot("Assets/Textures/Background.png", 4);
        }
    }
#endif
}
