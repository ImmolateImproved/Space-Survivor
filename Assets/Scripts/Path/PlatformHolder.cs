using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHolder : MonoBehaviour
{
    [field: SerializeField] public ColorFaction PlatformColor { get; private set; }
    [SerializeField] private Transform mainPlatform;
    private List<Obstacle> obstacles;

    private PlatformProvider platformProvider;
    public PlatformHolder NextPlatform { get; set; }
    public PlatformHolder PrevPlatform { get; set; }

    public Vector3 PlatfomrSize => mainPlatform.localScale;

    public void InitFirstSpawn(PlatformProvider platformProvider)
    {
        this.platformProvider = platformProvider;
        obstacles = new List<Obstacle>(2);
    }

    public void SetScale(Vector3 scale)
    {
        mainPlatform.localScale = scale;
    }

    public void InitForReuse(PlatformHolder lastPlatform)
    {
        gameObject.SetActive(true);
        PrevPlatform = lastPlatform;
        if (lastPlatform)
        {
            lastPlatform.NextPlatform = this;
        }
    }

    public void Despawn()
    {
        if (!gameObject.activeSelf) return;

        obstacles.Clear();

        if (NextPlatform)
        {
            NextPlatform.PrevPlatform = null;
        }

        gameObject.SetActive(false);
        platformProvider.Despawn(this);

        DespawnPrevPlatforms();
    }

    public void DespawnPrevPlatforms()
    {
        if (PrevPlatform)
        {
            PrevPlatform.Despawn();
        }
    }

    public Obstacle GetRandomObstacle()
    {
        var randIndex = Random.Range(0, obstacles.Count);

        return obstacles[randIndex];
    }

    public void AddObstacle(Obstacle obstacle)
    {
        obstacles.Add(obstacle);
    }
}

[System.Serializable]
public class PlatformMovement
{
    public float speed;
    public float moveRange;

    public void Movement(Transform myTransform)
    {
        myTransform.DOMoveX(moveRange, speed).SetEase(Ease.Linear).SetSpeedBased(true).SetLoops(-1, LoopType.Yoyo);
    }

    //public IEnumerator Movement(Vector2 offset, Transform myTransform)
    //{

    //    while (true)
    //    {
    //        var pos = myTransform.localPosition;
    //        pos.x = Mathf.PingPong((offset.x + Time.time) * speed, moveRange) - moveRange / 2;

    //        myTransform.localPosition = pos;

    //        yield return null;
    //    }
    //}
}