using UnityEngine;

public class XRMenuManager : MonoBehaviour
{
    [Header("Assign Canvases")]
    public GameObject MainMenuCanvas;
    public GameObject SchematicSelectCanvas;

    // This method goes in the Button OnClick()
    public void OnStartButtonPressed()
    {
        MainMenuCanvas.SetActive(false);
        SchematicSelectCanvas.SetActive(true);
    }
}

