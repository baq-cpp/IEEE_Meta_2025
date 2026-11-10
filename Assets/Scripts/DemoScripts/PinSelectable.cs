using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class PinSelectable : XRBaseInteractable
{
    [Header("Wiring")]
    public Transform Anchor;

    [Header("Hover Visuals")]
    public MeshRenderer highlightRenderer;   // your highlight mesh

    protected override void Awake()
    {
        base.Awake();

        if (!Anchor)
            Anchor = transform;

        // Only hide a separately assigned highlight mesh.
        if (highlightRenderer &&
            highlightRenderer != GetComponent<MeshRenderer>())
        {
            highlightRenderer.enabled = false;
        }
        else
        {
            highlightRenderer = null;
        }
    }

    void SetHover(bool on)
    {
        if (highlightRenderer) highlightRenderer.enabled = on;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        string interactorName = args.interactorObject != null &&
                                args.interactorObject.transform != null
            ? args.interactorObject.transform.name
            : "Unknown interactor";

        Debug.Log($"VR SELECT reached PinSelectable: {name} | Interactor: {interactorName}");

        if (WireManager.Instance == null)
        {
            Debug.LogError($"WireManager.Instance is NULL when selecting {name}");
            return;
        }

        Debug.Log($"Sending {name} to WireManager");
        WireManager.Instance.HandlePinSelected(this);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        SetHover(true);

        Debug.Log($"VR HOVER reached PinSelectable: {name}");
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        SetHover(false);
    }

    
}
