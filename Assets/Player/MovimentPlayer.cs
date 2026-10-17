
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    [Header("Bounds")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-8f, -4.5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(8f, 4.5f);

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool isDashing;
    private float lastDashTime = -Mathf.Infinity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        moveInput = new Vector2(horizontal, vertical).normalized;

        if (Input.GetKeyDown(dashKey) &&
            Time.time >= lastDashTime + dashCooldown &&
            !isDashing)
        {
            StartCoroutine(Dash());
        }
        Debug.Log("Update executando: " + Input.GetKey(KeyCode.D));
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        Vector2 newPosition =
            rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        ApplyBounds(ref newPosition);
        rb.MovePosition(newPosition);
    }

    private IEnumerator Dash()
    {
        if (moveInput == Vector2.zero)
            yield break;

        isDashing = true;
        lastDashTime = Time.time;

        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + moveInput * dashDistance;

        ApplyBounds(ref targetPosition);

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.fixedDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / dashDuration);

            Vector2 newPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                progress
            );

            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
        isDashing = false;
    }

    private void ApplyBounds(ref Vector2 position)
    {
        if (!clampToBounds) return;

        position.x = Mathf.Clamp(
            position.x,
            minBounds.x,
            maxBounds.x
        );

        position.y = Mathf.Clamp(
            position.y,
            minBounds.y,
            maxBounds.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (!clampToBounds) return;

        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            0f
        );

        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            maxBounds.y - minBounds.y,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
}