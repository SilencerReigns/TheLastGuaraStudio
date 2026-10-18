using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " tomou " + damage + " de dano. Vida restante: " + currentHealth);
        if (currentHealth <= 0f)
        {
            Die();
    }   }

    private void Die()
    {
        Debug.Log(gameObject.name + " foi derrotado!");

        Destroy(gameObject);
}   }