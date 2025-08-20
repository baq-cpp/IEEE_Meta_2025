using UnityEngine;
using UnityEngine.XR;

public class AdjustColliderToHead : MonoBehaviour
{
    public CharacterController characterController;
    public Transform xrCamera;
    public float skinWidth = 0.05f;

    void Update()
    {
        // Get camera height relative to XR Origin
        float headHeight = Mathf.Clamp(xrCamera.localPosition.y, 1f, 2f);
        characterController.height = headHeight;

        // Position the collider so its bottom is on the floor
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2f + characterController.skinWidth;
        newCenter.x = xrCamera.localPosition.x;
        newCenter.z = xrCamera.localPosition.z;
        characterController.center = newCenter;
    }
}
