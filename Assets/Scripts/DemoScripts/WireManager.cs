// WireManager.cs — point & click wiring + pin-tap delete mode
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WireManager : MonoBehaviour
{
    public static WireManager Instance { get; private set; }

    private void Update()
    {
#if UNITY_EDITOR
        if (Mouse.current == null || Camera.main == null)
            return;

        bool leftClick = Mouse.current.leftButton.wasPressedThisFrame;
        bool rightClick = Mouse.current.rightButton.wasPressedThisFrame;

        if (!leftClick && !rightClick)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            MouseWireSelector selector =
                hit.collider.GetComponentInParent<MouseWireSelector>();

            PinSelectable pin =
                hit.collider.GetComponentInParent<PinSelectable>();

            if (rightClick)
            {
                if (selector != null)
                {
                    DeleteAllWiresFromPin(selector.Pin);
                    break;
                }

                if (pin != null)
                {
                    DeleteAllWiresFromPin(pin);
                    break;
                }
            }
            else
            {
                if (selector != null)
                {
                    selector.SelectPin();
                    break;
                }

                if (pin != null)
                {
                    HandlePinSelected(pin);
                    break;
                }
            }
        }
#endif
    }

    [Header("Wire Prefab & Look")]
    public VRWire wirePrefab;
    public Material[] wireMaterials;
    public Transform wireParent;

    // --- Simulator state ---
    private PinSelectable _pendingPin;
    private readonly Dictionary<(int, int), VRWire> _wires = new();

    // Delete mode is toggled from controller via WireDeleteModeInput
    public bool DeleteMode { get; private set; }
    public void SetDeleteMode(bool on)
    {
        DeleteMode = on;
        if (DeleteMode) _pendingPin = null; // safety: clear pending wire selection
    }
    public void ToggleDeleteMode() => SetDeleteMode(!DeleteMode);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called by PinSelectable.OnSelectEntered
    public void HandlePinSelected(PinSelectable pin)
    {
        if (pin == null) return;

        Debug.Log($"WireManager received pin: {pin.name}");

        if (DeleteMode)
        {
            DeleteAllWiresFromPin(pin);
            return;
        }

        if (_pendingPin == null)
        {
            _pendingPin = pin;
            Debug.Log($"First pin selected: {pin.name}");
            return;
        }

        var a = _pendingPin;
        var b = pin;

        if (a == b)
        {
            _pendingPin = null;
            return;
        }

        var key = MakeKey(a, b);

        if (_wires.TryGetValue(key, out var existing))
        {
            if (existing) Destroy(existing.gameObject);

            _wires.Remove(key);
            _pendingPin = null;
            return;
        }

        var wire = CreateLockedWireBetween(a, b);

        if (wire == null)
        {
            Debug.LogError($"Wire creation failed between {a.name} and {b.name}");
            _pendingPin = null;
            return;
        }

        _wires[key] = wire;
        Debug.Log($"Wire created successfully: {a.name} -> {b.name}");
        _pendingPin = null;
    }

    private void DeleteAllWiresFromPin(PinSelectable pin)
    {
        if (pin == null || _wires.Count == 0) return;
        int id = pin.GetInstanceID();

        List<(int, int)> toRemove = null;
        foreach (var kv in _wires)
        {
            var (x, y) = kv.Key;
            if (x == id || y == id)
            {
                (toRemove ??= new List<(int, int)>()).Add(kv.Key);
            }
        }
        if (toRemove == null) return;

        foreach (var k in toRemove)
        {
            if (_wires.TryGetValue(k, out var w) && w) Destroy(w.gameObject);
            _wires.Remove(k);
        }
    }

    private VRWire CreateLockedWireBetween(PinSelectable a, PinSelectable b)
    {
        if (wirePrefab == null)
        {
            Debug.LogError("WireManager wirePrefab is not assigned.");
            return null;
        }

        Vector3 pa = a.Anchor ? a.Anchor.position : a.transform.position;
        Vector3 pb = b.Anchor ? b.Anchor.position : b.transform.position;

        if (!IsFinite(pa) || !IsFinite(pb))
        {
            Debug.LogError($"Invalid pin position. A={pa}, B={pb}");
            return null;
        }

        if (wirePrefab.endA == null || wirePrefab.endB == null)
        {
            Debug.LogError("Wire prefab is missing End A or End B.");
            return null;
        }

        Vector3 difference = pb - pa;
        Vector3 fwd = difference.sqrMagnitude > 0.0001f
            ? difference.normalized
            : Vector3.forward;

        Vector3 mid = (pa + pb) * 0.5f;

        var wire = Instantiate(
            wirePrefab,
            mid,
            Quaternion.LookRotation(fwd, Vector3.up),
            wireParent
        );

        if (wire == null)
            return null;

        if (wireMaterials != null && wireMaterials.Length > 0)
        {
            var mat = wireMaterials[Random.Range(0, wireMaterials.Length)];
            wire.SetMaterial(mat);
        }

        SnapEndToPin(wire.endA, a);
        SnapEndToPin(wire.endB, b);

        Debug.Log($"Wire endpoints snapped: A={wire.endA.transform.position}, B={wire.endB.transform.position}");

        return wire;
    }

    private static void SnapEndToPin(WireEnd end, PinSelectable pin)
    {
        if (!end || !pin) return;
        var anchor = pin.Anchor ? pin.Anchor : pin.transform;

        end.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
        end.transform.SetParent(anchor, true);

        var rb = end.GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = true; rb.useGravity = false; }

        var grab = end.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab) grab.enabled = false;
    }

    private static (int, int) MakeKey(PinSelectable a, PinSelectable b)
    {
        int ia = a.GetInstanceID();
        int ib = b.GetInstanceID();
        return ia < ib ? (ia, ib) : (ib, ia);
    }

    private static bool IsFinite(Vector3 value)
    {
        return !float.IsNaN(value.x) &&
               !float.IsNaN(value.y) &&
               !float.IsNaN(value.z) &&
               !float.IsInfinity(value.x) &&
               !float.IsInfinity(value.y) &&
               !float.IsInfinity(value.z);
    }
}
