using UnityEngine;
using TMPro;


public class InputFieldGrabber : MonoBehaviour
{
    [Header("Name of player")]
    [SerializeField] private string usertextinput;
    [Header("Id of player")]
    [SerializeField] private int usernumberinput;


    public void GrabText (string textinput)
    
    {
        if(textinput == string.Empty)
        {
            return;
        }

        usertextinput = textinput;
        
    }
    public void Grabnumber (string numberinput)
    {
        if(numberinput == string.Empty)
        {
            return;
        }

     

        usernumberinput  = int.Parse(numberinput);     
    }
  
    
}
