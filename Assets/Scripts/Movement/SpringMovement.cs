using UnityEditor;
using UnityEngine;

public class SpringMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRayDistance;

    [SerializeField] private Spring movementSpring;
    [SerializeField] private Spring rotationSpring;

    [Tooltip("The local rotation about which oscillations are centered.")]
    [SerializeField] private Vector3 localEquilibriumRotation = Vector3.zero;

    [Tooltip("The axes over which the oscillator applies torque. Within range [0, 1].")]
    [SerializeField] private Vector3 torqueScale = Vector3.one;

    private float springYPosition;

    private float angularDisplacementMagnitude;
    private Vector3 rotAxis;

    private Vector3 moveInput;
    private bool jumpInput;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(h, 0, v).normalized * moveSpeed;

        jumpInput = Input.GetKeyDown(KeyCode.Space) || jumpInput;
    }

    private void FixedUpdate()
    {
        //Move();

        if (GroundCheck())
        {
            Jump();
            AddSpringForce();
        }

        AddTorqueForce();
    }

    private bool GroundCheck()
    {
        var res = Physics.Raycast(transform.position, Vector3.down, out var hit, groundCheckRayDistance, groundLayer);

        if (res)
        {
            springYPosition = hit.point.y + movementSpring.length;
        }

        return res;
    }

    private void Move()
    {
        moveInput.y = rb.velocity.y;
        rb.velocity = moveInput;
    }

    private void Jump()
    {
        var jump = jumpInput ? jumpForce : 0;
        rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
        jumpInput = false;
    }

    private void AddSpringForce()
    {
        var displacement = Vector3.zero;
        displacement.y = rb.position.y - springYPosition;

        var force = movementSpring.HookesLaw(displacement, rb.velocity);
        rb.AddForceAtPosition(force, rb.position - transform.up);
    }

    private void AddTorqueForce()
    {
        var restoringTorque = CalculateRestoringTorque();

        rb.AddTorque(Vector3.Scale(restoringTorque, torqueScale));
    }

    private Vector3 CalculateRestoringTorque()
    {
        Quaternion deltaRotation = MathUtils.ShortestRotation(transform.localRotation, Quaternion.Euler(localEquilibriumRotation));
        deltaRotation.ToAngleAxis(out angularDisplacementMagnitude, out rotAxis);

        Vector3 angularDisplacement = angularDisplacementMagnitude * Mathf.Deg2Rad * rotAxis.normalized;

        var torque = rotationSpring.HookesLaw(angularDisplacement, rb.angularVelocity);

        return (torque);
    }
}