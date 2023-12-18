using UnityEngine;

public class PathProvider : InitializeMonoBeh
{
    [SerializeField] private PathGeneratorBase pathGenerator;
    [SerializeField] private Rigidbody player;
    [SerializeField] private float playerYPos;
    [SerializeField] private PathView view;
    [SerializeField] private TilePathSpawner tilePathSpawner;
    [SerializeField] private int pointsCount;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 noiseOffset;
    [SerializeField] private Vector3 startPosition;

    private Vector3 currentPosition;
    private Vector3[] path;

    public int Count => pointsCount;

    private void Update()
    {
        GeneratePath();
        view.SetPath(path);

        if (tilePathSpawner && tilePathSpawner.gameObject.activeSelf)
        {
            tilePathSpawner.SetPath(path);
        }

        enabled = false;
    }

    public override void Init()
    {
        path = new Vector3[pointsCount];

        GeneratePath();
        view.Init(path);

        SetPlayerPosition();
    }

    private void SetPlayerPosition()
    {
        if (!player) return;

        var playerPos = path[0];
        playerPos.y += playerYPos;
        player.position = playerPos;
        player.rotation = Quaternion.LookRotation(path[1] - path[0]);
    }

    private void GeneratePath()
    {
        currentPosition = startPosition;
        for (int i = 0; i < pointsCount; i++)
        {
            var direction = pathGenerator.GetNextDirection(currentPosition + noiseOffset);

            currentPosition += direction * moveSpeed;
            path[i] = currentPosition;
        }
    }
}