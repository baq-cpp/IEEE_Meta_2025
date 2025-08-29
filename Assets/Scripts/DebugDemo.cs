using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DebugDemo : MonoBehaviour
{
    [SerializeField] private Renderer led1Renderer;
    [SerializeField] private Renderer led2Renderer;

    public void OnCheckCircuit()
    {
        EnableEmission(led1Renderer, true);
        EnableEmission(led2Renderer, true);
    }

    private void EnableEmission(Renderer rend, bool state)
    {
        if (rend == null) return;

        var mat = rend.material;
        if (state)
            mat.EnableKeyword("_EMISSION");
        else
            mat.DisableKeyword("_EMISSION");
    }
}

