using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] obstaclePrefabs;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;

    private int prevXPosition;

    private float curZPosition;

    private Transform curPlatform;

    public void Spawn(Transform platform)
    {
        if (!enabled) return;

        if (curPlatform != platform)
        {
            curZPosition = 0;
            curPlatform = platform;
        }

        Spawn(platform.position);
    }

    private void Spawn(Vector3 position)
    {
        position.x = GetRandomXPosition();
        position.y += yOffset;
        position.z += curZPosition;
        curZPosition += zOffset;
        var randomIndex = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[randomIndex], position, Quaternion.identity);
    }

    private float GetRandomXPosition()
    {
        var xPos = Random.Range(-1, 2);

        if (xPos == prevXPosition)
        {
            xPos = GetNextRandomPosition(xPos);
        }

        prevXPosition = xPos;

        return xPos * xOffset;
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