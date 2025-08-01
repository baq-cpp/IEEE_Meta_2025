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

    [Header("Hint Log UI")]
    [SerializeField] private Transform hintLogContent; // The Content GameObject inside the ScrollView
    [SerializeField] private GameObject hintEntryPrefab; // A TMP_Text prefab for each new hint

    private Dictionary<string, Queue<string>> _hintsLookup;
    private Stack<string> _stack1 = new Stack<string>();
    private Stack<string> _stack2 = new Stack<string>();


    void Start()
    {
        hintMenu.SetActive(false);
    }

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
        string fullHint = $"{componentType}: {text}";
        Debug.Log($"[Hint for {componentType}]: {text}");
        Debug.Log("Showing Hint: " + fullHint);

        GameObject newHintObj = Instantiate(hintEntryPrefab, hintLogContent);
        TMP_Text hintTMP = newHintObj.GetComponentInChildren<TMP_Text>();
        if (hintTMP != null)
            hintTMP.text = fullHint;
    }

    public void CloseHintMenu()
    {
        hintMenu.SetActive(false);
    }
}
