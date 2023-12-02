using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum HexDirections
{
    Right, BottomRight, BottomLeft, Left, TopLeft, TopRight
}

public static class HexDirectionsExtentions
{
    public const int DIRECTIONS_COUNT = 6;

    public static HexDirections GetNextDirection(this HexDirections direction)
    {
        var intialDirection = ((int)direction + 1) % DIRECTIONS_COUNT;

        return (HexDirections)intialDirection;
    }

    public static HexDirections GetOppositeDirection(this HexDirections direction)
    {
        var newDirection = ((int)direction + DIRECTIONS_COUNT / 2) % DIRECTIONS_COUNT;

        return (HexDirections)newDirection;
    }
}

public static class HexTileNeighbors
{
    public static readonly int2[] Neighbors = new int2[]
    {
        new int2(1, 0),
        new int2(1, -1),
        new int2(0, -1),
        new int2(-1, 0),
        new int2(-1, 1),
        new int2(0, 1)
    };

    public static int2 GetNeighborNode(int2 current, int2 direction)
    {
        return current + direction;
    }

    public static int2 GetClosestDirection(Vector2 direction)
    {
        var closets = default(int2);
        var min = int.MaxValue;

        var intDirection = new int2
        {
            x = Mathf.RoundToInt(direction.x),
            y = Mathf.RoundToInt(direction.y)
        };

        for (int i = 0; i < Neighbors.Length; i++)
        {
            var current = math.dot(intDirection, Neighbors[i]);

            if (current < min)
            {
                min = current;
                closets = Neighbors[i];
            }
        }

        return closets;
    }

    public static bool IsNeighbors(this NativeArray<int2> array, int2 nodeA, int2 nodeB)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (GetNeighborNode(nodeA, array[i]).Equals(nodeB))
            {
                return true;
            }
        }

        return false;
    }

    public static int CalculateTilesCount(int radius)
    {
        var neighborCount = 6;

        var count = 1;

        for (int i = 1; i <= radius; i++)
        {
            count += neighborCount * i;
        }

        return count;
    }
}