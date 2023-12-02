using UnityEngine;

public abstract class PathGeneratorBase : ScriptableObject
{
    [SerializeField] protected float scale;

    public virtual Vector3 GetNextDirection(Vector2 currentPosition)
    {
        return default;
    }

    public Vector3 GetNextDirection(Vector3 currentPosition)
    {
        var position2D = new Vector2(currentPosition.x, currentPosition.z);

        return GetNextDirection(position2D);
    }
}