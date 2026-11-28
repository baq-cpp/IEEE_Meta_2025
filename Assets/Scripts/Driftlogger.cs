using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DriftLogger : MonoBehaviour
{
    Vector3 last;
    Rigidbody rb;
    XRGrabInteractable grab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
        last = transform.position;
    }

    void LateUpdate()
    {
        if (transform.position != last)
        {
            var by = grab.firstInteractorSelecting != null ? grab.firstInteractorSelecting.transform.name : "none";
            Debug.Log($"f{Time.frameCount} moved {(transform.position - last).ToString("F5")} selectedBy={by} parent={(transform.parent ? transform.parent.name : "none")} kinematic={rb.isKinematic} vel={rb.linearVelocity}");
            last = transform.position;
        }
    }
}
