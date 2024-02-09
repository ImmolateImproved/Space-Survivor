using UnityEngine;

[System.Serializable]
public struct ColorFactionViewMap
{
    public ColorFaction colorFaction;
    public Color color;
}

public class ColorFactionHandler : MonoBehaviour
{
    private HoverCraft hoverCraft;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ColorFaction hoverCraftColor;
    [SerializeField] private float slowFactor;
    [SerializeField] private float speedBoost;

    [SerializeField] private ColorFactionViewMap[] colorFactionViewMap;

    private void Awake()
    {
        hoverCraft = GetComponent<HoverCraft>();
        ChangeMesh(hoverCraftColor);
    }

    private void Update()
    {
        var currentPlatform = hoverCraft.CurrentPlatform;

        if (!currentPlatform) return;

        if (currentPlatform.PlatformColor != hoverCraftColor)
        {
            hoverCraftColor = currentPlatform.PlatformColor;
            ChangeMesh(hoverCraftColor);
        }
    }

    public void HandleCollision(ColorFaction obstacleColor)
    {
        if (hoverCraftColor == obstacleColor)
        {
            hoverCraft.ApplySpeedBoost(transform.forward * speedBoost);
        }
        else
        {
            LevelLoader.RestartLevel();
            // hoverCraft.MultiplyVelocity(slowFactor);
        }
    }

    private void ChangeMesh(ColorFaction colorFaction)
    {
        foreach (var item in colorFactionViewMap)
        {
            if (item.colorFaction == colorFaction)
            {
                meshRenderer.materials[1].color = item.color;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var obstacle = other.GetComponent<Obstacle>();

        if (obstacle)
        {
            HandleCollision(obstacle.ObstacleColor);
        }
    }
}