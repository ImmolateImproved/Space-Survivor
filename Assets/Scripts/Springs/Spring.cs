using UnityEngine;

[System.Serializable]
public struct Spring
{
    public float stiffness;
    public float damper;

    public float length;

    public readonly Vector3 HookesLaw(Vector3 displacement, Vector3 velocity)
    {
        var force = (stiffness * displacement) + (damper * velocity);

        return -force;
    }

    public readonly float Hover(float hitDistance, float velocity)
    {
        var displacement = length - hitDistance;

        var force = (stiffness * displacement) - (damper * velocity);

        return Mathf.Max(0, force);
    }
}