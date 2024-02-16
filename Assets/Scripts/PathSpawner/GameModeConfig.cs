using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameModeConfig")]
public class GameModeConfig : ScriptableObject
{
    [System.Serializable]
    public struct PlatformSpawnerConfig
    {
        public Vector3 startPosition;
        public Vector3 startRotation;
        public Vector3 scale;
        public Vector3 spawnOffset;
        public float randomXTranslation;
        [Range(0f, 100f)] public float translationChance;

        public float randomRotation;
        [Range(0f, 100f)] public float rotationChance;
    }

    [System.Serializable]
    public struct ObstacleSpawnerConfig
    {
        public Vector3 startPostion;
        public float zOffset;
        public float obstacleWidth;
        public int obstacleCount;
        public float obstacleMinSpeed;
        public float obstacleMaxSpeed;
    }

    [System.Serializable]
    public struct HovercraftConfig
    {
        public float driveForce;
        public float enginePower;
        public float maxGroundDistance;
        public float maxVelocity;
        public float hoverGravity;
        public float fallGravity;
        public Spring spring;
    }

    [field: SerializeField] public PlatformSpawnerConfig PlatformSpawner { get; private set; }

    [field: SerializeField] public ObstacleSpawnerConfig ObstacleSpawner { get; private set; }

    [field: SerializeField] public HovercraftConfig Hovercraft { get; private set; }
}