using Unity.Mathematics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int2 Node { get; private set; }

    private Vector3 offset;

    public void Init(Vector3 position, float tileSize, Vector3 offset, int2 node = default)
    {
        Node = node;
        this.offset = offset;
        SetPositionAndScale(position, tileSize);
    }

    public void SetPositionAndScale(Vector3 position, float tileSize)
    {
        position += offset;
        transform.position = position;
        transform.localScale = Vector3.one * tileSize;
    }
}