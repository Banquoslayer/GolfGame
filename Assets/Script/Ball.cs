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
    public float radius;           // Golf ball radius in meters
    public float dragCoefficient = 0.2f;     // Typical for a sphere
    public float airDensity = 1f;         // Air density at sea level (kg/m³), derive from info from the course

    bool launchButtonPressed = false;

    private Vector3 hitPoint;
    private bool hasHit = false;

    private TrailRenderer trailRenderer;

    public bool HasLaunched { get; private set; }
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

        if (HasLaunched)
        {
            ApplyDrag();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckForLanding(collision);
    }

    void Startup()
    {
        rb = GetComponent<Rigidbody>();
        ballMaterial.color = Color.white;
        radius = GetComponent<SphereCollider>().radius;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void CheckForInput()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            launchButtonPressed = true;
        }
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
        float dragForceMag = 0.5f * airDensity * dragCoefficient * area * speed * speed;

        // Apply force opposite to velocity
        Vector3 dragForce = -dragForceMag * velocity.normalized;

        rb.AddForce(dragForce);
    }

    void CheckForLanding(Collision collision)
    {
        if (!hasHit)
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

        hasHit = true;
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
        ballMaterial.color = Color.red;

        trailRenderer.startWidth = 0.1f;

        rb.useGravity = true;

        Vector3 launchVector = CalculateLaunchAngle(launchAngle);

        if (HasLaunched) return;

        Vector3 velocity = launchVector.normalized * (clubHeadSpeed * smashFactor) * 0.44704f;
        rb.AddForce(velocity * rb.mass, ForceMode.Impulse);
        //rb.linearVelocity = velocity;
        HasLaunched = true;
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
