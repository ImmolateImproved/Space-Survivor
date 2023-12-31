﻿using System.Collections;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] private Transform platformPrefab;
    [SerializeField] private Vector3 startPostion;
    [SerializeField] private Vector3 spawnOffset;
    private Vector3 currentSpawnPosition;

    [Header("Platform horizontal translation")]
    [SerializeField] private float horizontalTranslation;
    [SerializeField, Range(0, 100)] private float translationChance;

    [Header("Platform rotation")]
    [SerializeField] private float rotationAmount;
    [SerializeField, Range(0, 100)] private float rotationChance;

    [Space()]
    [SerializeField] private int platformCount;

    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private void Start()
    {
        currentSpawnPosition = startPostion;

        SpawnPlatform(false);

        for (int i = 0; i < platformCount; i++)
        {
            SpawnPlatform(true);
        }
    }

    private void SpawnPlatform(bool changePosition)
    {
        var position = changePosition ? GetPlatformPosition() : currentSpawnPosition;
        var rotation = changePosition ? GetPlatformRotation() : Quaternion.identity;

        currentSpawnPosition += spawnOffset;

        var platform = Instantiate(platformPrefab, position, rotation);

        obstacleSpawner.Spawn(platform);
        obstacleSpawner.Spawn(platform);
        obstacleSpawner.Spawn(platform);
        obstacleSpawner.Spawn(platform);
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
        return Mathf.Abs(xPos) <= horizontalTranslation;
    }

    private float MoveHorizontal(float xPosition, float direction)
    {
        var nextPosition = xPosition + horizontalTranslation * direction;

        if (IsInBounds(nextPosition))
        {
            xPosition = nextPosition;
        }
        else
        {
            xPosition += horizontalTranslation * -direction;
        }

        return xPosition;
    }

    private Quaternion GetPlatformRotation()
    {
        var rotation = platformPrefab.rotation;
        var rotationChanceRnd = Random.Range(0.1f, 100f);

        if (rotationChanceRnd <= rotationChance)
        {
            var rotationDirection = Mathf.Sign(Random.Range(-1, 1));
            rotation *= Quaternion.Euler(0, 0, rotationAmount * rotationDirection);
        }

        return rotation;
    }
}