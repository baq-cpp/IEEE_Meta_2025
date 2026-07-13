using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CircuitChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Button currentSelected;

    public void OnButtonClick(Button clicked)
    {
        // Unhighlight previous
        if (currentSelected != null && currentSelected != clicked)
            SetHighlight(currentSelected, false);

        // Highlight new
        
        currentSelected = clicked;
        SetHighlight(currentSelected, true);
    }

    void SetHighlight(Button btn, bool on)
    {
        var colors = btn.colors;
        colors.normalColor = on ? colors.selectedColor : colors.disabledColor;  // or any color you choose
        btn.colors = colors;
    }
}
