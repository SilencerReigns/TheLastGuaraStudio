using UnityEngine;

public class TestDummy : MonoBehaviour
{
    [Header("Movement")]
    public float amplitude = 1f;      // Distance from center (units)
    public float speed = 0.2f;          // Cycles per second

    [Header("Style")]
    public bool useSineWave = true;   // Sine = smooth, PingPong = linear

    [Header("Physics")]
    public bool useVelocity = true;   // true = drive via velocity (recommended)
    public bool lockOtherAxes = true; // Freeze X/Z and rotation for stability

    private Rigidbody2D rb;
    private float centerY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (lockOtherAxes)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX
                           | RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 0f; // otherwise gravity fights the motion
        }
    }

    void Start()
    {
        centerY = rb.position.y;
    }

    void FixedUpdate()
    {
        float t = Time.fixedTime * speed;

        float targetOffset;
        float targetVelocity;

        if (useSineWave)
        {
            // Position: y = sin(2π·t) * A
            // Velocity: y' = 2π·t' · cos(2π·t) · A
            float omega = speed * Mathf.PI * 2f;
            targetOffset = Mathf.Sin(omega * Time.fixedTime) * amplitude;
            targetVelocity = Mathf.Cos(omega * Time.fixedTime) * amplitude * omega;
        }
        else
        {
            // PingPong derivative: constant speed ±(2 * amplitude * speed)
            targetOffset = Mathf.PingPong(t * 2f, amplitude * 2f) - amplitude;
            float dir = Mathf.Repeat(t * 2f, amplitude * 2f * 2f) < amplitude * 2f ? 1f : -1f;
            targetVelocity = dir * amplitude * 2f * speed;
        }

        if (useVelocity)
        {
            // Preserve any external X/Z velocity (knockback, pushing, etc.)
            Vector3 vel = rb.linearVelocity;
            vel.y = targetVelocity;
            rb.linearVelocity = vel;
        }
        else
        {
            // Position-driven: move via MovePosition (still physics-friendly)
            Vector3 pos = rb.position;
            pos.y = centerY + targetOffset;
            rb.MovePosition(pos);
        }
    }
}
