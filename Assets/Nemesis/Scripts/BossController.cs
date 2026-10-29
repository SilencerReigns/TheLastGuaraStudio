using System.Collections;
using UnityEngine;

// Cérebro do boss Nemesis (etapa 1): alcance de ativação, fila de ataques e exclusividade.
// Os ataques ainda são "esqueletos": tocam a animação e escrevem no Console.
// Os efeitos reais entram nas próximas etapas (rajada, círculo, invocação).
[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(BossAnimator))]
public class BossController : MonoBehaviour
{
    [Header("Ativação")]
    [SerializeField] private float detectionRange = 10f;
    [Tooltip("Tempo entre a ativação e o primeiro ataque")]
    [SerializeField] private float firstAttackDelay = 1f;

    [Header("Ataque 1 - Rajada")]
    [Tooltip("Tempo de espera, depois que a rajada termina, até poder repetir")]
    [SerializeField] private float burstCooldown = 4f;

    private Transform playerPosition;
    private HealthPlayer playerHealth;
    private BossHealth bossHealth;
    private BossAnimator bossAnimator;
    private BossBurstAttack burstAttack;

    private bool isActive;
    private bool isBusy;       // true durante qualquer ataque: nenhum outro pode começar
    private bool isDead;
    private bool circlePending;
    private int pendingSummons;
    private float nextBurstTime;

    private void Awake()
    {
        bossHealth = GetComponent<BossHealth>();
        bossAnimator = GetComponent<BossAnimator>();
        burstAttack = GetComponent<BossBurstAttack>();
    }

    private void OnEnable()
    {
        bossHealth.OnCircleThreshold += HandleCircleThreshold;
        bossHealth.OnSummonThreshold += HandleSummonThreshold;
        bossHealth.OnHurt += HandleHurt;
        bossHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        bossHealth.OnCircleThreshold -= HandleCircleThreshold;
        bossHealth.OnSummonThreshold -= HandleSummonThreshold;
        bossHealth.OnHurt -= HandleHurt;
        bossHealth.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        // Mesmo caminho fixo usado pelos inimigos
        GameObject targetObj = GameObject.Find("PlayerRF/Body/Hitbox");

        if (targetObj != null)
        {
            playerPosition = targetObj.transform;
            playerHealth = targetObj.GetComponentInParent<HealthPlayer>();
        }
        else
        {
            Debug.LogError("BossController: O caminho 'PlayerRF/Body/Hitbox' não foi encontrado na cena!");
        }
    }

    private void Update()
    {
        if (isDead || playerPosition == null) return;

        // Antes da primeira entrada no range o boss fica parado, sem atacar e sem barra
        if (!isActive)
        {
            if (IsPlayerInRange()) Activate();
            return;
        }

        if (isBusy) return;
        if (playerHealth != null && playerHealth.IsDead) return;
        if (!IsPlayerInRange()) return;

        // Prioridade: invocação > círculo > rajada. Se algum limiar chegou durante outro ataque, ele espera aqui.
        if (pendingSummons > 0)
        {
            pendingSummons--;
            StartCoroutine(SummonAttack());
        }
        else if (circlePending)
        {
            circlePending = false;
            StartCoroutine(CircleAttack());
        }
        else if (Time.time >= nextBurstTime)
        {
            StartCoroutine(BurstAttack());
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, playerPosition.position) <= detectionRange;
    }

    private void Activate()
    {
        if (isActive) return;

        isActive = true;
        bossHealth.ShowBar();
        nextBurstTime = Time.time + firstAttackDelay;

        Debug.Log("[Boss] Ativado: barra de vida visível");
    }

    // ---------- Eventos da vida ----------

    // Se vários limiares do círculo chegarem de uma vez (golpe muito forte), conta como um círculo só
    private void HandleCircleThreshold()
    {
        circlePending = true;
    }

    private void HandleSummonThreshold()
    {
        pendingSummons++;
    }

    private void HandleHurt()
    {
        // Levar dano antes de o player entrar no range também ativa o boss
        Activate();

        bossAnimator.PlayHurt();
    }

    private void HandleDeath()
    {
        isDead = true;
        StopAllCoroutines();
        isBusy = false;

        if (burstAttack != null) burstAttack.ClearBullets();

        float length = bossAnimator.PlayDeath();
        bossHealth.HideBar();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Some depois que a animação de morte terminar
        Destroy(gameObject, length);
    }

    // ---------- Ataques (esqueletos das próximas etapas) ----------

    // Ataque 1: rajada de projéteis teleguiados (BossBurstAttack)
    private IEnumerator BurstAttack()
    {
        isBusy = true;
        Debug.Log("[Boss] Ataque 1: rajada");

        float startTime = Time.time;
        float length = bossAnimator.PlayAttack1();

        if (burstAttack != null)
        {
            // espera o quadro certo da animação e dispara as ondas
            yield return new WaitForSeconds(burstAttack.FireDelay);
            yield return StartCoroutine(burstAttack.FireWaves());
        }
        else
        {
            Debug.LogError("BossController: falta o componente BossBurstAttack no Nemesis.");
        }

        // o boss só fica livre quando a animação também terminar
        float remaining = length - (Time.time - startTime);
        if (remaining > 0f) yield return new WaitForSeconds(remaining);

        nextBurstTime = Time.time + burstCooldown;
        isBusy = false;
    }

    // ETAPA 3: aqui entra o círculo com canalização e morte instantânea
    private IEnumerator CircleAttack()
    {
        isBusy = true;
        Debug.Log("[Boss] Ataque 2: círculo (efeito entra na etapa 3)");

        float length = bossAnimator.PlayAttack2();
        yield return new WaitForSeconds(length);

        isBusy = false;
    }

    // ETAPA 4: aqui entram a invocação e a invulnerabilidade
    private IEnumerator SummonAttack()
    {
        isBusy = true;
        Debug.Log("[Boss] Ataque 3: invocação (efeito entra na etapa 4)");

        float length = bossAnimator.PlayAttack3();
        yield return new WaitForSeconds(length);

        isBusy = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}