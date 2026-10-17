using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeleeRange : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f;

    public bool isStunned = false;
    private bool isRepelled = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        GetStunned();
        yield return new WaitForSeconds(5f);
        GetRepelled();
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

        // Ponto alvo mantém o X e Z do próprio objeto, mudando apenas o Y para o do jogador
        Vector3 targetY = new Vector3(transform.position.x, playerPosition.position.y, transform.position.z);

        // Move suavemente até a posição alvo no eixo Y
        transform.position = Vector3.MoveTowards(transform.position, targetY, speed * Time.deltaTime);
    }

    // Sistema de Repulsão (apenas no eixo Y)
    public void GetRepelled()
    {
        if (playerPosition == null || isRepelled) return;

        isRepelled = true;

        // Determina se deve ser repelido para cima ou para baixo com base no Y do jogador
        float directionY = transform.position.y >= playerPosition.position.y ? 1f : -1f;

        Vector3 endPoint = new Vector3(
            transform.position.x,
            transform.position.y + (directionY * repelDistance),
            transform.position.z
        );

        transform.DOMove(endPoint, 0.5f).OnComplete(() =>
        {
            isRepelled = false;
        });
    }

    // Sistema de Stun
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