using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.VRTemplate;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum DCPowerSupplyKnob
{
    VoltageCoarse,
    VoltageFine,
    CurrentCoarse,
    CurrentFine
}

/// <summary>
/// Controls the imported DC power supply model and its four XR knobs.
///
/// Coarse voltage: 0-30 V
/// Fine voltage: -1 to +1 V trim
/// Coarse current: 0-5 A
/// Fine current: -0.1 to +0.1 A trim
/// </summary>
public class DCPowerSupplyController : MonoBehaviour
{
    [Serializable]
    public class FloatValueEvent : UnityEvent<float> { }

    [Serializable]
    public class BoolValueEvent : UnityEvent<bool> { }

    [Header("Model references")]
    [SerializeField] private Transform voltageCoarseKnob;
    [SerializeField] private Transform voltageFineKnob;
    [SerializeField] private Transform currentCoarseKnob;
    [SerializeField] private Transform currentFineKnob;
    [SerializeField] private Transform powerButton;

    [Header("Output limits")]
    [Min(0.01f)]
    [SerializeField] private float maximumVoltage = 30f;

    [Min(0.01f)]
    [SerializeField] private float maximumCurrent = 5f;

    [Min(0f)]
    [SerializeField] private float voltageFineRange = 1f;

    [Min(0f)]
    [SerializeField] private float currentFineRange = 0.1f;

    [Header("Initial settings")]
    [Range(0f, 1f)]
    [SerializeField] private float voltageCoarseValue = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float voltageFineValue = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float currentCoarseValue = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float currentFineValue = 0.5f;

    [SerializeField] private bool powerEnabled;

    [Header("Knob motion")]
    [SerializeField] private float minimumKnobAngle = -135f;
    [SerializeField] private float maximumKnobAngle = 135f;

    [Header("Events")]
    [SerializeField] private FloatValueEvent voltageChanged = new FloatValueEvent();
    [SerializeField] private FloatValueEvent currentLimitChanged = new FloatValueEvent();
    [SerializeField] private BoolValueEvent powerStateChanged = new BoolValueEvent();

    private XRKnob voltageCoarseControl;
    private XRKnob voltageFineControl;
    private XRKnob currentCoarseControl;
    private XRKnob currentFineControl;
    private DCPowerSupplyButton powerButtonControl;

    public float OutputVoltage =>
        Mathf.Clamp(
            voltageCoarseValue * maximumVoltage +
            (voltageFineValue - 0.5f) * voltageFineRange,
            0f,
            maximumVoltage);

    public float CurrentLimit =>
        Mathf.Clamp(
            currentCoarseValue * maximumCurrent +
            (currentFineValue - 0.5f) * currentFineRange,
            0f,
            maximumCurrent);

    public bool PowerEnabled => powerEnabled;

