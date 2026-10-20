using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    public int selectedIndex = -1;
    public string[] sceneNames; // e.g. {"Scene1", "Scene2", "Scene3"}
    
    public void SelectOption(int index)
    {
        selectedIndex = index;
        Debug.Log("Selected schematic " + (index + 1));
    }

    public void OnStartPressed()
    {
        if (selectedIndex < 0 || selectedIndex >= sceneNames.Length)
        {
            Debug.LogWarning("No option selected!");
            return;
        }

        if (MetricsManager.Instance == null)
        {
            Debug.LogError("MetricsManager not found in menu scene.");
            return;
        }

        string selectedLab = sceneNames[selectedIndex];
        MetricsManager.Instance.PrepareLab(selectedLab);
        SceneManager.LoadScene(sceneNames[selectedIndex]);
       
    }

}

