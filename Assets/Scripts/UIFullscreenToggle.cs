using UnityEngine;
using System.Collections;

public class UIFullscreenToggle : MonoBehaviour
{
    [Header("References")]
    public Camera vrCamera;

    [Header("Fullscreen Settings")]
    public float fullscreenDistance = 2f;
    public float fullscreenXOffset = 0f; // Positive = right, Negative = left
    public float transitionDuration = 0.5f;

    private bool isFullscreen = false;

    // Store original position/rotation/parent
    private Vector3 originalWorldPos;
    private Quaternion originalWorldRot;
    private Transform originalParent;

    private Coroutine transitionCoroutine;

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        originalParent = transform.parent;
        originalWorldPos = transform.position;
        originalWorldRot = transform.rotation;
    }

    public void ToggleFullscreen()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        if (isFullscreen)
        {
            // Exit fullscreen: unparent first
            transform.SetParent(originalParent, true);
            transitionCoroutine = StartCoroutine(MoveWorld(originalWorldPos, originalWorldRot));
        }
        else
        {
            // Enter fullscreen: parent to camera immediately so it follows instantly
            transform.SetParent(vrCamera.transform, true);

            Vector3 targetLocalPos = vrCamera.transform.InverseTransformPoint(
                vrCamera.transform.position
                + vrCamera.transform.forward * fullscreenDistance
                + vrCamera.transform.right * fullscreenXOffset
            );

            Quaternion targetLocalRot = Quaternion.identity; // Facing straight forward in camera space

            transitionCoroutine = StartCoroutine(MoveLocal(targetLocalPos, targetLocalRot));
        }

        isFullscreen = !isFullscreen;
    }

    private IEnumerator MoveWorld(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private IEnumerator MoveLocal(Vector3 targetLocalPos, Quaternion targetLocalRot)
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            transform.localPosition = Vector3.Lerp(startPos, targetLocalPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, targetLocalRot, t);
            yield return null;
        }

        transform.localPosition = targetLocalPos;
        transform.localRotation = targetLocalRot;
    }
}