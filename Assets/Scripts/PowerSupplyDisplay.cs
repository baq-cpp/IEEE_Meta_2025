using TMPro;
using UnityEngine;

public class PowerSupplyDisplay : MonoBehaviour
{
    [SerializeField] private GameObject screenDisplay;
    [SerializeField] private TMP_Text voltageText;

    public void SetPowerState(bool state)
    {
        SetScreen(state);
    }

    private bool isScreenOn = false;

    private void Awake()
    {
        if (voltageText == null && screenDisplay != null)
        {
            voltageText = screenDisplay.GetComponent<TMP_Text>();
        }
    }

    private void Start()
    {
        SetVoltage(0f);
        SetScreen(false);
    }

    public void TogglePower()
    {
        SetScreen(!isScreenOn);
    }

    public void SetVoltage(float voltage)
    {
        if (voltageText != null)
        {
            voltageText.text = $"{voltage:0.00} V";
        }
    }

    private void SetScreen(bool state)
    {
        isScreenOn = state;

        if (screenDisplay != null)
        {
            screenDisplay.SetActive(state);
        }
    }
}