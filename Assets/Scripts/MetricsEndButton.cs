using UnityEngine;

public class MetricsEndButton : MonoBehaviour
{
    public void EndLab()
    {
        if (MetricsManager.Instance == null)
        {
            Debug.LogError("MetricsManager not found.");
            return;
        }

        MetricsManager.Instance.EndLab();
    }
}
