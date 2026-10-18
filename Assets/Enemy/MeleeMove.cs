using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeleeMove : MonoBehaviour
{
    // O ponto de referência do jogador é mantido privado e fixo via script
    private Transform playerPosition;
    private HealthPlayer playerHealth;
    private PlayerParry playerParry;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f;

    [Header("Perseguição")]
    [SerializeField] private float detectionRange = 8f;   // só persegue o player dentro dessa distância

    [Header("Dano")]
    [SerializeField] private float damage = 10f;

    public bool isStunned = false;
    private bool isRepelled = false;

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
        if (!isStunned && !isRepelled)
        {
            Move();
        }
    }

    void Move()
    {
        if (playerPosition == null) return;

        // Player morto: para de perseguir
        if (playerHealth != null && playerHealth.IsDead) return;

        // Fora do alcance: fica parado onde está
        float distance = Vector2.Distance(transform.position, playerPosition.position);
        if (distance > detectionRange) return;

        // Persegue a posição X e Y do jogador (mantendo apenas o Z do próprio objeto)
        Vector3 targetPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z);

        // Move suavemente nos eixos X e Y
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    // Detecta o contato físico com o Player em 2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Inimigo atordoado ou já sendo repelido não causa dano
        if (!isStunned && !isRepelled)
        {
            if (playerParry != null && playerParry.IsParrying)
            {
                // Parry na hora do contato: anula o ataque e atordoa o inimigo
                Debug.Log(gameObject.name + ": parry anulou o ataque!");
                GetStunned();
            }
            else if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, (Vector2)transform.position);
            }
        }

        GetRepelled();
    }

    // Repell System (Repulsão nos eixos X e Y)
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
