using UnityEngine;

public class PowerSupplyDisplay : MonoBehaviour
{
    [SerializeField] private GameObject screenDisplay;

    private bool isScreenOn = false;

    private void Start()
    {
        SetScreen(false);
    }

    public void TogglePower()
    {
        SetScreen(!isScreenOn);
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