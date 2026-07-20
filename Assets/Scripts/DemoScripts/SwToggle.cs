using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SwToggle : MonoBehaviour
{
    public Transform lever;          // The mesh to move (defaults to this transform)
    public float travel = 0.002f;    // 2 mm
    public float moveTime = 0.08f;   // seconds to tween

    [Header("State")]
    public bool isOn = false;

    private Vector3 baseLocalPos;
    private Coroutine anim;
    private XRSimpleInteractable xri;

    void Awake()
    {
        if (!lever) lever = transform;
        baseLocalPos = lever.localPosition;

        xri = GetComponent<XRSimpleInteractable>();
        xri.selectEntered.AddListener(_ => Toggle());
    }

    void OnDestroy()
    {
        if (xri) xri.selectEntered.RemoveAllListeners();
    }

    public void Toggle()
    {
        isOn = !isOn;

        // On = -Z, Off = +Z
        Vector3 offset = new Vector3(0, 0, isOn ? -travel : 0);
        Vector3 target = baseLocalPos + offset;

        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(TweenLocalPosition(lever, target, moveTime));
    }

    IEnumerator TweenLocalPosition(Transform t, Vector3 to, float time)
    {
        Vector3 from = t.localPosition;
        float k = 0f;
        time = Mathf.Max(0.0001f, time);

        while (k < 1f)
        {
            k += Time.deltaTime / time;
            float s = k * k * (3f - 2f * k); // smoothstep
            t.localPosition = Vector3.LerpUnclamped(from, to, s);
            yield return null;
        }
        t.localPosition = to;
    }
}
