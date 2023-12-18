using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TilePathSpawner : MonoBehaviour
{
    [Header("Tiles setting")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField, Min(1)] private float tileSize = 1;

    [Header("Platforms setting")]
    [SerializeField] private bool spawnPlatform;
    [SerializeField, Min(1)] private int platformSize = 1;
    [SerializeField] private float tileSpacing;

    [Header("Vertical position setting")]
    [SerializeField] private float startYPos;
    [SerializeField] private float yPosIncrement;
    private float currentYPos;

    public float TileSpacing => tileSize * tileSpacing;

    private Vector3[] path;

    private List<Tile> tiles;

    private void Awake()
    {
        tiles = new List<Tile>();
        currentYPos = startYPos;
    }

    public void SetPath(Vector3[] path)
    {
        this.path = path;
        tiles.Capacity = path.Length;

        for (int i = 0; i < path.Length; i++)
        {
            var node = GridUtils.PositionToNode(path[i], TileSpacing);

            var hexPosition = GridUtils.NodeToPosition(node, TileSpacing);

            if (tiles.Count == path.Length)
            {
                tiles[i].SetPositionAndScale(hexPosition, tileSize);
            }
            else
            {
                if (spawnPlatform)
                {
                    SpawnPlatform(node);
                }
                else
                {
                    SpawnTile(hexPosition);
                }

                currentYPos += yPosIncrement;
            }
        }
    }

    private Tile SpawnTile(Vector3 position)
    {
        position.y = currentYPos;
        var tile = Instantiate(tilePrefab);

        tile.Init(position, tileSize, Vector3.up * currentYPos);
        tiles.Add(tile);

        return tile;
    }

    private void SpawnPlatform(int2 node)
    {
        var nodes = GridUtils.NodesInRagne(node, platformSize);

        for (int i = 0; i < nodes.Count; i++)
        {
            var position = GridUtils.NodeToPosition(nodes[i], TileSpacing);

            SpawnTile(position);
        }
    }
}