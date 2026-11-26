using UnityEngine;

public class CableChainGenerator : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    [Min(4)] public int segmentCount = 24;
    [Min(0.005f)] public float segmentRadius = 0.012f;
    [Min(0f)] public float sagAmount = 0.12f;
    [Min(1f)] public float density = 1000f;

    private Rigidbody[] links;
    private LineRenderer cableLine;

    private void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError(
                "Assign Start Point and End Point on CableChainGenerator.",
                this
            );
            return;
        }

        BuildCable();
    }

    private void BuildCable()
    {
        Transform cableRoot = new GameObject("Generated Cable").transform;
        cableRoot.SetParent(transform, false);

        Rigidbody startAnchor = CreateAnchor(
            cableRoot, "Start Anchor", startPoint.position
        );
        Rigidbody endAnchor = CreateAnchor(
            cableRoot, "End Anchor", endPoint.position
        );

        links = new Rigidbody[segmentCount];

        float sphereVolume =
            (4f / 3f) * Mathf.PI * segmentRadius * segmentRadius * segmentRadius;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (i + 1f) / (segmentCount + 1f);

            Vector3 position = Vector3.Lerp(
                startPoint.position, endPoint.position, t
            );

            position += Vector3.down *
                        sagAmount *
                        Mathf.Sin(Mathf.PI * t);

            GameObject link = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            link.name = $"Cable Link {i + 1:00}";
            link.transform.SetParent(cableRoot, true);
            link.transform.position = position;
            link.transform.localScale = Vector3.one * (segmentRadius * 2f);

            Rigidbody body = link.AddComponent<Rigidbody>();
            body.mass = Mathf.Max(0.001f, density * sphereVolume);
            body.useGravity = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.linearDamping = 1.5f;
            body.angularDamping = 3f;
            body.solverIterations = 12;
            body.solverVelocityIterations = 12;

            links[i] = body;
        }

        for (int i = 1; i < links.Length; i++)
        {
            Rigidbody previous = links[i - 1];
            Rigidbody current = links[i];

            ConfigurableJoint joint = CreateJoint(current, previous);

            Vector3 midpoint = (previous.position + current.position) * 0.5f;
            joint.anchor = current.transform.InverseTransformPoint(midpoint);
            joint.connectedAnchor =
                previous.transform.InverseTransformPoint(midpoint);
        }

        ConfigurableJoint firstJoint = CreateJoint(links[0], startAnchor);
        firstJoint.anchor =
            links[0].transform.InverseTransformPoint(startPoint.position);
        firstJoint.connectedAnchor =
            startAnchor.transform.InverseTransformPoint(startPoint.position);

        int lastIndex = links.Length - 1;
        ConfigurableJoint lastJoint =
            CreateJoint(links[lastIndex], endAnchor);
        lastJoint.anchor =
            links[lastIndex].transform.InverseTransformPoint(endPoint.position);
        lastJoint.connectedAnchor =
            endAnchor.transform.InverseTransformPoint(endPoint.position);

        cableLine = cableRoot.gameObject.AddComponent<LineRenderer>();
        cableLine.useWorldSpace = true;
        cableLine.positionCount = segmentCount + 2;
        cableLine.widthMultiplier = segmentRadius;
        cableLine.numCapVertices = 4;
        cableLine.startColor = Color.red;
        cableLine.endColor = Color.red;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            cableLine.material = new Material(shader);
        }

        UpdateCableLine();
    }

    private Rigidbody CreateAnchor(Transform parent, string objectName, Vector3 position)
    {
        GameObject anchor = new GameObject(objectName);
        anchor.transform.SetParent(parent, true);
        anchor.transform.position = position;

        Rigidbody body = anchor.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        return body;
    }

    private ConfigurableJoint CreateJoint(Rigidbody body, Rigidbody connectedBody)
    {
        ConfigurableJoint joint =
            body.gameObject.AddComponent<ConfigurableJoint>();

        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        joint.lowAngularXLimit = new SoftJointLimit { limit = -25f };
        joint.highAngularXLimit = new SoftJointLimit { limit = 25f };
        joint.angularYLimit = new SoftJointLimit { limit = 25f };
        joint.angularZLimit = new SoftJointLimit { limit = 25f };

        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.projectionDistance = 0.02f;
        joint.projectionAngle = 10f;
        return joint;
    }

    private void LateUpdate()
    {
        if (cableLine != null && links != null)
        {
            UpdateCableLine();
        }
    }

    private void UpdateCableLine()
    {
        cableLine.SetPosition(0, startPoint.position);

        for (int i = 0; i < links.Length; i++)
        {
            cableLine.SetPosition(i + 1, links[i].position);
        }

        cableLine.SetPosition(segmentCount + 1, endPoint.position);
    }
}