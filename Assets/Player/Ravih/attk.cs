using UnityEngine;

public class PlayerAttack2D : MonoBehaviour
{
    [Header("Configurações do Ataque")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackDamage = 20f; // Dano da espadada
    [SerializeField] private float attackRate = 2f;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) // Botão esquerdo do rato
            {
                AtacarComEspada();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void AtacarComEspada()
    {
        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }

        // Procura todos os colisores na área do ataque
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                // 1. Aplica o repulso ao inimigo
                MeleeMove inimigoMove = hit.GetComponent<MeleeMove>();
                if (inimigoMove != null)
                {
                    inimigoMove.GetRepelled();
                }

                // 2. Aplica o dano no script EnemyHealth
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}