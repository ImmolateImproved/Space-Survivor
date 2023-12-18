using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class HexGridBuilder : MonoBehaviour
{
    [SerializeField] private int gridRadius;
    [SerializeField] private float tileSpacing;
    [SerializeField] private float tileSize;
    [SerializeField] private int nodesCount;
    [SerializeField] private float yPosIncrement;
    [SerializeField, Range(0, 1f)] private float fillPrecent;

    [SerializeField] private Tile tilePrefab;
    private ObjectPool<Transform> tilePool;

    [SerializeField] private Rigidbody player;

    private bool firstTile;

    private Grid grid;

    private float tileSlotRadiusPrev;
    private float tileSizePrev;

    private float yPos;

    private WaitForSeconds tileWfs;

    public float SlotSize => tileSize * tileSpacing;

    private void Awake()
    {
        InitPool();
        StartCoroutine(InitGrid());
    }

    private void OnDestroy()
    {
        grid.Dispose();
    }

    private void Update()
    {
        var slotRadiusChange = !Mathf.Approximately(tileSpacing, tileSlotRadiusPrev);
        var tileSizeChange = !Mathf.Approximately(tileSize, tileSizePrev);

        if (!(slotRadiusChange || tileSizeChange)) return;

        tileSlotRadiusPrev = tileSpacing;
        tileSizePrev = tileSize;

        foreach (var tile in grid.GetTiles())
        {
            var position = GridUtils.NodeToPosition(tile.Node, SlotSize);

            tile.SetPositionAndScale(position, tileSize);
        }
    }

    private void InitPool()
    {
        tileWfs = new WaitForSeconds(25);

        tilePool = new ObjectPool<Transform>(
            createFunc: () => Instantiate(tilePrefab, transform).transform,
            actionOnRelease: (t) => { t.gameObject.SetActive(false); },
            collectionCheck: false);
    }

    private IEnumerator InitGrid()
    {
        tileSlotRadiusPrev = tileSpacing;
        tileSizePrev = tileSize;

        nodesCount = HexTileNeighbors.CalculateTilesCount(gridRadius);

        //var nodes = BuildSquareGrid(gridRadius, Allocator.Persistent);//BuildGridBFS(nodesCount, Allocator.Persistent);//

        grid = new Grid(nodesCount);

        var wfs = new WaitForSeconds(0.05f);

        for (int j = 0; ; j++)
        {
            for (int i = 0; i < gridRadius; i++)
            {
                var rand = UnityEngine.Random.Range(0, 1f);

                if (rand > fillPrecent) continue;

                var node = new int2(i, j);
                node = GridUtils.OddrToAxial(node);

                var tile = Instantiate(tilePrefab, transform);
                var position = GridUtils.NodeToPosition(node, SlotSize);
                position.y = yPos;

                if (!firstTile)
                {
                    var playerPos = position;
                    playerPos.y += 3;
                    player.position = playerPos;
                    firstTile = true;
                }

                StartCoroutine(ReleaseTile(tile.transform));

                tile.Init(position, tileSize, default, node);

                grid.AddTile(node, tile);
            }

            yPos += yPosIncrement;
            yield return wfs;
        }
    }

    private IEnumerator ReleaseTile(Transform tile)
    {
        yield return tileWfs;
        tilePool.Release(tile);
    }
}