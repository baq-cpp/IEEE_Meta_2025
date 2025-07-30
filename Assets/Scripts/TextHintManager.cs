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

    [SerializeField] private List<ComponentHints> hintsDatabase;
    [SerializeField] private Button getHintButton;
    [SerializeField] private RectTransform contentPanel; // the Content of the Scroll View
    [SerializeField] private GameObject hintEntryPrefab; // prefab with TMP_Text inside
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
        OfferHintForTop();
    }

    private void OfferHintForTop()
    {
        if (_stack1.Count == 0) return;
        var current = _stack1.Peek();

        if (_hintsLookup.TryGetValue(current, out var queue) && queue.Count > 0)
        {
            var hint = queue.Dequeue();
            AddHintToLog(current, hint);
        }
        else
        {
            _stack1.Pop();
            _stack2.Push(current);
            OfferHintForTop();
        }
    }

    private void AddHintToLog(string componentType, string hint)
    {
        // Instantiate a hint entry UI below the content panel
        var go = Instantiate(hintEntryPrefab, contentPanel);
        go.transform.SetParent(contentPanel, false);
        var textComp = go.GetComponentInChildren<TMP_Text>();
        //textComp.text = $"{componentType}: {hint}";
        if (textComp != null)
            textComp.text = $"{componentType}: {hint}";

        // Optional: auto-scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ResetStacks()
    {
        while (_stack2.Count > 0)
            _stack1.Push(_stack2.Pop());
    }
}
