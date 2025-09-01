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
        if (!Anchor) Anchor = transform;
        if (!highlightRenderer)
            highlightRenderer = GetComponentInChildren<MeshRenderer>(true);
        if (highlightRenderer) highlightRenderer.enabled = false;
    }

    void SetHover(bool on)
    {
        if (highlightRenderer) highlightRenderer.enabled = on;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        SetHover(true);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        SetHover(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        WireManager.Instance.HandlePinSelected(this);
    }
}
