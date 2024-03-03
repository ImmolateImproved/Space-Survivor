using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WaveRing", fileName = "WaveRing")]
public class WaveRing : ScriptableObject
{
    [SerializeField] private float waveFrequency;
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float phase;
    [SerializeField] private float rotation;
    [SerializeField] private float waveStrengthFactor = 1;
    [SerializeField, Range(0, 360)] private float range = 90;

    private Vector3[] ringPoints;

    public void Init(int pointsCount)
    {
        ringPoints = new Vector3[pointsCount];
    }

    public Vector3[] CalculateRing(Vector3 center, float radius, float rotationOffset, float phaseOffset)
    {
        var increment = 360 / ringPoints.Length;

        for (int i = 0; i < ringPoints.Length; i++)
        {
            var angleDeg = i * increment;

            var currentAngle = (angleDeg + rotation + rotationOffset) * Mathf.Deg2Rad;
            var currentPhase = (angleDeg + phase + phaseOffset) * Mathf.Deg2Rad;

            var currentRadius = radius;

            if (angleDeg < range)
            {
                var percent = angleDeg / range;
                var strength = CustomCos_0_1_0(waveStrengthFactor * percent);
                currentRadius += strength * Mathf.Sin(currentPhase * waveFrequency) * waveAmplitude;
            }

            var x = currentRadius * Mathf.Cos(currentAngle);
            var z = currentRadius * Mathf.Sin(currentAngle);

            var point = new Vector3(x, 0, z);
            point += center;

            ringPoints[i] = point;
        }

        return ringPoints;
    }

    private float CustomCos_0_1_0(float x)
    {
        return 0.5f * (1 + Mathf.Cos(x * 2 * Mathf.PI + Mathf.PI));
    }
}