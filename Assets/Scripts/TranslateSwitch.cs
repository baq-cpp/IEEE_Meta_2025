using UnityEngine;

public class TranslateSwitch : MonoBehaviour
{
    [SerializeField] private Transform handle;   // drag your SwitchHandle child here
    [SerializeField] private float offset = 0.002f; // 2 mm in meters

    private bool isOn = false;

    public void ToggleSwitch()
    {
        isOn = !isOn;
        MoveHandle();
    }

    private void MoveHandle()
    {
        if (handle == null) return;

        // local position relative to parent
        var pos = handle.localPosition;
        pos.z = isOn ? offset : -offset;
        handle.localPosition = pos;
    }
}
