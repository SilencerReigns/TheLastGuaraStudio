using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;

    [SerializeField] private float speed = 5f;
    [SerializeField] public float stunTime = 2f;
    [SerializeField] private float repelDistance = 3f; // Distância fixa para repelir

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
        // Só move se não estiver nem atordoado nem a ser repelido
        if (!isStunned && !isRepelled)
        {
            Move();
        }
    }

    void Move()
    {
        if (playerPosition == null) return;

        var movement = playerPosition.position - transform.position;
        movement = movement.normalized * Time.deltaTime * speed;
        transform.position += movement;
    }

    // Sistema de Repulsão
    public void GetRepelled()
    {
        if (playerPosition == null || isRepelled) return;

        isRepelled = true;

        // Calcula a direção oposta ao jogador
        Vector3 directionAwayFromPlayer = (transform.position - playerPosition.position).normalized;
        Vector3 endPoint = transform.position + (directionAwayFromPlayer * repelDistance);

        // Executa a animação e reativa o movimento normal quando terminar
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