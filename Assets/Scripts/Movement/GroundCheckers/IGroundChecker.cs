using UnityEngine;

public interface IGroundChecker
{
    bool GetGroundData(out float distanceToGround, out Vector3 groundNormal);
}

public class PathGroundChecker : IGroundChecker
{
    private Transform transform;
    private Vector3[] path;
    private float pathRadius;
    private float distanceToGround;
    private int lineSegment;

    public PathGroundChecker(Transform transform, Vector3[] path, float pathRadius, float distanceToGround, int lineSegment)
    {
        this.transform = transform;
        this.path = path;
        this.pathRadius = pathRadius;
        this.distanceToGround = distanceToGround;
        this.lineSegment = lineSegment;
    }

    public bool GetGroundData(out float distanceToGround, out Vector3 groundNormal)
    {
        var position = transform.position;
        var closestPoint = GetClosestPoint(position, lineSegment);
        ChangeLineSegment(closestPoint);

        distanceToGround = this.distanceToGround;
        groundNormal = closestPoint;

        return Vector3.Distance(closestPoint, position) < pathRadius;
    }

    public Vector3 GetClosestPoint(Vector3 position, int segmentIndex)
    {
        var firstPosition = path[segmentIndex];
        var nextPoint = path[(segmentIndex + 1) % path.Length];

        var pathDir = nextPoint - firstPosition;
        var dirToMover = position - firstPosition;

        var pathDirNorm = pathDir.normalized;

        var dot = Vector3.Dot(dirToMover, pathDirNorm);
        dot = Mathf.Clamp(dot, 0, pathDir.magnitude);

        var projection = firstPosition + pathDirNorm * dot;

        return projection;
    }

    public void ChangeLineSegment(Vector3 position)
    {
        var nextLineSegment = (lineSegment + 1) % path.Length;
        var dist = Vector3.Distance(position, path[nextLineSegment]);

        if (dist < 0.1f)
        {
            lineSegment = nextLineSegment;
        }
    }
}

public class RaycastGroundChecker : IGroundChecker
{
    private Transform transform;
    private float maxGroundDistance;
    private LayerMask groundLayer;

    public RaycastGroundChecker(Transform transform, float maxGroundDistance, LayerMask groundLayer)
    {
        this.transform = transform;
        this.maxGroundDistance = maxGroundDistance;
        this.groundLayer = groundLayer;
    }

    public bool GetGroundData(out float distanceToGround, out Vector3 groundNormal)
    {
        var ray = new Ray(transform.position, -transform.up);
        var isHit = Physics.Raycast(ray, out var hit, maxGroundDistance, groundLayer);

        distanceToGround = hit.distance;
        groundNormal = hit.normal;

        return isHit;
    }
}