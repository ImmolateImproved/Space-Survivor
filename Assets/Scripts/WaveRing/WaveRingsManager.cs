using System.Collections.Generic;
using UnityEngine;

public class WaveRingsManager : MonoBehaviour
{
    [SerializeField] private WaveRing ringSettings;
    [SerializeField] private LineRenderer lrPrefab;
    [SerializeField] private int ringsCount;

    [Header("Path generation settings")]
    [SerializeField] private float additionalRadius = 1;
    [SerializeField] private float additionalRadiusOffset;

    [Header("Base settings")]
    [SerializeField, Min(1)] private int pointsCount;
    [SerializeField] private Vector3 center;
    [SerializeField] private float baseRadius;
    [SerializeField] private float baseRadiusOffset;

    [Header("Speed")]
    [SerializeField] private float waveSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Offset")]
    [SerializeField] private float phaseOffset;
    [SerializeField] private float rotationOffset;

    private float currentRotationOffset;
    private float currentPahaseOffset;

    [SerializeField] private bool lineView = true;
    private LineRenderer[] ringLineViews;

    private void Awake()
    {
        ringSettings.Init(pointsCount);
        ringLineViews = new LineRenderer[ringsCount];

        for (int i = 0; i < ringsCount; i++)
        {
            ringLineViews[i] = Instantiate(lrPrefab, transform);
            ringLineViews[i].positionCount = pointsCount;
        }
    }

    private void Update()
    {
        if (lineView)
        {
            ApplyPointsToLine();
        }

        UpdatePhaseAndRotation();
    }

    public List<Vector3> GeneratePath()
    {
        var result = new List<Vector3>(pointsCount * ringsCount);

        for (int i = 0; i < ringsCount; i++)
        {
            var radius = additionalRadius + (additionalRadiusOffset * i);

            var ring = CalculateRingPoints(i);

            for (int j = 0; j < ring.Length; j++)
            {
                ring[j] *= radius;
            }

            result.AddRange(ring);
        }

        return result;
    }

    private void UpdatePhaseAndRotation()
    {
        currentRotationOffset += rotationSpeed * Time.deltaTime;
        currentPahaseOffset += waveSpeed * Time.deltaTime;
    }

    private void ApplyPointsToLine()
    {
        for (int i = 0; i < ringsCount; i++)
        {
            var points = CalculateRingPoints(i + 1);
            ringLineViews[i].SetPositions(points);
        }
    }

    private Vector3[] CalculateRingPoints(int i)
    {
        var radius = baseRadius + (baseRadiusOffset * i);
        var rotation = currentRotationOffset + (rotationOffset * i);
        var phase = currentPahaseOffset + (phaseOffset * i);
        var result = ringSettings.CalculateRing(center, radius, rotation, phase);

        return result;
    }
}