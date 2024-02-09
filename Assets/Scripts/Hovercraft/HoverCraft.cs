using UnityEngine;

public class HoverCraft : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; private set; }

    [Header("Drive settings")]
    [SerializeField] private float driveForce = 17f;
    [SerializeField, Range(0, 1f)] private float slowingVelFactor = 0.99f;
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
    [SerializeField] private float jumpForce;

    [Space()]
    [SerializeField] private Transform shipView;

    private Rigidbody rb;
    private FuelTank fuelTank;

    private float drag;
    private bool isOnGround;

    private float rudder;
    private float thruster;
    private bool jump;

    private float defaultGroundDist;

    public PlatformHolder CurrentPlatform { get; private set; }

    private void Awake()
    {
        Init();
        defaultGroundDist = maxGroundDistance;
    }

    private void Update()
    {
        rudder = Input.GetAxisRaw("Horizontal");
        thruster = Input.anyKeyDown ? 1 : thruster;
        jump = Input.GetKeyDown(KeyCode.E) || jump;

        thruster = fuelTank.FuelAmount <= 0 ? 0 : thruster;

        if (thruster != 0)
        {
            fuelTank.BurnFuel();
        }
    }

    private void FixedUpdate()
    {
        Speed = Vector3.Dot(rb.velocity, transform.forward);

        if (rb.velocity.y < 0)
        {
            maxGroundDistance = defaultGroundDist;
        }

        isOnGround = Raycast(out var distanceToGround, out var groundNormal);

        Hover(distanceToGround, groundNormal);
        Propulsion();
        View();

        if (jump)
        {
            var direction = transform.forward + transform.up;
            direction.Normalize();
            maxGroundDistance = 0;
            ApplySpeedBoost(direction * jumpForce);
        }
        jump = false;
    }

    private void Init()
    {
        rb = GetComponent<Rigidbody>();
        fuelTank = GetComponent<FuelTank>();
        drag = driveForce / terminalVelocity;
    }

    public void ApplySpeedBoost(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void MultiplyVelocity(float factor)
    {
        rb.velocity *= factor;
    }

    private bool Raycast(out float distanceToGround, out Vector3 groundNormal)
    {
        var ray = new Ray(transform.position, -transform.up);
        var isHit = Physics.Raycast(ray, out var hit, maxGroundDistance, groundLayer);

        distanceToGround = hit.distance;
        groundNormal = hit.normal;

        if (isHit)
        {
            var platform = hit.transform.parent.GetComponent<PlatformHolder>();
            if (CurrentPlatform != platform)
            {
                CurrentPlatform = platform;
                CurrentPlatform.DespawnPrevPlatforms();
            }
        }

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

        //if (!isOnGround) return;

        var propulsion = driveForce * thruster - drag * Speed;// Mathf.Clamp(Speed, 0, terminalVelocity);
        rb.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
    }

    private void View()
    {
        var angle = angleOfRoll * -rudder;

        var bodyRotation = transform.rotation * Quaternion.Euler(0, 0, angle);
        shipView.rotation = Quaternion.Lerp(shipView.rotation, bodyRotation, Time.deltaTime * 10);
    }

    public void ResetSpeedToTerminal()
    {
        if (Speed > terminalVelocity)
        {
            Speed = terminalVelocity;
        }
    }

    public float GetSpeedPercentage()
    {
        return rb.velocity.magnitude / terminalVelocity;
    }
}