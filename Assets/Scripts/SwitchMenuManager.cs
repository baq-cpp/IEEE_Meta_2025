using UnityEngine;

public class SwitchMenuManager : MonoBehaviour
{
    [Header("Assign Canvases")]
    public GameObject CurrentCanvas;
    public GameObject NextCanvas;

    // This method goes in the Button OnClick()
    public void OnStartButtonPressed()
    {
        CurrentCanvas.SetActive(false);
        NextCanvas.SetActive(true);
    }
}

