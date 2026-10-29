using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    private EnemyAnimator enemyAnimator;
    private bool isDead;

    public bool IsDead => isDead;

    void Awake()
    {
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " tomou " + damage + " de dano. Vida restante: " + currentHealth);

        // Se a vida chegar a zero ou menos, o inimigo morre
        if (currentHealth <= 0f)
        {
            Die();
        }
        else if (enemyAnimator != null)
        {
            enemyAnimator.PlayHurt();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " foi derrotado!");

        // Para de se mover e de causar dano
        MeleeMove move = GetComponent<MeleeMove>();
        if (move != null) move.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Toca a animação de morte e só destrói o inimigo quando ela terminar
        float delay = 0f;
        if (enemyAnimator != null)
        {
            delay = enemyAnimator.PlayDeath();
        }

        Destroy(gameObject, delay);
    }

    // Teste sem o ataque do player: botão direito no componente > "Testar dano (20)"
    [ContextMenu("Testar dano (20)")]
    private void TestDamage()
    {
        if (!Application.isPlaying) return;
        TakeDamage(20f);
    }
}
