using Unity.Mathematics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int2 Node { get; private set; }

    public void Init(Vector3 position, float tileSize, int2 node)
    {
        Node = node;
        SetPositionAndScale(position, tileSize);
    }

    public void SetPositionAndScale(Vector3 position, float tileSize)
    {
        transform.position = position;
        transform.localScale = Vector3.one * tileSize;
    }
}