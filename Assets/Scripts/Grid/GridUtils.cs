using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class GridUtils
{
    public static readonly float2x2 NodeToPositionMatrix = new float2x2
    {
        c0 = new float2(math.sqrt(3), 0),
        c1 = new float2(math.sqrt(3) / 2f, 3 / 2f)
    };

    public static readonly float2x2 PositionToNodeMatrix = math.inverse(NodeToPositionMatrix);

    public static int Distance(int2 nodeA, int2 nodeB)
    {
        var result = (math.abs(nodeA.x - nodeB.x) + math.abs(nodeA.x + nodeA.y - nodeB.x - nodeB.y) + math.abs(nodeA.y - nodeB.y)) / 2;

        return result;
    }

    public static int2 PositionToNode(float3 position, float tileSpacing)
    {
        var fractionalNodeCoordinate = math.mul(PositionToNodeMatrix, new float2(position.x, position.z)) / tileSpacing;

        return AxialRound(fractionalNodeCoordinate);
    }

    public static float3 NodeToPosition(int2 node, float tileSpacing)
    {
        var position = tileSpacing * math.mul(NodeToPositionMatrix, node);

        return new float3(position.x, 0, position.y);
    }

    public static List<int2> NodesInRagne(int2 centerNode, int range)
    {
        var result = new List<int2>();

        for (int x = -range; x <= range; x++)
        {
            var start = Mathf.Max(-range, -x - range);
            var end = Mathf.Min(range, -x + range);

            for (int y = start; y <= end; y++)
            {
                result.Add(new int2(x, y) + centerNode);
            }
        }

        return result;
    }

    public static int2 AxialToOddr(int2 node)
    {
        var col = node.x + (node.y - (node.y & 1)) / 2;
        var row = node.y;

        return new int2(col, row);
    }

    public static int2 OddrToAxial(int2 node)
    {
        var q = node.x - (node.y - (node.y & 1)) / 2;
        var r = node.y;

        return new int2(q, r);
    }

    private static int2 AxialRound(float2 position)
    {
        var x = position.x;
        var y = position.y;

        var xgrid = Mathf.RoundToInt(x);
        var ygrid = Mathf.RoundToInt(y);
        x -= xgrid;
        y -= ygrid;

        if (math.abs(x) >= math.abs(y))
        {
            return new int2(xgrid + Mathf.RoundToInt(x + 0.5f * y), ygrid);
        }

        return new int2(xgrid, ygrid + Mathf.RoundToInt(y + 0.5f * x));
    }
}