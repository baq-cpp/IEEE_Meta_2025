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
        public List<string> hints;
    }


    public GameObject hintMenu;
    public GameObject openMenu;

    [SerializeField] private List<ComponentHints> hintsDatabase = new List<ComponentHints>();
    [SerializeField] private Button getHintButton;
    [SerializeField] private TMP_Text hintLogText; // The single TMP_Text inside your ScrollView Content
    [SerializeField] private ScrollRect scrollRect;

    private Dictionary<string, Queue<string>> _hintsLookup;
    private Stack<string> _stack1 = new Stack<string>();
    private Stack<string> _stack2 = new Stack<string>();

    void Awake()
    {
        _hintsLookup = new Dictionary<string, Queue<string>>();
        foreach (var entry in hintsDatabase)
            _hintsLookup[entry.componentType] = new Queue<string>(entry.hints);

        getHintButton.onClick.AddListener(OnGetHintClicked);
        ClearHintLog();
    }

    public void OnComponentInteracted(string componentType)
    {
        if (_stack2.Count > 0 && (_stack1.Count == 0 || _stack1.Peek() != componentType))
            ResetStacks();

        if (_stack1.Count == 0 || _stack1.Peek() != componentType)
            _stack1.Push(componentType);
    }

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

    private void AppendHintToLog(string componentType, string text)
    {
        string newEntry = $"- <b>{componentType}</b>: {text}";
        if (!string.IsNullOrEmpty(hintLogText.text))
            hintLogText.text += "\n\n" + newEntry;
        else
            hintLogText.text = newEntry;

        Debug.Log($"[Hint for {componentType}]: {text}");

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ClearHintLog()
    {
        hintLogText.text = "";
    }
}
