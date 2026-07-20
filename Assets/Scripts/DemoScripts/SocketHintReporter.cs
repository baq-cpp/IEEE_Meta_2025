using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketHintReporter : MonoBehaviour
{
    XRSocketInteractor socket;
    TextHintManager hintManager;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        hintManager = FindFirstObjectByType<TextHintManager>();

        socket.selectEntered.AddListener(OnSocketed);
        socket.selectExited.AddListener(OnUnsocketed);
    }

    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSocketed);
        socket.selectExited.RemoveListener(OnUnsocketed);
    }

    void OnSocketed(SelectEnterEventArgs args)
    {
        var inserted = args.interactableObject.transform.gameObject;
        Debug.Log($"[Socket] {name} received {inserted.name}");

        if (hintManager != null)
        {
            hintManager.OnComponentInteracted(inserted.name); // pushes this type to the hint stack
            // Optional: auto-append a hint immediately (see note below)
            // hintManager.SendMessage("OfferHintForTop"); // or expose a public method to call directly
        }
        else
        {
            Debug.LogWarning("[SocketHintReporter] No TextHintManager found in scene.");
        }
    }

    void OnUnsocketed(SelectExitEventArgs args)
    {
        var removed = args.interactableObject.transform.gameObject;
        Debug.Log($"[Socket] {name} released {removed.name}");
    }
}
