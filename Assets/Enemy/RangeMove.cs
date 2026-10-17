using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RangeMove : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f;

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

        // Mantém o X e Z do próprio objeto, alterando apenas o Y para o do jogador
        Vector3 targetY = new Vector3(transform.position.x, playerPosition.position.y, transform.position.z);

        // Move suavemente no eixo Y
        transform.position = Vector3.MoveTowards(transform.position, targetY, speed * Time.deltaTime);
    }

    // Detecta o contato físico com o Player em 2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetRepelled();
        }
    }

    /* 
    // Se o Collider estiver marcado como "Is Trigger", use este método:
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GetRepelled();
        }
    }
    */

    // Repell System (Repulsão apenas no eixo Y)
    public void GetRepelled()
    {
        if (playerPosition == null || isRepelled) return;

        isRepelled = true;

        // Empurra para cima se estiver acima/no mesmo Y do player, ou para baixo se estiver abaixo
        float directionY = transform.position.y >= playerPosition.position.y ? 1f : -1f;

        Vector3 endPoint = new Vector3(
            transform.position.x,
            transform.position.y + (directionY * repelDistance),
            transform.position.z
        );

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