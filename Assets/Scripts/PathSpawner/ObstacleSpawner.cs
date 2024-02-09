using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private Obstacle[] obstaclePrefabs;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float minAmount;
    [SerializeField] private float maxAmount;

    private int prevXPosition;

    public void Spawn(PlatformHolder parent)
    {
        var currentAmount = Random.Range(minAmount, maxAmount);

        for (int i = 0; i < currentAmount; i++)
        {
            var position = GetPosition(i, parent.PlatfomrSize.z);

            if (position.z >= parent.PlatfomrSize.z / 2) break;

            var randomIndex = Random.Range(0, obstaclePrefabs.Length);
            var obstacle = Instantiate(obstaclePrefabs[randomIndex], parent.transform);

            obstacle.Init(position, parent.PlatfomrSize.x);
        }
    }

    private Vector3 GetPosition(int i, float platformLength)
    {
        var position = new Vector3
        {
            x = GetRandomXPosition(),
            y = offset.y,
            z = offset.z * (i + 1) - platformLength / 2
        };

        return position;
    }

    private float GetRandomXPosition()
    {
        var xPos = Random.Range(-1, 2);

        if (xPos == prevXPosition)
        {
            xPos = GetNextRandomPosition(xPos);
        }

        prevXPosition = xPos;

        return xPos * offset.x;
    }

    private int GetNextRandomPosition(int position)
    {
        const int pointsCount = 3;
        var randPos = position + RandomSign();

        randPos = (randPos + 1 + pointsCount) % pointsCount;

        return randPos - 1;
    }

    private int RandomSign()
    {
        return (int)Mathf.Sign(Random.Range(-1, 1));
    }
}

[CreateAssetMenu(menuName = "ScriptableObjects/ObstaclePrefabProvider")]
public class ObstaclePrefabProvider : ScriptableObject
{
    [SerializeField] private Obstacle[] obstaclePrefabs;



}