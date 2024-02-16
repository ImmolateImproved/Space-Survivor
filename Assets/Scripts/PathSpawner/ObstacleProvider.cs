using UnityEngine;

[System.Serializable]
public class ColorToMesh
{
    public ColorFaction color;
    public Mesh mesh;
}

[CreateAssetMenu(menuName = "ScriptableObjects/ObstaclePrefabProvider")]
public class ObstacleProvider : ScriptableObject
{
    [SerializeField] private Obstacle[] obstaclePrefabs;
    [SerializeField] private ColorToMesh[] colorToMesh;

    public ColorToMesh GetColorToMesh(ColorFaction color)
    {
        foreach (var item in colorToMesh)
        {
            if (item.color == color)
            {
                return item;
            }    
        }

        return null;
    }

    public Obstacle GetPrefabByColor(ColorFaction color)
    {
        foreach (var item in obstaclePrefabs)
        {
            if (item.ObstacleColor == color)
            {
                return item;
            }
        }

        return null;
    }

    public Obstacle GetRandom()
    {
        var randomIndex = Random.Range(0, obstaclePrefabs.Length);

        return obstaclePrefabs[randomIndex];
    }
}