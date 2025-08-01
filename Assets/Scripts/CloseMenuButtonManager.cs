using UnityEngine;

public class CloseMenuButtonManager : MonoBehaviour
{
    [Tooltip("Drag your menu panel GameObject here (Canvas or Panel)")]
    public GameObject menuPanel;

    // Call this from your Close button's OnClick() event in the Inspector
    public void CloseMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }
}
