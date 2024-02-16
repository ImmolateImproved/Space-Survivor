using UnityEngine;

public class ObstacleSpawnerConfig : GameModeInit
{
    [SerializeField] private ObstacleProvider obstacleProvider;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float zOffset;

    [SerializeField] private float obstacleWidth = 10;
    [SerializeField] private int obstacleCount = 1;

    [SerializeField] private float obstacleMinSpeed;
    [SerializeField] private float obstacleMaxSpeed;

    public override void InitGameMode(GameModeConfig config)
    {
        startPosition = config.ObstacleSpawner.startPostion;
        zOffset = config.ObstacleSpawner.zOffset;
        obstacleWidth = config.ObstacleSpawner.obstacleWidth;
        obstacleCount = config.ObstacleSpawner.obstacleCount;

        obstacleMinSpeed = config.ObstacleSpawner.obstacleMinSpeed;
        obstacleMaxSpeed = config.ObstacleSpawner.obstacleMaxSpeed;
    }

    public void TrySpawn(PlatformHolder parentPlatform, int obstacleSpawned)
    {
        if (obstacleCount == 0) return;

        for (int i = 0; i < obstacleCount; i++)
        {
            var obstaclePrefab = obstacleProvider.GetRandom();

            if (obstacleSpawned <= 2)
            {
                obstaclePrefab = obstacleProvider.GetPrefabByColor(parentPlatform.PlatformColor);
            }

            Spawn(i, parentPlatform, obstaclePrefab);
        }

        if (obstacleCount < 2) return;
        var randObstacle = parentPlatform.GetRandomObstacle();
        randObstacle.SetColor(obstacleProvider.GetColorToMesh(parentPlatform.PlatformColor));
    }

    private void Spawn(int i, PlatformHolder parentPlatform, Obstacle obstaclePrefab)
    {
        var position = GetPosition(i, parentPlatform.PlatfomrSize);
        var obstacle = Instantiate(obstaclePrefab, parentPlatform.transform);
        obstacle.Init(position, obstacleWidth, parentPlatform.PlatfomrSize.x, obstacleMinSpeed, obstacleMaxSpeed);
        parentPlatform.AddObstacle(obstacle);
    }

    private Vector3 GetPosition(int i, Vector3 platformSize)
    {
        var position = new Vector3
        {
            x = GetRandomXPosition(platformSize.x),
            y = startPosition.y,
            z = startPosition.z + zOffset * i - platformSize.z / 2
        };

        return position;
    }

    private float GetRandomXPosition(float patformWidth)
    {
        var xPos = Random.Range(-1f, 1f);

        return xPos * (patformWidth / 2);
    }

    private int RandomSign()
    {
        return (int)Mathf.Sign(Random.Range(-1, 1));
    }
}