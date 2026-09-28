using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Adjustable resistor for the VR breadboard project.
/// The resistor remains a two-terminal component for breadboard placement.
/// </summary>
public class VariableResistor : MonoBehaviour
{
    [Header("Resistance range")]
    [Min(0f)]
    [SerializeField] private float minimumResistance = 0f;

    [Min(0.001f)]
    [SerializeField] private float maximumResistance = 10000f;

    [Range(0f, 1f)]
    [SerializeField] private float normalizedResistance = 0.5f;

    [Tooltip("Amount added when the controller Activate action is pressed.")]
    [Min(0.001f)]
    [SerializeField] private float adjustmentStep = 100f;

    [Header("Optional references")]
    [SerializeField] private Transform knob;
    [SerializeField] private UnityEvent<float> resistanceChanged;

    private XRGrabInteractable grabInteractable;

    public float Resistance => Mathf.Lerp(minimumResistance, maximumResistance, normalizedResistance);
    public float MinimumResistance => minimumResistance;
    public float MaximumResistance => maximumResistance;
    public float NormalizedResistance => normalizedResistance;

    private void Awake()
    {
        EnsureKnob();
        ApplyVisualState();

        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.activated.AddListener(OnActivated);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.activated.RemoveListener(OnActivated);
    }

    private void Update()
    {
        // Desktop fallback for testing without a VR controller.
        if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.Equals))
            IncreaseResistance();
        if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.Minus))
            DecreaseResistance();
    }

    private void OnActivated(ActivateEventArgs args)
    {
        IncreaseResistance();
    }

    public void IncreaseResistance()
    {
        SetResistance(Resistance + adjustmentStep);
    }

    public void DecreaseResistance()
    {
        SetResistance(Resistance - adjustmentStep);
    }

    public void SetResistance(float resistanceOhms)
    {
        float range = Mathf.Max(0.001f, maximumResistance - minimumResistance);
        SetNormalizedResistance((resistanceOhms - minimumResistance) / range);
    }

    public void SetNormalizedResistance(float value)
    {
        float previousResistance = Resistance;
        normalizedResistance = Mathf.Clamp01(value);
        ApplyVisualState();

        if (!Mathf.Approximately(previousResistance, Resistance))
            resistanceChanged?.Invoke(Resistance);
    }

    private void EnsureKnob()
    {
        if (knob != null)
            return;

        GameObject knobObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        knobObject.name = "VariableResistor_Knob";
        knobObject.transform.SetParent(transform, false);
        knobObject.transform.localPosition = new Vector3(0f, 0.0035f, 0f);
        knobObject.transform.localScale = new Vector3(0.0022f, 0.00065f, 0.0022f);
        knob = knobObject.transform;

        Collider knobCollider = knobObject.GetComponent<Collider>();
        if (knobCollider != null)
            knobCollider.isTrigger = true;

        Renderer knobRenderer = knobObject.GetComponent<Renderer>();
        if (knobRenderer != null)
            knobRenderer.material = CreateMaterial(new Color(0.08f, 0.08f, 0.08f));

        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.name = "KnobIndicator";
        indicator.transform.SetParent(knob, false);
        indicator.transform.localPosition = new Vector3(0f, 1.05f, 0f);
        indicator.transform.localScale = new Vector3(0.18f, 0.12f, 0.75f);

        Collider indicatorCollider = indicator.GetComponent<Collider>();
        if (indicatorCollider != null)
            indicatorCollider.enabled = false;

        Renderer indicatorRenderer = indicator.GetComponent<Renderer>();
        if (indicatorRenderer != null)
            indicatorRenderer.material = CreateMaterial(new Color(1f, 0.65f, 0.05f));
    }

    private void ApplyVisualState()
    {
        if (knob == null)
            return;

        knob.localRotation = Quaternion.Euler(
            0f,
            Mathf.Lerp(-135f, 135f, normalizedResistance),
            0f);
    }

    private static Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader) { color = color };
        return material;
    }
}

