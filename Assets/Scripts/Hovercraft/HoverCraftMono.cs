using UnityEngine;

public class HoverCraftMono : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; private set; }

    [Header("Drive settings")]
    [SerializeField] private float driveForce = 17f;
    [SerializeField] private float enginePower = 100f;
    [SerializeField, Range(0, 1f)] private float slowingVelFactor = 0.99f;

    [Header("Hover settings")]
    [SerializeField] private float maxGroundDistance = 5f;
    [SerializeField] private float hoverForce = 300f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Spring spring;

    [Header("Physics settings")]
    [SerializeField] private float maxVelocity = 100f;
    [SerializeField] private float hoverGravity = 20f;
    [SerializeField] private float fallGravity = 80f;
    [SerializeField] private float jumpForce;

    [Header("View settings")]
    [SerializeField] private Transform shipView;
    [SerializeField] private float angleOfRoll = 30f;

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

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }

    public void InitGameMode(GameModeConfig config)
    {
        driveForce = config.Hovercraft.driveForce;
        enginePower = config.Hovercraft.enginePower;
        maxVelocity = config.Hovercraft.maxVelocity;
        maxGroundDistance = config.Hovercraft.maxGroundDistance;
        spring = config.Hovercraft.spring;
        hoverGravity = config.Hovercraft.hoverGravity;
        fallGravity = config.Hovercraft.fallGravity;
    }

    private void Init()
    {
        rb = GetComponent<Rigidbody>();
        fuelTank = GetComponent<FuelTank>();
        drag = driveForce / enginePower;
        defaultGroundDist = maxGroundDistance;
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
                //CurrentPlatform.DespawnPrevPlatforms();
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
        if (Speed > enginePower)
        {
            Speed = enginePower;
        }
    }

    public float GetSpeedPercentage()
    {
        return rb.velocity.magnitude / enginePower;
    }
}

//public class HoverCraft : ScriptableObject
//{
//    [field: SerializeField] public float Speed { get; private set; }

//    [Header("Drive settings")]
//    [SerializeField] private float driveForce = 17f;
//    [SerializeField] private float enginePower = 100f;
//    [SerializeField, Range(0, 1f)] private float slowingVelFactor = 0.99f;

//    [Header("Hover settings")]
//    [SerializeField] private float maxGroundDistance = 5f;
//    [SerializeField] private float hoverForce = 300f;
//    [SerializeField] private LayerMask groundLayer;
//    [SerializeField] private Spring spring;

//    [Header("Physics settings")]
//    [SerializeField] private float maxVelocity = 100f;
//    [SerializeField] private float hoverGravity = 20f;
//    [SerializeField] private float fallGravity = 80f;
//    [SerializeField] private float jumpForce;

//    [Header("View settings")]
//    [SerializeField] private Transform shipView;
//    [SerializeField] private float angleOfRoll = 30f;

//    private Transform transform;
//    private Rigidbody rb;
//    private FuelTank fuelTank;

//    private float drag;
//    private bool isOnGround;

//    private float rudder;
//    private float thruster;
//    private bool jump;

//    private float defaultGroundDist;

//    public PlatformHolder CurrentPlatform { get; private set; }

//    public void Init(Transform transform)
//    {
//        drag = driveForce / enginePower;
//        defaultGroundDist = maxGroundDistance;
//    }

//    public void Update(float rudder, float thruster, bool jump)
//    {
//        this.rudder = rudder;
//        this.thruster = thruster;
//        this.jump = jump;

//        thruster = fuelTank.FuelAmount <= 0 ? 0 : thruster;

//        if (thruster != 0)
//        {
//            fuelTank.BurnFuel();
//        }
//    }

//    public void FixedUpdate()
//    {
//        Speed = Vector3.Dot(rb.velocity, transform.forward);

//        if (rb.velocity.y < 0)
//        {
//            maxGroundDistance = defaultGroundDist;
//        }

//        Hover();
//        Propulsion();
//        Jump();
//        View();

//        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
//    }

//    public void ApplySpeedBoost(Vector3 force)
//    {
//        rb.AddForce(force, ForceMode.VelocityChange);
//    }

//    public void MultiplyVelocity(float factor)
//    {
//        rb.velocity *= factor;
//    }

//    private void Hover()
//    {
//        var (distanceToGround, groundNormal) = Raycast();

//        if (isOnGround)
//        {
//            groundNormal = groundNormal.normalized;

//            var forceScaler = spring.Hover(distanceToGround, rb.velocity.y);
//            var force = forceScaler * hoverForce * groundNormal;
//            var gravity = distanceToGround * hoverGravity * -groundNormal;

//            rb.AddForce(force, ForceMode.Acceleration);
//            rb.AddForce(gravity, ForceMode.Acceleration);
//        }
//        else
//        {
//            groundNormal = Vector3.up;
//            var gravity = -groundNormal * fallGravity;
//            rb.AddForce(gravity, ForceMode.Acceleration);
//        }

//        var projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
//        var rotation = Quaternion.LookRotation(projection, groundNormal);
//        rb.MoveRotation(Quaternion.Lerp(rb.rotation, rotation, 10 * Time.deltaTime));
//    }

//    private void Propulsion()
//    {
//        var rotationTorque = rudder - rb.angularVelocity.y;
//        rb.AddRelativeTorque(0, rotationTorque, 0, ForceMode.VelocityChange);

//        var sidewaysSpeed = Vector3.Dot(rb.velocity, transform.right);
//        var sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);
//        rb.AddForce(sideFriction, ForceMode.Acceleration);

//        if (thruster <= 0)
//        {
//            rb.velocity *= slowingVelFactor;
//        }

//        //if (!isOnGround) return;

//        var propulsion = driveForce * thruster - drag * Speed;// Mathf.Clamp(Speed, 0, terminalVelocity);
//        rb.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
//    }

//    private void Jump()
//    {
//        if (jump)
//        {
//            var direction = transform.forward + transform.up;
//            direction.Normalize();
//            maxGroundDistance = 0;
//            ApplySpeedBoost(direction * jumpForce);
//        }
//        jump = false;
//    }

//    private (float distanceToGround, Vector3 groundNormal) Raycast()
//    {
//        var ray = new Ray(transform.position, -transform.up);
//        var isHit = Physics.Raycast(ray, out var hit, maxGroundDistance, groundLayer);

//        if (isHit)
//        {
//            var platform = hit.transform.parent.GetComponent<PlatformHolder>();
//            if (CurrentPlatform != platform)
//            {
//                CurrentPlatform = platform;
//                //CurrentPlatform.DespawnPrevPlatforms();
//            }
//        }

//        isOnGround = isHit;

//        return (hit.distance, hit.normal);
//    }

//    private void View()
//    {
//        var angle = angleOfRoll * -rudder;

//        var bodyRotation = transform.rotation * Quaternion.Euler(0, 0, angle);
//        shipView.rotation = Quaternion.Lerp(shipView.rotation, bodyRotation, Time.deltaTime * 10);
//    }

//    public void ResetSpeedToTerminal()
//    {
//        if (Speed > enginePower)
//        {
//            Speed = enginePower;
//        }
//    }

//    public float GetSpeedPercentage()
//    {
//        return rb.velocity.magnitude / enginePower;
//    }
//}