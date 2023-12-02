using UnityEngine;

public class TurboPad : MonoBehaviour
{
    public Transform direction;
    public float speed;

    private void OnTriggerEnter(Collider other)
    {
        var hoverCraft = other.GetComponent<HoverCraft>();

        if (hoverCraft)
        {
            hoverCraft.ApplySpeedBoost(direction.forward * speed);
        }
    }
}