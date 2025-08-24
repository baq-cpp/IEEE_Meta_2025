using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Hint
{
    public string title;
    [TextArea] public string details;
}

public class HintController : MonoBehaviour
{
    public List<Hint> allHints;
    public GameObject hintMenu;
    public GameObject openMenu;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI hintNum;
    public Button prevButton, nextButton;

    private int unlockCount = 0;
    private int currentIndex = -1;

    public ScrollRect scrollRect; // Assign your Scroll View's ScrollRect here
    public GameObject verticalScrollbar; // Assign the Vertical Scrollbar object here

    void Start()
    {
        hintMenu.SetActive(false);
        prevButton.onClick.AddListener(OnPrev);
        nextButton.onClick.AddListener(OnNext);
        UpdateButtons();
    }

    public void OnGetHint()
    {
        // Always unlock the next hint if any remain
        if (unlockCount < allHints.Count)
        {
            unlockCount++;
        }

        // Always displays the most recent hint unlocked
        currentIndex = unlockCount - 1;

        hintMenu.SetActive(true);
        openMenu.SetActive(false);
        DisplayCurrentHint();
    }

    void OnPrev()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayCurrentHint();
        }
    }

    void OnNext()
    {
        if (currentIndex < unlockCount - 1)
        {
            currentIndex++;
            DisplayCurrentHint();
        }
    }

    void DisplayCurrentHint()
    {
        hintText.text = allHints[currentIndex].details;
        hintNum.text = $"Hint {currentIndex + 1} of {unlockCount}";

        UpdateButtons();

        // Force a mesh update so preferredHeight is accurate
        hintText.ForceMeshUpdate();

        // Check if text height exceeds viewport height
        float textHeight = hintText.preferredHeight;
        float viewportHeight = scrollRect.viewport.rect.height;

        bool needsScroll = textHeight > viewportHeight;
        verticalScrollbar.SetActive(needsScroll);

        // Reset scroll position to top when showing a new hint
        scrollRect.verticalNormalizedPosition = 1f;
    }


    void UpdateButtons()
    {
        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < unlockCount - 1;
    }

    public void CloseHintMenu()
    {
        hintMenu.SetActive(false);
    }
}
