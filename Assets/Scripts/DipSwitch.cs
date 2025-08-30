using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
public class DipSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isselected = false;

    public void OnSelectEntered(SelectEnterEventArgs args)  
    {

        isselected = !isselected;

        Vector3 pos = transform.localPosition;

        if (isselected)
        {
            pos.z -= -.00183f;
        }
        transform.localPosition = pos;


       



    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
