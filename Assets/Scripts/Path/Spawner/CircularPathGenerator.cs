using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircularPathGenerator
{
    [SerializeField] private int count;
    [SerializeField] private float xRadius;
    [SerializeField] private float yRadius;

    [SerializeField] private Vector2 radiusMinMax;

    public List<Vector3> GeneratePath()
    {
        var path = new List<Vector3>(count);

        float increment = 360f / count;

        for (int i = 0; i < count; i++)
        {
            var angle = i * increment * Mathf.Deg2Rad;

            var r1 = xRadius + Random.Range(radiusMinMax.x, radiusMinMax.y);
            var r2 = yRadius + Random.Range(radiusMinMax.x, radiusMinMax.y);

            var x = r1 * Mathf.Cos(angle);
            var y = r2 * Mathf.Sin(angle);

            var point = new Vector3(x, 0, y);

            path.Add(point);
        }

        return path;
    }
}