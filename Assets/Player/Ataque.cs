using System.Collections.Generic;
using UnityEngine;

// Fica no objeto da hitbox do ataque (filho do player).
// A animação liga a hitbox; este script causa dano em cada inimigo que ela tocar.
public class AttackHitboxDamage : MonoBehaviour
{
    [Header("Dano")]
    [SerializeField] private float lightDamage = 15f;
    [SerializeField] private float heavyDamage = 30f;

    [Header("Detecção")]
    // Animator do player (o do objeto "Sprite"). Se ficar vazio, é procurado sozinho.
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string heavyStateName = "Heavy";
    // Evita dano duplo no mesmo inimigo (vários colliders ou eventos seguidos)
    [SerializeField] private float hitInterval = 0.2f;

    private readonly Dictionary<EnemyHealth, float> lastHitTime = new Dictionary<EnemyHealth, float>();

    private void Awake()
    {
        if (playerAnimator == null)
        {
            playerAnimator = transform.root.GetComponentInChildren<Animator>();
        }
    }

    // Funciona com a hitbox como Trigger...
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    // ...e também se ela não for Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHit(collision.collider);
    }

    private void TryHit(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null) return;

        if (lastHitTime.TryGetValue(enemy, out float lastTime) && Time.time - lastTime < hitInterval) return;
        lastHitTime[enemy] = Time.time;

        enemy.TakeDamage(GetCurrentDamage());
    }

    // Golpe pesado causa mais dano; qualquer outro ataque usa o dano leve
    private float GetCurrentDamage()
    {
        bool isHeavy = playerAnimator != null &&
                       playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(heavyStateName);

        return isHeavy ? heavyDamage : lightDamage;
    }
}
