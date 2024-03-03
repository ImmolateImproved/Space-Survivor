using DG.Tweening;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static int configIndex;
    [SerializeField] private bool useRandomConfig;
    [SerializeField] private GameModeConfig gameModeConfig;
    [SerializeField] private GameModeConfig[] gameModeConfigs;

    [SerializeField] private GameModeInit[] gameModeInits;

    //[SerializeField] private HoverCraft hoverCraft;
    //[SerializeField] private PlatformSpawner platformSpawner;
    //[SerializeField] private ObstacleSpawnerConfig obstacleSpawner;

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);

        if (useRandomConfig)
        {
            gameModeConfig = gameModeConfigs[configIndex];
            configIndex = (configIndex + 1) % gameModeConfigs.Length;
        }

        foreach (var item in gameModeInits)
        {
            item.InitGameMode(gameModeConfig);
        }

        //hoverCraft.InitGameMode(gameModeConfig);
        //platformSpawner.InitGameMode(gameModeConfig);
        //obstacleSpawner.InitGameMode(gameModeConfig);
    }
}

public abstract class GameModeInit : MonoBehaviour
{
    public abstract void InitGameMode(GameModeConfig config);
}