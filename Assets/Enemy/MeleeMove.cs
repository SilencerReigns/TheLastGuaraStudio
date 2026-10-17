using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeleeMove : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f;
    [SerializeField] private float damageAmount = 10f; // Quantidade de dano configurável no Inspector

    public bool isStunned = false;
    private bool isRepelled = false;

    void Update()
    {
        if (!isStunned && !isRepelled)
        {
            Move();
        }
    }

    void Move()
    {
        if (playerPosition == null) return;

        // Movimento normal em direção ao jogador em 2D
        var movement = playerPosition.position - transform.position;
        movement = movement.normalized * Time.deltaTime * speed;
        transform.position += movement;
    }

    // Detecta o contato físico com o Player em 2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Procura o script HealthPlayer no objeto do jogador com o qual colidiu
            //HealthPlayer playerHealth = collision.gameObject.GetComponent<HealthPlayer>();

            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(damageAmount); // Aplica o dano ao jogador
            //}

            GetRepelled();
        }
    }

    /* 
    // Se estiver a usar o Collider com "Is Trigger" ativado no Inimigo ou no Player:
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthPlayer playerHealth = other.GetComponent<HealthPlayer>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            GetRepelled();
        }
    }
    */

    // Repell System
    public void GetRepelled()
    {
        if (playerPosition == null || isRepelled) return;

        isRepelled = true;

        // Calcula a direção oposta ao jogador no plano 2D
        Vector2 directionAway = (transform.position - playerPosition.position).normalized;
        Vector3 endPoint = transform.position + (Vector3)(directionAway * repelDistance);

        // O DOTween faz a animação e só reativa o movimento normal ao terminar
        transform.DOMove(endPoint, 0.5f).OnComplete(() =>
        {
            isRepelled = false;
        });
    }

    // Stun System
    public void GetStunned()
    {
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }
}