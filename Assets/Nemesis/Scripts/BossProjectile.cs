using UnityEngine;

// Projétil teleguiado do boss. É criado pelo BossBurstAttack, que configura tudo pelo Launch().
// Não precisa de collider nem de Rigidbody: o acerto é checado pela distância até o player.
public class BossProjectile : MonoBehaviour
{
    [Tooltip("Gira o desenho se o sprite original não apontar para a direita (em graus)")]
    [SerializeField] private float spriteRotationOffset = 0f;

    private Transform target;
    private HealthPlayer playerHealth;
    private PlayerParry playerParry;

    private Vector2 direction;
    private float speed;
    private float damage;
    private float turnRate;
    private float hitRadius;
    private float lifetime;
    private float age;

    public void Launch(Vector2 startDirection, float speed, float damage, float turnRate, float hitRadius,
                       float lifetime, Transform target, HealthPlayer playerHealth, PlayerParry playerParry)
    {
        this.direction = startDirection.normalized;
        this.speed = speed;
        this.damage = damage;
        this.turnRate = turnRate;
        this.hitRadius = hitRadius;
        this.lifetime = lifetime;
        this.target = target;
        this.playerHealth = playerHealth;
        this.playerParry = playerParry;

        UpdateRotation();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        age += dt;
        if (age >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Curva em direção ao player, no máximo turnRate graus por segundo
        if (target != null && turnRate > 0f)
        {
            Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;

            if (toTarget.sqrMagnitude > 0.0001f)
            {
                float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnRate * dt);

                float rad = newAngle * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            }
        }

        transform.position += (Vector3)(direction * speed * dt);
        UpdateRotation();

        CheckHit();
    }

    private void CheckHit()
    {
        if (target == null) return;
        if (Vector2.Distance(transform.position, target.position) > hitRadius) return;

        if (playerParry != null && playerParry.IsParrying)
        {
            // Parry bloqueia: o projétil some sem causar dano
            Debug.Log("[Boss] Parry bloqueou um projétil");
        }
        else if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(damage, (Vector2)transform.position);
        }

        Destroy(gameObject);
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteRotationOffset);
    }
}
