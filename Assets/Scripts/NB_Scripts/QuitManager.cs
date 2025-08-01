using UnityEngine;

public class QuitManager : MonoBehaviour
{
    public void QuitApp()
    {
        // In the actual build, this closes the application
        Application.Quit();

        // In the Unity Editor, this stops Play Mode for convenience
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
