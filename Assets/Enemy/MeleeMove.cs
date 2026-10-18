using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeleeMove : MonoBehaviour
{
    // O ponto de referência do jogador é mantido privado e fixo via script
    private Transform playerPosition;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f;

    public bool isStunned = false;
    private bool isRepelled = false;

    void Start()
    {
        // Busca o GameObject pelo caminho fixo na hierarquia da cena
        GameObject targetObj = GameObject.Find("PlayerRF/Body/Hitbox");

        if (targetObj != null)
        {
            playerPosition = targetObj.transform;
        }
        else
        {
            Debug.LogError("RangeMove: O caminho 'PlayerRF/Body/Hitbox' não foi encontrado na cena!");
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

        // Persegue a posição X e Y do jogador (mantendo apenas o Z do próprio objeto)
        Vector3 targetPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z);

        // Move suavemente nos eixos X e Y
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    // Detecta o contato físico com o Player em 2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetRepelled();
        }
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