using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeleeMove : MonoBehaviour
{
    // O ponto de referência do jogador é mantido privado e fixo via script
    private Transform playerPosition;
    private HealthPlayer playerHealth;
    private PlayerParry playerParry;
    private EnemyAnimator enemyAnimator;
    private Rigidbody2D rb;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f;   // usado só em GetRepelled() (não é mais chamado ao atacar)

    [Header("Perseguição")]
    [SerializeField] private float detectionRange = 8f;   // só persegue o player dentro dessa distância

    [Header("Dano")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;   // tempo entre um ataque e o próximo

    public bool isStunned = false;
    private bool isRepelled = false;
    private bool isTouchingPlayer = false;
    private float nextAttackTime = 0f;

    void Awake()
    {
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();

        // Com Rigidbody2D (Dynamic) os colliders dos inimigos se respeitam.
        // Sem Rigidbody2D, o movimento volta a ser pelo transform (como era antes).
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Busca o GameObject pelo caminho fixo na hierarquia da cena
        GameObject targetObj = GameObject.Find("PlayerRF/Body/Hitbox");

        if (targetObj != null)
        {
            playerPosition = targetObj.transform;

            // HealthPlayer e PlayerParry podem estar no Hitbox ou em qualquer pai dele (ex.: PlayerRF)
            playerHealth = targetObj.GetComponentInParent<HealthPlayer>();
            playerParry = targetObj.GetComponentInParent<PlayerParry>();

            if (playerHealth == null)
            {
                Debug.LogWarning("MeleeMove: HealthPlayer não encontrado em 'PlayerRF/Body/Hitbox' nem nos pais dele. O dano não será aplicado.");
            }

            if (playerParry == null)
            {
                Debug.LogWarning("MeleeMove: PlayerParry não encontrado em 'PlayerRF/Body/Hitbox' nem nos pais dele. O parry não vai anular o ataque.");
            }
        }
        else
        {
            Debug.LogError("MeleeMove: O caminho 'PlayerRF/Body/Hitbox' não foi encontrado na cena!");
        }
    }

    void Update()
    {
        bool moving = false;

        if (!isStunned && !isRepelled)
        {
            if (isTouchingPlayer)
            {
                // Encostado no player: não anda, só ataca quando o cooldown terminar
                TryAttack();
            }
            else
            {
                moving = Move();
            }
        }

        // Sem movimento: zera a velocidade do Rigidbody, senão ele continua deslizando
        if (!moving && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // andando = Walk, parado/atordoado/empurrado = Idle
        if (enemyAnimator != null)
        {
            enemyAnimator.SetMoving(moving);
        }
    }

    // Devolve true se o inimigo realmente se moveu neste frame
    bool Move()
    {
        if (playerPosition == null) return false;

        // Player morto: para de perseguir
        if (playerHealth != null && playerHealth.IsDead) return false;

        // Fora do alcance: fica parado onde está
        float distance = Vector2.Distance(transform.position, playerPosition.position);
        if (distance > detectionRange) return false;

        // Direção até o player (só X e Y)
        Vector2 toPlayer = (Vector2)playerPosition.position - (Vector2)transform.position;

        // Já está praticamente em cima do alvo: não precisa andar
        if (toPlayer.sqrMagnitude < 0.0025f) return false;

        // Vira o sprite para o lado do player
        if (enemyAnimator != null)
        {
            enemyAnimator.FaceTowards(toPlayer.x);
        }

        if (rb != null)
        {
            // Anda pelo Rigidbody: a física impede de atravessar os outros inimigos
            rb.linearVelocity = toPlayer.normalized * speed;
        }
        else
        {
            // Sem Rigidbody2D: movimento antigo pelo transform
            Vector3 targetPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        return true;
    }

    // Ao desligar o script (ex.: inimigo morrendo), para de deslizar
    void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Ataca o player se o cooldown já terminou
    private void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        if (playerHealth != null && playerHealth.IsDead) return;

        // Vira o sprite para o player
        if (enemyAnimator != null && playerPosition != null)
        {
            enemyAnimator.FaceTowards(playerPosition.position.x - transform.position.x);
        }

        // Começa o cooldown (vale tanto para o ataque quanto para o parry)
        nextAttackTime = Time.time + attackCooldown;

        if (playerParry != null && playerParry.IsParrying)
        {
            // Parry na hora do ataque: anula o ataque e atordoa o inimigo
            Debug.Log(gameObject.name + ": parry anulou o ataque!");
            GetStunned();

            if (enemyAnimator != null) enemyAnimator.PlayHurt();
        }
        else if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, (Vector2)transform.position);

            if (enemyAnimator != null) enemyAnimator.PlayAttack();
        }
    }

    // Só marca se está encostado no player; quem ataca é o Update (por causa do cooldown)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    // Repell System (Repulsão nos eixos X e Y)
    // Não é mais chamado quando o inimigo ataca, mas continua aqui porque outros scripts (ex.: attk.cs) ainda usam
    public void GetRepelled()
    {
        if (playerPosition == null || isRepelled) return;

        isRepelled = true;

        // Calcula a direção oposta ao jogador
        Vector2 direction = (transform.position - playerPosition.position).normalized;

        // Caso estejam exatamente na mesma posição, define uma direção padrão
        if (direction == Vector2.zero)
        {
            direction = Vector2.up;
        }

        // Calcula o ponto final empurrando para longe do jogador
        Vector3 endPoint = transform.position + (Vector3)(direction * repelDistance);

        // O DOTween faz a animação e reativa o movimento normal ao terminar
        // SetLink: se o inimigo for destruído no meio do empurrão, o tween é cancelado junto
        transform.DOMove(endPoint, 0.5f).SetLink(gameObject).OnComplete(() =>
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

    // Mostra o raio de perseguição na Scene quando o inimigo está selecionado
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
