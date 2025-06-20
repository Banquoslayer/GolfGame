using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Material ballMaterial;

    //Right now it's one club for testing, but in the future this can be a list of clubs that player can switch between
    //Club data is meant to have info on the club being used, like club_head_speed, launch_angle, etc.
    [SerializeField] ClubData selectedClub;

    [Header("Physics Properties")]
    [SerializeField] float radius = 0.021335f;          
    [SerializeField] float dragCoefficient = 0.2f;     
    [SerializeField] float airDensity = 1.225f; 
    [SerializeField] float liftCoefficient = 0.2f;

    [Header("Spin Amount")]
    [SerializeField] float frontSpinRPM;
    [SerializeField] float sideSpinRPM;
    [SerializeField]  float spinDampingFactor = 0.90f;
    [SerializeField] float spinScale = 0.3f;

    bool launchButtonPressed = false;
    bool readytoLaunch = true;

    // Hit point is the point where the ball lands, used for calculating distance traveled
    private Vector3 hitPoint;

    private TrailRenderer trailRenderer;

    public bool Launching { get; private set; }
    private bool hasLanded;

    public bool GetHasLanded()
    {
        return hasLanded;
    }

    private void SetHasLanded(bool value)
    {
        hasLanded = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Startup();
    }

    private void Update()
    {
        CheckForInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LaunchIfPressed();

        if (Launching)
        {
            ApplyDrag();
            ApplySpin();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckForLanding(collision);
    }

    public bool IsReadyToLaunch()
    {
        return readytoLaunch;
    }

    void Startup()
    {
        rb = GetComponent<Rigidbody>();
        ballMaterial.color = Color.white;
        radius = GetComponent<SphereCollider>().radius;
        trailRenderer = GetComponent<TrailRenderer>();

        // Scale spin rate to a manegable level
        frontSpinRPM *= spinScale;
        sideSpinRPM *= spinScale;
    }

    void CheckForInput()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            launchButtonPressed = true;
        }
    }

    void ApplySpin()
    {
        Vector3 velocity = rb.linearVelocity;
        if (velocity.magnitude < 0.1f) return;

        float speed = velocity.magnitude;
        float area = Mathf.PI * radius * radius;
        float rpmToRad = 2 * Mathf.PI / 60f;

        // 1. Get camera and calculate direction from camera to ball (flattened)
        Transform cam = Camera.main.transform;
        Vector3 toBall = (transform.position - cam.position).normalized;
        Vector3 flatDirection = Vector3.ProjectOnPlane(toBall, Vector3.up).normalized;

        // 2. Define spin axes relative to camera view
        Vector3 sideSpinAxis = Vector3.up;  // vertical axis for sidespin (left/right curve)
        Vector3 backSpinAxis = Vector3.Cross(flatDirection, sideSpinAxis).normalized; // right axis for backspin (lift)

        // 3. Compose spin vector in world space from RPM values
        Vector3 spinAxis = sideSpinAxis * (sideSpinRPM * rpmToRad) + backSpinAxis * (frontSpinRPM * rpmToRad);

        // 4. Lift is perpendicular to both spin axis and velocity (Magnus effect)
        Vector3 liftDirection = Vector3.Cross(velocity.normalized, spinAxis.normalized).normalized;

        float spinSpeed = spinAxis.magnitude;
        if (spinSpeed < 0.001f) return;  // avoid division by zero

        float liftForceMagnitude = 0.5f * airDensity * liftCoefficient * area * speed * speed;
        Vector3 liftForce = liftDirection * liftForceMagnitude;

        rb.AddForce(liftForce);

        // Dampen spin values in RPM each FixedUpdate
        frontSpinRPM = Mathf.MoveTowards(frontSpinRPM, 0f, spinDampingFactor * Time.fixedDeltaTime);
        sideSpinRPM = Mathf.MoveTowards(sideSpinRPM, 0f, spinDampingFactor * Time.fixedDeltaTime);

        // ✅ Debug rays
        Debug.DrawRay(transform.position, velocity.normalized * 2f, Color.white);        // Velocity
        Debug.DrawRay(transform.position, spinAxis * 2f, Color.magenta);                // Spin Axis
        Debug.DrawRay(transform.position, liftDirection * 2f, Color.green);             // Lift direction
    }

    void ApplyDrag()
    {
        if (rb == null) return;

        Vector3 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed < 0.01f) return; // Avoid tiny jittery forces

        // Calculate cross-sectional area: ?r�
        float area = Mathf.PI * radius * radius;

        // Drag force magnitude: � * ? * v� * Cd * A
        float dragForceMag = 0.5f * airDensity * dragCoefficient * area * velocity.sqrMagnitude;

        // Apply force opposite to velocity
        Vector3 dragForce = -dragForceMag * velocity.normalized;

        rb.AddForce(dragForce);
    }

    void CheckForLanding(Collision collision)
    {
        if (!GetHasLanded())
            Land(collision);
    }

    void Land(Collision collision)
    {
        // Get contact point of collision
        hitPoint = collision.contacts[0].point;

        //trailRenderer.startWidth = 0.0f; // Stop trail rendering

        // Calculate distance from launch point to hit point
        float distance = Vector3.Distance(new Vector3(0, 0.5f, 0), this.GetComponent<Transform>().position) * 1.09361f;
        Debug.Log($"Ball traveled {distance} yards before hitting {collision.gameObject.name}");

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        SetHasLanded(true);
        Launching = false;
    }

    void LaunchIfPressed()
    {
        if (launchButtonPressed)
        {
            LaunchBall(selectedClub.launchAngle, selectedClub.clubHeadSpeed, selectedClub.smashFactor);
            launchButtonPressed = false;
        }
    }

    void LaunchBall(float launchAngle, float clubHeadSpeed, float smashFactor)
    {
        if (Launching) return;

        readytoLaunch = false;

        ballMaterial.color = Color.red;

        trailRenderer.startWidth = 0.1f;

        rb.useGravity = true;

        Vector3 launchVector = CalculateLaunchAngle(launchAngle);

        Vector3 velocity = launchVector.normalized * (clubHeadSpeed * smashFactor) * 0.44704f;
        rb.AddForce(velocity * rb.mass, ForceMode.Impulse);
        Launching = true;
    }

    //TODO: Add a way to set the launch angle and club head speed dynamically, maybe through UI or input, or accuracy minigame
    Vector3 CalculateLaunchAngle(float launchAngle)
    {
        //Convert degrees to launch angle
        Transform cam = Camera.main.transform;
        Transform ball = this.GetComponent<Transform>();

        Vector3 toBall = (ball.position - cam.position).normalized;

        Vector3 flatDirection = Vector3.ProjectOnPlane(toBall, Vector3.up).normalized;

        Vector3 sideAxis = Vector3.Cross(flatDirection, Vector3.up);
        Quaternion pitchUp = Quaternion.AngleAxis(launchAngle, sideAxis);

        // Calculate the launch vector by applying the pitch rotation to the flat direction
        return pitchUp * flatDirection;
    }
}
