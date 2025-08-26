using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ComponentButton : MonoBehaviour
{
    public string componentType;       // e.g. "Resistor", "LED"
    private Button _button;
    private TextHintManager _hintManager;

    void Awake()
    {
        _button = GetComponent<Button>();
        _hintManager = FindFirstObjectByType<TextHintManager>();

        _button.onClick.AddListener(OnClick);
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        _hintManager.OnComponentInteracted(componentType);
        Debug.Log($"Button clicked: {componentType}");
    }
}
