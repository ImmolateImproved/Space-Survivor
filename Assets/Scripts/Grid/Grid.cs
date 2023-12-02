using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Grid : IDisposable
{
    private NativeHashSet<int2> nodes;

    private Dictionary<int2, Tile> tiles;

    public Grid(int capacity)
    {
        nodes = new NativeHashSet<int2>(capacity, Allocator.Persistent);
        tiles = new Dictionary<int2, Tile>(capacity);
    }

    public Grid(NativeHashSet<int2> nodes)
    {
        this.nodes = nodes;
        tiles = new Dictionary<int2, Tile>(nodes.Count());
    }

    public bool AddTile(int2 node, Tile tile)
    {
        return tiles.TryAdd(node, tile);
    }

    public bool AddNodeAndTile(int2 node, Tile tile)
    {
        return AddNode(node) && tiles.TryAdd(node, tile);
    }

    public bool GetTile(int2 node, out Tile tile)
    {
        return tiles.TryGetValue(node, out tile);
    }

    public IEnumerable<Tile> GetTiles()
    {
        foreach (var tile in tiles)
        {
            yield return tile.Value;
        }
    }

    public int2 GetNeighborNodeFromDirection(int2 currentNode, HexDirections direction)
    {
        var dir = HexTileNeighbors.Neighbors[(int)direction];

        var nextNode = currentNode + dir;

        return nextNode;
    }

    public NativeList<int2> GetNeighborNodes(int2 startNode)
    {
        var neighbors = new NativeList<int2>(6, Allocator.Temp);

        var direction = HexDirections.BottomLeft;

        for (int i = 0; i < HexDirectionsExtentions.DIRECTIONS_COUNT; i++)
        {
            var nextNode = GetNeighborNodeFromDirection(startNode, direction);
            direction = direction.GetNextDirection();

            if (HasNode(nextNode))
            {
                neighbors.Add(nextNode);
            }
        }
        return neighbors;
    }

    public bool AddNode(int2 node)
    {
        return nodes.Add(node);
    }

    public bool RemoveNode(int2 node)
    {
        return nodes.Remove(node);
    }

    public bool HasNode(int2 nodeIndex)
    {
        return nodes.Contains(nodeIndex);
    }

    public void Dispose()
    {
        nodes.Dispose();
    }
}