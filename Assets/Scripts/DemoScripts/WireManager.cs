// WireManager.cs — point & click wiring + pin-tap delete mode
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour
{
    public static WireManager Instance { get; private set; }

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
    }

    // Called by PinSelectable.OnSelectEntered
    public void HandlePinSelected(PinSelectable pin)
    {
        if (pin == null) return;

        // NEW: if in delete mode, single tap nukes all wires on that pin
        if (DeleteMode)
        {
            DeleteAllWiresFromPin(pin);
            return;
        }

        // --- Normal wiring flow ---
        if (_pendingPin == null)
        {
            _pendingPin = pin;
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
        _wires[key] = wire;
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
        Vector3 pa = a.Anchor ? a.Anchor.position : a.transform.position;
        Vector3 pb = b.Anchor ? b.Anchor.position : b.transform.position;
        var mid = (pa + pb) * 0.5f;
        var fwd = (pb - pa).sqrMagnitude > 0.0001f ? (pb - pa).normalized : Vector3.forward;

        var wire = Instantiate(wirePrefab, mid, Quaternion.LookRotation(fwd, Vector3.up), wireParent);

        if (wireMaterials != null && wireMaterials.Length > 0)
        {
            var mat = wireMaterials[Random.Range(0, wireMaterials.Length)];
            wire.SetMaterial(mat);
        }

        SnapEndToPin(wire.endA, a);
        SnapEndToPin(wire.endB, b);

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
}
