using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] private WaveRingsManager waveManager;
    [SerializeField] private CircularPathGenerator pathGenerator;
    [SerializeField] private PlatformProvider platformProvider;
    [SerializeField] private Vector3 startPostion;
    [SerializeField] private Vector3 startRotation;
    [SerializeField] private Vector3 scale;
    [SerializeField] private Vector3 spawnOffset;
    private Vector3 currentSpawnPosition;

    [Header("Random horizontal translation")]
    [SerializeField] private float randomXTranslation;
    [SerializeField, Range(0, 100)] private float translationChance;

    [Header("Random rotation")]
    [SerializeField] private float randomRotation;
    [SerializeField, Range(0, 100)] private float rotationChance;

    [Space()]
    [SerializeField] private int platformCount;
    [SerializeField] private int prespawnCount;
    private int obstacleSpawned;

    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private void OnEnable()
    {
        platformProvider.OnDespawn += PlatformProvider_OnDespawn;
    }

    private void OnDisable()
    {
        platformProvider.OnDespawn -= PlatformProvider_OnDespawn;
    }

    private void Start()
    {
        platformProvider.Init(prespawnCount);

        currentSpawnPosition = startPostion;

        GeneratePath();
    }

    public void InitGameMode(GameModeConfig config)
    {
        startPostion = config.PlatformSpawner.startPosition;
        startRotation = config.PlatformSpawner.startRotation;
        scale = config.PlatformSpawner.scale;
        spawnOffset = config.PlatformSpawner.spawnOffset;

        randomXTranslation = config.PlatformSpawner.randomXTranslation;
        translationChance = config.PlatformSpawner.translationChance;

        randomRotation = config.PlatformSpawner.randomRotation;
        rotationChance = config.PlatformSpawner.rotationChance;
    }

    private void GeneratePath()
    {
        var hoverCraft = GameObject.FindAnyObjectByType<HoverCraftMono>();
        if (hoverCraft)
        {
            var hoverCraftRb = hoverCraft.GetComponent<Rigidbody>();

            var pos = currentSpawnPosition;
            pos.y = 5;
            hoverCraftRb.position = pos;
        }

        Spawn(currentSpawnPosition, Quaternion.identity);

        for (int i = 0; i < platformCount; i++)
        {
            SpawnWithAutoPosition();
        }

        //var path = pathGenerator.GeneratePath();//waveManager.GeneratePath();



        //for (int i = 0; i < path.Count; i++)
        //{
        //    var nextPoint = (i < path.Count - 1) ? path[i + 1] : path[0];

        //    var direction = nextPoint - path[i];
        //    var rotation = Quaternion.LookRotation(direction);
        //    Spawn(path[i], rotation);
        //}
    }

    private void PlatformProvider_OnDespawn()
    {
        SpawnWithAutoPosition();
    }

    private void Spawn(Vector3 position, Quaternion rotation)
    {
        currentSpawnPosition += spawnOffset;

        var platform = platformProvider.Get();
        platform.transform.SetPositionAndRotation(position, rotation);
        platform.SetScale(scale);
        obstacleSpawned++;

        if (obstacleSpawner)
        {
            obstacleSpawner.TrySpawn(platform, obstacleSpawned);
        }
    }

    private void SpawnWithAutoPosition()
    {
        var position = GetPlatformPosition();
        var rotation = GetPlatformRotation();

        Spawn(position, rotation);
    }

    private Vector3 GetPlatformPosition()
    {
        var xPosition = currentSpawnPosition.x;
        var translationChanceRnd = Random.Range(0.1f, 100f);

        if (translationChanceRnd <= translationChance)
        {
            var translationDirection = Mathf.Sign(Random.Range(-1, 1));
            xPosition = MoveHorizontal(xPosition, translationDirection);
        }

        currentSpawnPosition.x = xPosition;

        return currentSpawnPosition;
    }

    private bool IsInBounds(float xPos)
    {
        return Mathf.Abs(xPos) <= randomXTranslation;
    }

    private float MoveHorizontal(float xPosition, float direction)
    {
        var nextPosition = xPosition + randomXTranslation * direction;

        if (IsInBounds(nextPosition))
        {
            xPosition = nextPosition;
        }
        else
        {
            xPosition += randomXTranslation * -direction;
        }

        return xPosition;
    }

    private Quaternion GetPlatformRotation()
    {
        var rotation = Quaternion.Euler(startRotation);
        var rotationChanceRnd = Random.Range(0.1f, 100f);

        if (rotationChanceRnd <= rotationChance)
        {
            var rotationDirection = Mathf.Sign(Random.Range(-1, 1));
            rotation *= Quaternion.Euler(0, 0, randomRotation * rotationDirection);
        }

        return rotation;
    }
}

[System.Serializable]
public class PlatformProvider
{
    [SerializeField] private PlatformHolder[] platformPrefabs;
    private ObjectPool<PlatformHolder> platformPool;

    public PlatformHolder LastPlatform { get; private set; }

    public event System.Action OnDespawn = delegate { };

    public void Init(int prespawnCount)
    {
        platformPool = new ObjectPool<PlatformHolder>(
            PoolOnCreate,
            PoolOnGet,
            null, null, false, 200, 500);

        for (int i = 0; i < prespawnCount; i++)
        {
            var platform = Get();
            platform.gameObject.SetActive(false);
            //var platformToSpawn = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

            //var platform = UnityEngine.Object.Instantiate(platformToSpawn);
            //platform.gameObject.SetActive(false);
        }
        LastPlatform = null;
    }

    public PlatformHolder Get()
    {
        return platformPool.Get();
    }

    private PlatformHolder PoolOnCreate()
    {
        var platformToSpawn = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

        var platform = UnityEngine.Object.Instantiate(platformToSpawn);

        platform.InitFirstSpawn(this);

        return platform;
    }

    private void PoolOnGet(PlatformHolder platformHolder)
    {
        platformHolder.InitForReuse(LastPlatform);
        LastPlatform = platformHolder;
    }

    public void Despawn(PlatformHolder platformHolder)
    {
        platformPool.Release(platformHolder);
        //OnDespawn();
    }
}