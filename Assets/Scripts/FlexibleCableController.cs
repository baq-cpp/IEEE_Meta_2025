using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlexibleCableController : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    private LineRenderer cableLine;

    private void Awake()
    {
        cableLine = GetComponent<LineRenderer>();
        cableLine.positionCount = 2;
        cableLine.useWorldSpace = false;
    }

    private void LateUpdate()
    {
        if (startPoint == null || endPoint == null)
            return;

        cableLine.SetPosition(
            0,
            transform.InverseTransformPoint(startPoint.position)
        );

        cableLine.SetPosition(
            1,
            transform.InverseTransformPoint(endPoint.position)
        );
    }
}