using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class OpenMenuManager : MonoBehaviour
{

    public InputActionProperty showButton;
    public GameObject firstMenu;
    public List<GameObject> otherMenus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject menu in otherMenus)
        {
            menu.SetActive(false);
        }

        firstMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame() && !AnyOtherMenusOpen())
        {
            ToggleFirstMenu();
        }
    }

    bool AnyOtherMenusOpen()
    {
        foreach (GameObject menu in otherMenus)
        {
            if (menu.activeSelf)
            {
                return true;
            }
        }
        return false;
    }


    void ToggleFirstMenu()
    {
        firstMenu.SetActive(!firstMenu.activeSelf);
    }
}