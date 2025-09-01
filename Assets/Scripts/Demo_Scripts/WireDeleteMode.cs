using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRLikeDeleteMode : MonoBehaviour
{
    public enum ActivationMode { Toggle, Hold }
    public enum Hand { Right, Left }

    [Header("Binding (XR-style)")]
    [Tooltip("Leave empty to auto-bind to <XRController>{RightHand}/secondaryButton")]
    public InputActionProperty action; // XRI-style binding field
    public Hand hand = Hand.Right;

    [Header("Behavior")]
    public ActivationMode mode = ActivationMode.Toggle;

    [Header("Haptics (optional)")]
    public XRBaseController hapticController;
    [Range(0f, 1f)] public float hapticAmplitude = 0.35f;
    [Min(0f)] public float hapticDuration = 0.05f;

    private InputAction _runtimeAction; // only used if we auto-create

    void OnEnable()
    {
        // If no action assigned, auto-make one that binds to SecondaryButton
        if (action.reference == null && action.action == null)
        {
            var whichHand = hand == Hand.Right ? "RightHand" : "LeftHand";
            _runtimeAction = new InputAction(type: InputActionType.Button);
            _runtimeAction.AddBinding($"<XRController>{{{whichHand}}}/secondaryButton");
            _runtimeAction.Enable();
        }
        else
        {
            action.action.Enable();
        }

        // For Toggle mode, react on performed (button down edges).
        // For Hold mode, we’ll just poll in Update (simple & robust).
        if (mode == ActivationMode.Toggle)
            GetAction().performed += OnPerformed;
    }

    void OnDisable()
    {
        if (mode == ActivationMode.Toggle)
            GetAction().performed -= OnPerformed;

        if (_runtimeAction != null) _runtimeAction.Disable();
        else if (action.action != null) action.action.Disable();
    }

    void Update()
    {
        if (mode != ActivationMode.Hold) return;

        var pressed = GetAction().IsPressed();
        if (WireManager.Instance == null) return;

        WireManager.Instance.SetDeleteMode(pressed);
        if (pressed) Pulse();
    }

    private void OnPerformed(InputAction.CallbackContext _)
    {
        if (WireManager.Instance == null) return;
        WireManager.Instance.ToggleDeleteMode();
        Pulse();
    }

    private InputAction GetAction()
    {
        if (_runtimeAction != null) return _runtimeAction;
        return action.action;
    }

    private void Pulse()
    {
        if (hapticController != null)
            hapticController.SendHapticImpulse(hapticAmplitude, hapticDuration);
    }
}
