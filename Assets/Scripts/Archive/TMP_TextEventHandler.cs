using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TMP_TextEventHandler : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text text;

    // UnityEvent so other scripts can subscribe
    [System.Serializable]
    public class LinkClickedEvent : UnityEvent<string, string, int> { }
    public LinkClickedEvent onLinkClicked = new LinkClickedEvent();

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            string linkText = linkInfo.GetLinkText();

            Debug.Log($"Clicked link: {linkId}");
            onLinkClicked.Invoke(linkId, linkText, linkIndex);
        }
    }
}