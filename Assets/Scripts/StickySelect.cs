using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class StickySelect : MonoBehaviour
{
     private bool isSelected = false;

    public void Toggle()
    {
        isSelected = !isSelected;
        
    }
}
