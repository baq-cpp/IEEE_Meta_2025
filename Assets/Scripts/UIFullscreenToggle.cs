using UnityEngine;
using UnityEngine.Animations;

public class UIFullscreenToggle : MonoBehaviour
{
    public Camera vrCamera; // ← Assign your HMD camera here (not the XR Origin root)
    public ParentConstraint parentConstraint;
    public float fullscreenDistance = 1.0f;

    private bool isFullscreen = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;
    }

    public void ToggleFullscreen()
    {
        if (!isFullscreen)
            EnterFullscreen();
        else
            ExitFullscreen();

        isFullscreen = !isFullscreen;

        parentConstraint.enabled = !parentConstraint.enabled;
    }

    void EnterFullscreen()
    {
        
        transform.SetParent(vrCamera.transform, false);
        transform.localPosition = new Vector3(0f, 0f, fullscreenDistance);
        transform.localRotation = Quaternion.identity;
    }

    void ExitFullscreen()
    {
        transform.SetParent(originalParent, true);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
