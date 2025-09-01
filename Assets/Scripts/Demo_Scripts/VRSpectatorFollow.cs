// VRSpectatorFollow.cs
using UnityEngine;

public class VRSpectatorFollow : MonoBehaviour
{
    [SerializeField] Camera xrCamera;        // assign your XR Origin’s Camera here
    [SerializeField] float desktopFOV = 70f;
    [SerializeField] Vector3 positionOffset; // e.g., new Vector3(0f, 0.1f, -0.3f)
    [SerializeField] Vector3 eulerOffset;    // small yaw/pitch/roll tweaks

    Camera spectator;

    void Awake()
    {
        spectator = GetComponent<Camera>();
        spectator.stereoTargetEye = StereoTargetEyeMask.None; // same as Target Eye = None
        spectator.fieldOfView = desktopFOV;
    }

    void LateUpdate()
    {
        if (!xrCamera) return;
        // follow head pose
        transform.SetPositionAndRotation(xrCamera.transform.position, xrCamera.transform.rotation);
        // apply offsets in head space
        transform.position += xrCamera.transform.TransformVector(positionOffset);
        transform.rotation = transform.rotation * Quaternion.Euler(eulerOffset);
        spectator.fieldOfView = desktopFOV;
    }
}
