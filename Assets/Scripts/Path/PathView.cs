using UnityEngine;

public class PathView : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private int lineCount;

    private LineRenderer[] lr;

    public void Init(Vector3[] points)
    {
        lr = new LineRenderer[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            lr[i] = Instantiate(linePrefab);
            lr[i].transform.SetParent(transform);
            lr[i].positionCount = points.Length;
        }

        lr[0].SetPositions(points);

        DrawAdditionalLines();
    }

    public void SetPath(Vector3[] points)
    {
        lr[0].SetPositions(points);
        DrawAdditionalLines();
    }

    private void DrawAdditionalLines()
    {
        for (int j = 1; j < lr.Length; j++)
        {
            var prevPosition = Vector3.zero;

            for (int i = 0; i < lr[0].positionCount; i++)
            {
                var currentPosition = lr[0].GetPosition(i);

                var cross = Vector3.Cross(currentPosition - prevPosition, Vector3.up);
                var newPosition = currentPosition + cross * j;

                prevPosition = currentPosition;

                lr[j].SetPosition(i, newPosition);
            }
        }
    }
}
