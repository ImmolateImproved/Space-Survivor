using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Transform myTransform;
    [SerializeField] private ColorFaction obstacleColor;
    [SerializeField] private float speed;

    public ColorFaction ObstacleColor => obstacleColor;

    private void Awake()
    {
        myTransform = transform;
    }

    public void Init(Vector3 position, float plaformWidth)
    {
        myTransform.localPosition = position;

        StartCoroutine(Movement(position, plaformWidth));
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