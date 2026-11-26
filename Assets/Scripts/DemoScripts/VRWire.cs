// ===============================
// File: VRWire.cs
// ===============================
using UnityEngine;


/// <summary>
/// Draws the wire between two WireEnds using a LineRenderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class VRWire : MonoBehaviour
{
    public WireEnd endA;
    public WireEnd endB;
    public float slack = 0.02f; // small sag in meters
    public float clearanceHeight = 0.35f;
    public int segments = 12;

    public Material[] wireMaterials;

    private LineRenderer _lr;


    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = Mathf.Max(segments, 2);
        _lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _lr.receiveShadows = false;
        _lr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        _lr.useWorldSpace = true;

        RandomizeMaterial();
    }


    private void LateUpdate()
    {
        if (!endA || !endB) return;
        DrawCurve(endA.transform.position, endB.transform.position);
    }


    private void DrawCurve(Vector3 a, Vector3 b)
    {
        Vector3 mid = (a + b) * 0.5f;

        // Raise the middle of the wire above the objects
        Vector3 controlPoint = mid + Vector3.up * clearanceHeight;

        // Optional small sag
        controlPoint += Vector3.down * slack;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (segments - 1f);

            Vector3 p =
                Vector3.Lerp(
                    Vector3.Lerp(a, controlPoint, t),
                    Vector3.Lerp(controlPoint, b, t),
                    t
                );

            _lr.SetPosition(i, p);
        }
    }


    public void SetMaterial(Material mat)
    {
        if (!_lr) _lr = GetComponent<LineRenderer>();
        if (mat && _lr) _lr.sharedMaterial = mat;
    }

    public void RandomizeMaterial()
    {
        if (wireMaterials != null && wireMaterials.Length > 0)
        {
            int index = Random.Range(0, wireMaterials.Length);
            SetMaterial(wireMaterials[index]);
        }
    }
}