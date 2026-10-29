using System;
using UnityEngine;
using HeyHeyThere.PixelBarsFree;

// Vida do boss: dano, invulnerabilidade, limiares de vida e barra de vida.
// Quem decide o que fazer em cada limiar é o BossController (escuta os eventos daqui).
public class BossHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 1000f;

    [Header("Limiares (% da vida máxima)")]
    [Tooltip("A cada X% de vida perdida, o boss faz o ataque do círculo")]
    [SerializeField] private float circleEveryPercent = 2f;
    [Tooltip("A cada X% de vida perdida, o boss invoca inimigos e fica inalvejável")]
    [SerializeField] private float summonEveryPercent = 25f;
    [Tooltip("Quando a vida restante chegar a X%, o boss começa a fugir")]
    [SerializeField] private float fleeAtPercent = 10f;

    [Header("Barra de vida (objeto da cena)")]
    [SerializeField] private PixelStatBar healthBar;
    [Tooltip("Opcional: o objeto inteiro da barra (moldura, nome...). Se vazio, usa só o objeto do PixelStatBar.")]
    [SerializeField] private GameObject barRoot;

    [Header("Debug")]
    [SerializeField] private bool logEvents = true;

    public event Action OnCircleThreshold;
    public event Action OnSummonThreshold;
    public event Action OnFleeThreshold;
    public event Action OnHurt;
    public event Action OnDeath;

    private float health;
    private bool isDead;
    private bool isInvulnerable;
    private int circleStepsFired;
    private int summonStepsFired;
    private bool fleeFired;

    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public bool IsInvulnerable => isInvulnerable;

    private void Awake()
    {
        health = maxHealth;

        // a barra só aparece quando o player entra no range pela primeira vez
        GameObject bar = GetBarObject();
        if (bar != null) bar.SetActive(false);
    }

    // Inalvejável (ex.: durante a invocação): TakeDamage devolve false e não tira vida
    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;

        if (logEvents) Debug.Log("[Boss] Invulnerável: " + value);
    }

    // Devolve true se o dano foi aplicado
    public bool TakeDamage(float damage)
    {
        if (isDead || isInvulnerable || damage <= 0f)
        {
            if (logEvents) Debug.Log("[Boss] Dano bloqueado (morto ou invulnerável)");
            return false;
        }

        health = Mathf.Max(health - damage, 0f);

        if (logEvents) Debug.Log("[Boss] Dano recebido: " + damage + " | vida: " + health + "/" + maxHealth);

        if (healthBar != null) healthBar.value = health;

        // na morte não dispara os limiares (evita dezenas de eventos de uma vez)
        if (health > 0f) CheckThresholds();

        if (health <= 0f)
        {
            isDead = true;
            if (logEvents) Debug.Log("[Boss] Morreu");
            OnDeath?.Invoke();
        }
        else
        {
            OnHurt?.Invoke();
        }

        return true;
    }

    public void ShowBar()
    {
        GameObject bar = GetBarObject();
        if (bar == null) return;

        bar.SetActive(true);

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            healthBar.Settle();
        }
    }

    public void HideBar()
    {
        GameObject bar = GetBarObject();
        if (bar != null) bar.SetActive(false);
    }

    private GameObject GetBarObject()
    {
        if (barRoot != null) return barRoot;
        return healthBar != null ? healthBar.gameObject : null;
    }

    private void CheckThresholds()
    {
        // 0.001 de folga evita erro de arredondamento (ex.: 1.9999 em vez de 2)
        float lostPercent = (1f - health / maxHealth) * 100f + 0.001f;
        float remainingPercent = health / maxHealth * 100f;

        if (circleEveryPercent > 0f)
        {
            int steps = Mathf.FloorToInt(lostPercent / circleEveryPercent);

            while (circleStepsFired < steps)
            {
                circleStepsFired++;
                if (logEvents) Debug.Log("[Boss] Limiar do círculo (a cada " + circleEveryPercent + "%)");
                OnCircleThreshold?.Invoke();
            }
        }

        if (summonEveryPercent > 0f)
        {
            int steps = Mathf.FloorToInt(lostPercent / summonEveryPercent);

            while (summonStepsFired < steps)
            {
                summonStepsFired++;
                if (logEvents) Debug.Log("[Boss] Limiar da invocação (a cada " + summonEveryPercent + "%)");
                OnSummonThreshold?.Invoke();
            }
        }

        if (!fleeFired && remainingPercent <= fleeAtPercent)
        {
            fleeFired = true;
            if (logEvents) Debug.Log("[Boss] Limiar da fuga (vida restante <= " + fleeAtPercent + "%)");
            OnFleeThreshold?.Invoke();
        }
    }

    // Testes sem precisar lutar: botão direito no componente, com o jogo rodando
    [ContextMenu("Testar dano de 2% da vida")]
    private void TestDamage2()
    {
        if (Application.isPlaying) TakeDamage(maxHealth * 0.02f);
    }

    [ContextMenu("Testar dano de 25% da vida")]
    private void TestDamage25()
    {
        if (Application.isPlaying) TakeDamage(maxHealth * 0.25f);
    }
}
