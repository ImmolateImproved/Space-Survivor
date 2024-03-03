using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Transform myTransform;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private ColorFaction obstacleColor;
    private float speed;

    public ColorFaction ObstacleColor => obstacleColor;

    private void Awake()
    {
        myTransform = transform;
    }

    public void Init(Vector3 position, float width, float plaformWidth, float minSpeed, float maxSpeed)
    {
        speed = Random.Range(minSpeed, maxSpeed);

        myTransform.localPosition = position;

        var scale = myTransform.localScale;
        scale.x = width;
        myTransform.localScale = scale;

        StartCoroutine(Movement(position, plaformWidth));
    }

    public void SetColor(ColorToMesh colorToMesh)
    {
        obstacleColor = colorToMesh.color;
        meshFilter.mesh = colorToMesh.mesh;
    }

    private IEnumerator Movement(Vector2 offset, float plaformWidth)
    {
        var moveRange = plaformWidth - myTransform.localScale.x;

        while (true)
        {
            var pos = myTransform.localPosition;
            pos.x = Mathf.PingPong((offset.x + Time.time) * speed, moveRange) - moveRange / 2;

            myTransform.localPosition = pos;

            yield return null;
        }
    }
}