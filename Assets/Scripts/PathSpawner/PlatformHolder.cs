using UnityEngine;

public class PlatformHolder : MonoBehaviour
{
    [field: SerializeField] public ColorFaction PlatformColor { get; private set; }
    [SerializeField] private Transform mainPlatform;

    private PlatformProvider platformProvider;
    public PlatformHolder PrevPlatform { get; set; }

    public Vector3 PlatfomrSize => mainPlatform.localScale;

    public void InitForReuse(PlatformProvider platformProvider)
    {
        gameObject.SetActive(true);
        this.platformProvider = platformProvider;
        PrevPlatform = platformProvider.LastPlatform;
    }

    public void Despawn()
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
        platformProvider.OnDespawn(this);

        DespawnPrevPlatforms();
    }

    public void DespawnPrevPlatforms()
    {
        if (PrevPlatform)
        {
            PrevPlatform.Despawn();
        }
    }
}