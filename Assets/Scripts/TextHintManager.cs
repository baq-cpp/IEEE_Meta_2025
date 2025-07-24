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


    [SerializeField]
    private List<ComponentHints> hintsDatabase = new List<ComponentHints>();

    [SerializeField] private Button getHintButton;
    [SerializeField] private TMP_Text hintText;

    private Dictionary<string, Queue<string>> _hintsLookup;
    private Stack<string> _stack1 = new Stack<string>();
    private Stack<string> _stack2 = new Stack<string>();

    void Awake()
    {
        _hintsLookup = new Dictionary<string, Queue<string>>();
        foreach (var entry in hintsDatabase)
            _hintsLookup[entry.componentType] = new Queue<string>(entry.hints);

        getHintButton.onClick.AddListener(OnGetHintClicked);
        ClearHintText();
    }

    /// Called when user places or interacts with a component
    public void OnComponentInteracted(string componentType)
    {
        // If new component and stack2 has items, reset so new is top
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
            ShowHint(current, hint);
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

    private void ShowHint(string componentType, string text)
    {
        hintText.text = $"{componentType}: {text}";
        Debug.Log($"[Hint for {componentType}]: {text}");
        // TODO: Replace with UI popup, voice, etc.
    }

    private void ClearHintText()
    {
        hintText.text = "";
    }
}
