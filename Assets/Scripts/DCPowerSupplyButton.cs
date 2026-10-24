using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class DCPowerSupplyButton : XRBaseInteractable
{
    private DCPowerSupplyController controller;

    public void Initialize(DCPowerSupplyController powerSupplyController)
    {
        controller = powerSupplyController;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (controller != null)
            controller.TogglePower();
    }
}

