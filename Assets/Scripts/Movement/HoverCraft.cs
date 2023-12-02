using TMPro;
using UnityEditor;
using UnityEngine;

public class HoverCraft : MonoBehaviour
{
    [SerializeField] private float speed;

    [Header("Drive settings")]
    [SerializeField] private float driveForce = 17f;
    [SerializeField] private float slowingVelFactor = 0.99f;
    [SerializeField] private float angleOfRoll = 30f;

    [Header("Hover settings")]
    [SerializeField] private float maxGroundDistance = 5f;
    [SerializeField] private float hoverForce = 300f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Spring spring;

    [Header("Physics settings")]
    [SerializeField] private float terminalVelocity = 100f;
    [SerializeField] private float hoverGravity = 20f;
    [SerializeField] private float fallGravity = 80f;

    [Space()]
    [SerializeField] private Transform shipView;

    private Rigidbody rb;

    private float drag;
    private bool isOnGround;

    private float rudder;
    private float thruster;

    public IGroundChecker GroundChecker { get; set; }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        rudder = Input.GetAxisRaw("Horizontal");
        thruster = Input.anyKeyDown ? 1 : thruster;// Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        speed = Vector3.Dot(rb.velocity, transform.forward);

        isOnGround = Raycast(out var distanceToGround, out var groundNormal);//

        Hover(distanceToGround, groundNormal);
        Propulsion();
        View();
    }

    private void Init()
    {
        rb = GetComponent<Rigidbody>();
        drag = driveForce / terminalVelocity;
    }

    public void ApplySpeedBoost(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
    }

    private bool Raycast(out float distanceToGround, out Vector3 groundNormal)
    {
        var ray = new Ray(transform.position, -transform.up);
        var isHit = Physics.Raycast(ray, out var hit, maxGroundDistance, groundLayer);

        distanceToGround = hit.distance;
        groundNormal = hit.normal;

        return isHit;
    }

    private void Hover(float distanceToGround, Vector3 groundNormal)
    {
        if (isOnGround)
        {
            groundNormal = groundNormal.normalized;

            var forceScaler = spring.Hover(distanceToGround, rb.velocity.y);

            var force = forceScaler * hoverForce * groundNormal;
            var gravity = distanceToGround * hoverGravity * -groundNormal;

            rb.AddForce(force, ForceMode.Acceleration);
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        else
        {
            groundNormal = Vector3.up;
            var gravity = -groundNormal * fallGravity;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        var projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        var rotation = Quaternion.LookRotation(projection, groundNormal);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, rotation, 10 * Time.deltaTime));
    }

    private void Propulsion()
    {
        var rotationTorque = rudder - rb.angularVelocity.y;
        rb.AddRelativeTorque(0, rotationTorque, 0, ForceMode.VelocityChange);

        var sidewaysSpeed = Vector3.Dot(rb.velocity, transform.right);
        var sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);
        rb.AddForce(sideFriction, ForceMode.Acceleration);

        if (thruster <= 0)
        {
            rb.velocity *= slowingVelFactor;
        }

        if (!isOnGround) return;

        var propulsion = driveForce * thruster - drag * Mathf.Clamp(speed, 0, terminalVelocity);
        rb.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
    }

    private void View()
    {
        var angle = angleOfRoll * -rudder;

        var bodyRotation = transform.rotation * Quaternion.Euler(0, 0, angle);
        shipView.rotation = Quaternion.Lerp(shipView.rotation, bodyRotation, Time.deltaTime * 10);
    }

    public float GetSpeedPercentage()
    {
        return rb.velocity.magnitude / terminalVelocity;
    }
}