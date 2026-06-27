using UnityEngine;
using UnityEngine.InputSystem;

public class XRMenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private InputActionProperty menuAction;

    private bool isOpen;

    void OnEnable()
    {
        menuAction.action.Enable();
    }

    void OnDisable()
    {
        menuAction.action.Disable();
    }

    void Update()
    {
        if (menuAction.action.WasPressedThisFrame())
        {
            isOpen = !isOpen;
            menuPanel.SetActive(isOpen);
            Debug.Log($"Menu toggled: {isOpen}");
        }
    }
}
