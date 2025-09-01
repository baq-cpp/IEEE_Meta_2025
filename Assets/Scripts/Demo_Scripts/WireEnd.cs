using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class WireEnd : XRGrabInteractable
{
    [Tooltip("Optional visual to highlight when hovering a compatible pin")] public Renderer tipRenderer;
    [Tooltip("Material for hover highlight")] public Material hoverMat;

    private Material _defaultMat;
    private XRSocketInteractor _currentSocket;
    private Rigidbody _rb;

    public bool IsConnected => _currentSocket != null;
    public XRSocketInteractor CurrentSocket => _currentSocket;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
        var col = GetComponent<Collider>();
        col.isTrigger = false;
        if (tipRenderer) _defaultMat = tipRenderer.sharedMaterial;
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // If plugged, only that socket or a direct interactor can select
        if (IsConnected && interactor != _currentSocket && interactor is not XRDirectInteractor)
            return false;
        return base.IsSelectableBy(interactor);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor socket)
        {
            _currentSocket = socket;
            _rb.isKinematic = true;
            if (socket.attachTransform)
            {
                transform.position = socket.attachTransform.position;
                transform.rotation = socket.attachTransform.rotation;
            }
            movementType = MovementType.Kinematic;
        }
        else if (args.interactorObject is XRDirectInteractor)
        {
            _rb.isKinematic = true;
            movementType = MovementType.Kinematic;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (args.interactorObject is XRSocketInteractor)
            _currentSocket = null;

        _rb.isKinematic = true;
        movementType = MovementType.Kinematic;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        if (tipRenderer && hoverMat && args.interactorObject is XRSocketInteractor)
            tipRenderer.material = hoverMat;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (tipRenderer && _defaultMat)
            tipRenderer.material = _defaultMat;
    }
}
