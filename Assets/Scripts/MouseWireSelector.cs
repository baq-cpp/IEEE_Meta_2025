using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MouseWireSelector : MonoBehaviour
{
    private PinSelectable pin;
    public PinSelectable Pin => pin;

    private void Awake()
    {
        pin = GetComponentInChildren<PinSelectable>(true);
    }

    public void SelectPin()
    {
        if (WireManager.Instance == null)
        {
            Debug.LogError("WireManager was not found.");
            return;
        }

        if (pin == null)
        {
            Debug.LogError("PinSelectable was not found.");
            return;
        }

        WireManager.Instance.HandlePinSelected(pin);
        Debug.Log($"Mouse selected wire pin: {name}");
    }
}
