using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextHintManager : MonoBehaviour
{
    [Serializable]
    public class ComponentHints
    {
        public string componentType;
        public List<HintEntry> hints;
    }

    [Serializable]
    public class HintEntry
    {
        [TextArea] public string text;
        public Sprite image; // Optional image for this specific hint
    }

    [Serializable]
    public class ComponentColor
    {
        public string componentType;
        public Color color;
    }

    [Header("UI References")]
    public GameObject hintMenu;
    public GameObject openMenu;
    [SerializeField] private Button getHintButton;
    [SerializeField] private TMP_Text hintLogText; // Single TMP_Text for log
    [SerializeField] private ScrollRect scrollRect;

    [Header("Image Popup")]
    [SerializeField] private GameObject imagePopupPanel;
    [SerializeField] private TMP_Text popupHintText;
    [SerializeField] private Image popupHintImage;
    [SerializeField] private Button popupCloseButton;

    [Header("Databases")]
    [SerializeField] private List<ComponentHints> hintsDatabase = new List<ComponentHints>();
    [SerializeField] private List<ComponentColor> componentColors = new List<ComponentColor>();

    // Internal
    private Dictionary<string, Queue<HintEntry>> _hintsLookup;
    private Dictionary<string, Color> _colorLookup;
    private Stack<string> _stack1 = new Stack<string>();
    private Stack<string> _stack2 = new Stack<string>();

    void Awake()
    {
        // Build lookup dictionaries...
        _hintsLookup = new Dictionary<string, Queue<HintEntry>>();
        foreach (var entry in hintsDatabase)
            _hintsLookup[entry.componentType] = new Queue<HintEntry>(entry.hints);

        _colorLookup = new Dictionary<string, Color>();
        foreach (var entry in componentColors)
            _colorLookup[entry.componentType] = entry.color;

        getHintButton.onClick.AddListener(OnGetHintClicked);

        if (popupCloseButton != null)
            popupCloseButton.onClick.AddListener(() => ClosePopupMenu());

        // Attach TMP_TextEventHandler and subscribe
        var handler = hintLogText.GetComponent<TMP_TextEventHandler>();
        if (handler == null) handler = hintLogText.gameObject.AddComponent<TMP_TextEventHandler>();
        handler.onLinkClicked.AddListener(OnLinkClicked);

        ClearHintLog();
    }


    // Track component interactions
    public void OnComponentInteracted(string componentType)
    {
        if (_stack2.Count > 0 && (_stack1.Count == 0 || _stack1.Peek() != componentType))
            ResetStacks();

        if (_stack1.Count == 0 || _stack1.Peek() != componentType)
            _stack1.Push(componentType);
    }

    // Button handler for Get Hint
    public void OnGetHintClicked()
    {
        hintMenu.SetActive(true);
        openMenu.SetActive(false);
        OfferHintForTop();
    }

    private void OfferHintForTop()
    {
        if (_stack1.Count == 0) return;

        var current = _stack1.Peek();
        if (_hintsLookup.TryGetValue(current, out var queue) && queue.Count > 0)
        {
            var hint = queue.Dequeue();
            AppendHintToLog(current, hint);
        }
        else
        {
            _stack1.Pop();
            _stack2.Push(current);
            OfferHintForTop();
        }
    }

    private void ResetStacks()
    {
        while (_stack2.Count > 0)
            _stack1.Push(_stack2.Pop());
    }

    // Append text + optional "Show Image"
    private void AppendHintToLog(string componentType, HintEntry hintEntry)
    {
        Color col = _colorLookup.ContainsKey(componentType) ? _colorLookup[componentType] : Color.white;
        string hexColor = ColorUtility.ToHtmlStringRGB(col);

        string newEntry = $"- <color=#{hexColor}><b>{componentType}</b></color>: {hintEntry.text}";

        if (!string.IsNullOrEmpty(hintLogText.text))
            hintLogText.text += "\n\n" + newEntry;
        else
            hintLogText.text = newEntry;

        Debug.Log($"[Hint for {componentType}]: {hintEntry.text}");

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        // If this hint has an image, add clickable link
        if (hintEntry.image != null)
        {
            string uniqueId = Guid.NewGuid().ToString();
            string linkId = $"showimage|{componentType}|{uniqueId}";

            hintLogText.text += $"\n<link={linkId}><color=#0000EE><u>Show Image</u></color></link>";

            // Store temp data in PlayerPrefs or dictionary (better: dictionary)
            _pendingImageHints[linkId] = (newEntry, hintEntry.image);
        }
    }

    // Handle TMP link clicks
    private Dictionary<string, (string, Sprite)> _pendingImageHints = new Dictionary<string, (string, Sprite)>();

    private void OnLinkClicked(string linkId, string linkText, int linkIndex)
    {
        if (linkId.StartsWith("showimage") && _pendingImageHints.ContainsKey(linkId))
        {
            var (text, image) = _pendingImageHints[linkId];
            popupHintText.text = text;
            popupHintImage.sprite = image;
            
            imagePopupPanel.SetActive(true);
            hintMenu.SetActive(false);
        }
    }

    public void ClosePopupMenu()
    {
        imagePopupPanel.SetActive(false);
        hintMenu.SetActive(true);
    }

    private void ClearHintLog()
    {
        hintLogText.text = "";
        _pendingImageHints.Clear();
    }
}
