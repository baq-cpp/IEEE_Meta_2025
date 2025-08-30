using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
public class DipSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isOn = false;
    private Quaternion onRotation;
    private Quaternion offRotation;

    private void Start()
    {
        // Store the ON and OFF rotations
        offRotation = transform.localRotation;
        onRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(-20, 0, 0)); // tweak as needed
    }

    public void Toggle()
    {
        isOn = !isOn;
        transform.localRotation = isOn ? onRotation : offRotation;
        Debug.Log("Switch is now: " + (isOn ? "ON" : "OFF"));
    }
}