    private void Awake()
    {
        FindMissingReferences();

        voltageCoarseControl = ConfigureKnob(
            voltageCoarseKnob,
            DCPowerSupplyKnob.VoltageCoarse,
            voltageCoarseValue);

        voltageFineControl = ConfigureKnob(
            voltageFineKnob,
            DCPowerSupplyKnob.VoltageFine,
            voltageFineValue);

        currentCoarseControl = ConfigureKnob(
            currentCoarseKnob,
            DCPowerSupplyKnob.CurrentCoarse,
            currentCoarseValue);

        currentFineControl = ConfigureKnob(
            currentFineKnob,
            DCPowerSupplyKnob.CurrentFine,
            currentFineValue);

        ConfigurePowerButton();
        NotifyOutputState();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // Desktop fallback:
        // 1/2 voltage coarse, 3/4 voltage fine,
        // 5/6 current coarse, 7/8 current fine, P power.
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.VoltageCoarse, -0.02f);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.VoltageCoarse, 0.02f);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.VoltageFine, -0.02f);
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.VoltageFine, 0.02f);

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.CurrentCoarse, -0.02f);
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.CurrentCoarse, 0.02f);

        if (Keyboard.current.digit7Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.CurrentFine, -0.02f);
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
            AdjustKnob(DCPowerSupplyKnob.CurrentFine, 0.02f);

        if (Keyboard.current.pKey.wasPressedThisFrame)
            TogglePower();
    }

    private void FindMissingReferences()
    {
        voltageCoarseKnob = FindChild(voltageCoarseKnob, "Knob_VCoarse");
        voltageFineKnob = FindChild(voltageFineKnob, "Knob_VFine");
        currentCoarseKnob = FindChild(currentCoarseKnob, "Knob_ACoarse");
        currentFineKnob = FindChild(currentFineKnob, "Knob_AFine");
        powerButton = FindChild(powerButton, "Power_Button");
    }

    private Transform FindChild(Transform assigned, string objectName)
    {
        if (assigned != null)
            return assigned;

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
                return child;
        }

        Debug.LogWarning($"DC power supply part '{objectName}' was not found.", this);
        return null;
    }

    private XRKnob ConfigureKnob(
        Transform knobTransform,
        DCPowerSupplyKnob knobType,
        float initialValue)
    {
        if (knobTransform == null)
            return null;

        if (knobTransform.GetComponent<Collider>() == null)
        {
            SphereCollider collider = knobTransform.gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.035f;
        }

        XRKnob knob = knobTransform.GetComponent<XRKnob>();
        if (knob == null)
            knob = knobTransform.gameObject.AddComponent<XRKnob>();

        knob.handle = knobTransform;
        knob.clampedMotion = true;
        knob.minAngle = minimumKnobAngle;
        knob.maxAngle = maximumKnobAngle;
        knob.positionTrackedRadius = 0.04f;
        knob.value = initialValue;
        knob.onValueChange.AddListener(value => OnKnobChanged(knobType, value));

        return knob;
    }

    private void ConfigurePowerButton()
    {
        if (powerButton == null)
            return;

        if (powerButton.GetComponent<Collider>() == null)
        {
            BoxCollider collider = powerButton.gameObject.AddComponent<BoxCollider>();
            collider.center = Vector3.zero;
            collider.size = Vector3.one * 0.05f;
        }

        powerButtonControl = powerButton.GetComponent<DCPowerSupplyButton>();
        if (powerButtonControl == null)
            powerButtonControl = powerButton.gameObject.AddComponent<DCPowerSupplyButton>();

        powerButtonControl.Initialize(this);
    }

    private void OnKnobChanged(DCPowerSupplyKnob knobType, float value)
    {
        SetStoredKnobValue(knobType, value);
        NotifyOutputState();
    }

    public void AdjustKnob(DCPowerSupplyKnob knobType, float normalizedDelta)
    {
        float currentValue = GetStoredKnobValue(knobType);
        float newValue = Mathf.Clamp01(currentValue + normalizedDelta);
        SetStoredKnobValue(knobType, newValue);

        XRKnob knob = GetKnobControl(knobType);
        if (knob != null)
            knob.value = newValue;

        NotifyOutputState();
    }

    public void SetKnobValue(DCPowerSupplyKnob knobType, float normalizedValue)
    {
        normalizedValue = Mathf.Clamp01(normalizedValue);
        SetStoredKnobValue(knobType, normalizedValue);

        XRKnob knob = GetKnobControl(knobType);
        if (knob != null)
            knob.value = normalizedValue;

        NotifyOutputState();
    }

    public void SetVoltage(float voltage)
    {
        float normalized = Mathf.Clamp01(voltage / maximumVoltage);
        SetKnobValue(DCPowerSupplyKnob.VoltageCoarse, normalized);
    }

    public void SetCurrentLimit(float current)
    {
        float normalized = Mathf.Clamp01(current / maximumCurrent);
        SetKnobValue(DCPowerSupplyKnob.CurrentCoarse, normalized);
    }

    public void TogglePower()
    {
        SetPowerEnabled(!powerEnabled);
    }

    public void SetPowerEnabled(bool enabled)
    {
        if (powerEnabled == enabled)
            return;

        powerEnabled = enabled;
        powerStateChanged?.Invoke(powerEnabled);
        NotifyOutputState();
    }

    private float GetStoredKnobValue(DCPowerSupplyKnob knobType)
    {
        switch (knobType)
        {
            case DCPowerSupplyKnob.VoltageCoarse:
                return voltageCoarseValue;
            case DCPowerSupplyKnob.VoltageFine:
                return voltageFineValue;
            case DCPowerSupplyKnob.CurrentCoarse:
                return currentCoarseValue;
            default:
                return currentFineValue;
        }
    }

    private void SetStoredKnobValue(DCPowerSupplyKnob knobType, float value)
    {
        switch (knobType)
        {
            case DCPowerSupplyKnob.VoltageCoarse:
                voltageCoarseValue = value;
                break;
            case DCPowerSupplyKnob.VoltageFine:
                voltageFineValue = value;
                break;
            case DCPowerSupplyKnob.CurrentCoarse:
                currentCoarseValue = value;
                break;
            case DCPowerSupplyKnob.CurrentFine:
                currentFineValue = value;
                break;
        }
    }

    private XRKnob GetKnobControl(DCPowerSupplyKnob knobType)
    {
        switch (knobType)
        {
            case DCPowerSupplyKnob.VoltageCoarse:
                return voltageCoarseControl;
            case DCPowerSupplyKnob.VoltageFine:
                return voltageFineControl;
            case DCPowerSupplyKnob.CurrentCoarse:
                return currentCoarseControl;
            default:
                return currentFineControl;
        }
    }

    private void NotifyOutputState()
    {
        voltageChanged?.Invoke(OutputVoltage);
        currentLimitChanged?.Invoke(CurrentLimit);
    }
}

