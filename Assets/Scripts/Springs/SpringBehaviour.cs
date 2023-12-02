using UnityEngine;

public class SpringBehaviour : MonoBehaviour
{
    public Spring spring;

    private Vector3 startPos;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Init(rb.position);
    }

    private void Init(Vector3 startPosition)
    {
        startPos = startPosition;
        startPos.y += spring.length;
    }

    private void FixedUpdate()
    {
        AddSpringForce();
    }

    public void AddSpringForce()
    {
        var displacement = rb.position - startPos;

        var force = spring.HookesLaw(displacement, rb.velocity);
        rb.AddForce(force);
    }
}