using UnityEngine;

public class PlayerAttack2D : MonoBehaviour
{
    [Header("Configurações do Ataque")]
    [SerializeField] private Collider2D attackCollider; // Collider do ataque com "Is Trigger" marcado
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackRate = 2f;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    void Start()
    {
        // Garante que o collider da espada comece desativado
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

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
       animator.SetTrigger("attk1");

        StartCoroutine(AtivarGatilhoAtaque());
    }

    private System.Collections.IEnumerator AtivarGatilhoAtaque()
    {
        isAttacking = true;

        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }

        // Tempo que o hitbox da espada fica ativo na tela (ajuste conforme a animação)
        yield return new WaitForSeconds(0.2f);

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        isAttacking = false;
    }

    // Deteção contínua ao entrar no gatilho do ataque
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Só processa a colisão se o ataque estiver ativo no momento
        if (!isAttacking) return;

        if (other.CompareTag("Enemy"))
        {
            // 1. Aplica a repulsão ao inimigo
            MeleeMove inimigoMove = other.GetComponent<MeleeMove>();
            if (inimigoMove != null)
            {
                inimigoMove.GetRepelled();
            }

            // 2. Aplica o dano no script EnemyHealth
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }
}