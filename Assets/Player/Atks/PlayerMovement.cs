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

    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isMovementLocked;

    private bool isDashing;
    private float lastDashTime = -Mathf.Infinity;

    // Variável para controlar a orientação do personagem
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isMovementLocked)
        {
            moveInput = Vector2.zero;
            return;
        }

        animator.SetFloat("MoveSpeed", Mathf.Abs(moveInput.magnitude * moveSpeed));

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        moveInput = new Vector2(horizontal, vertical).normalized;

        // Inverte o lado do personagem com base no Input Horizontal (apenas se não estiver a fazer Dash)
        if (!isDashing)
        {
            if (horizontal > 0f && !isFacingRight)
            {
                Flip();
            }
            else if (horizontal < 0f && isFacingRight)
            {
                Flip();
            }
        }

        if (Input.GetKeyDown(dashKey) &&
            Time.time >= lastDashTime + dashCooldown &&
            !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing || isMovementLocked) return;

        Vector2 newPosition =
            rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        ApplyBounds(ref newPosition);
        rb.MovePosition(newPosition);
    }

    // Método responsável por inverter a escala no eixo X
    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1f;
        transform.localScale = currentScale;
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

    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;

        if (locked)
        {
            moveInput = Vector2.zero;
        }
    }
}