using DG.Tweening;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static int configIndex;
    [SerializeField] private bool useRandomConfig;
    [SerializeField] private GameModeConfig gameModeConfig;
    [SerializeField] private GameModeConfig[] gameModeConfigs;

    [SerializeField] private HoverCraftMono hoverCraft;
    [SerializeField] private PlatformSpawner platformSpawner;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);

        if (useRandomConfig)
        {
            gameModeConfig = gameModeConfigs[configIndex];
            configIndex = (configIndex + 1) % gameModeConfigs.Length;
        }

        hoverCraft.InitGameMode(gameModeConfig);
        platformSpawner.InitGameMode(gameModeConfig);
        obstacleSpawner.InitGameMode(gameModeConfig);
    }
}