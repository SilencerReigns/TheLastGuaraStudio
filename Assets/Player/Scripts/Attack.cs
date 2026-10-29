using System.Collections.Generic;
using UnityEngine;

// Fica no objeto da hitbox do ataque (filho do player).
// A animação liga a hitbox; este script causa dano em cada inimigo (ou no boss) que ela tocar.
public class AttackHitboxDamage : MonoBehaviour
{
    [Header("Dano")]
    [SerializeField] private float lightDamage = 15f;
    [SerializeField] private float heavyDamage = 30f;

    [Header("Detecção")]
    // Animator do player (o do objeto "Sprite"). Se ficar vazio, é procurado sozinho.
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string heavyStateName = "Heavy";
    // Evita dano duplo no mesmo alvo (vários colliders ou eventos seguidos)
    [SerializeField] private float hitInterval = 0.2f;
    // Escreve no Console tudo que a hitbox toca (desligue quando tudo estiver funcionando)
    [SerializeField] private bool logHits = true;

    private readonly Dictionary<Component, float> lastHitTime = new Dictionary<Component, float>();

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
        BossHealth boss = enemy == null ? other.GetComponentInParent<BossHealth>() : null;

        Component target = enemy != null ? (Component)enemy : boss;

        if (logHits)
        {
            string alvo = enemy != null ? "inimigo (EnemyHealth)" : boss != null ? "BOSS (BossHealth)" : "nada que leve dano";
            Debug.Log("[Hitbox] tocou em '" + other.name + "' -> " + alvo);
        }

        if (target == null) return;

        if (lastHitTime.TryGetValue(target, out float lastTime) && Time.time - lastTime < hitInterval) return;
        lastHitTime[target] = Time.time;

        float damage = GetCurrentDamage();

        if (enemy != null) enemy.TakeDamage(damage);
        else boss.TakeDamage(damage);
    }

    // Golpe pesado causa mais dano; qualquer outro ataque usa o dano leve
    private float GetCurrentDamage()
    {
        bool isHeavy = playerAnimator != null &&
                       playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(heavyStateName);

        return isHeavy ? heavyDamage : lightDamage;
    }
}
