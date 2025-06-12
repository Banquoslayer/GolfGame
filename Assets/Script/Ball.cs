using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Material ballMaterial;

    public bool HasLaunched { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballMaterial.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.isPressed)
        {
            LaunchBall(new Vector3(1, 1, 0), 15);
        }
    }

    void LaunchBall(Vector3 launchVector, int clubHeadSpeed)
    {
        ballMaterial.color = Color.red;

        rb.useGravity = true;

        if (HasLaunched) return;

        Vector3 velocity = launchVector.normalized * clubHeadSpeed;
        rb.linearVelocity = velocity;
        HasLaunched = true;
    }
}
