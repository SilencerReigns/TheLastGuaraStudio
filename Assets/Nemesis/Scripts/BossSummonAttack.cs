using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ataque 3 do boss: invoca uma leva de inimigos e fica inalvejável até todos serem derrotados.
// Os inimigos invocados são os mesmos prefabs do jogo (com EnemyHealth).
public class BossSummonAttack : MonoBehaviour
{
    [Header("Inimigos invocados")]
    [Tooltip("Prefabs dos inimigos (ex.: enemyMelee). Se houver mais de um, cada inimigo sorteia um deles.")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemyCount = 5;
    [Tooltip("Distância do boss em que os inimigos aparecem (em círculo)")]
    [SerializeField] private float spawnRadius = 4f;
    [Tooltip("Alcance de perseguição dos inimigos invocados (eles precisam alcançar o player)")]
    [SerializeField] private float enemyDetectionRange = 30f;

    [Header("Tempo")]
    [Tooltip("Tempo desde o início da animação até os inimigos começarem a aparecer")]
    [SerializeField] private float spawnDelay = 0.8f;
    [Tooltip("Intervalo entre o aparecimento de um inimigo e o próximo")]
    [SerializeField] private float spawnInterval = 0.1f;

    [Header("Boss inalvejável")]
    [Tooltip("Sprite do boss que muda de cor enquanto ele está inalvejável. Se vazio, usa o do próprio Nemesis.")]
    [SerializeField] private SpriteRenderer bossSprite;
    [SerializeField] private Color invulnerableColor = new Color(0.55f, 0.75f, 1f, 1f);

    private BossHealth bossHealth;
    private BossAnimator bossAnimator;
    private Color originalColor = Color.white;

    private readonly List<EnemyHealth> aliveEnemies = new List<EnemyHealth>();
    private int castsInProgress;   // invocações que ainda estão aparecendo
    private bool watching;

    private void Awake()
    {
        bossHealth = GetComponent<BossHealth>();
        bossAnimator = GetComponent<BossAnimator>();

        if (bossSprite == null) bossSprite = GetComponent<SpriteRenderer>();
        if (bossSprite != null) originalColor = bossSprite.color;
    }

    // Faz a invocação e termina quando a animação acaba.
    // O boss continua inalvejável até os inimigos morrerem (isso é acompanhado em WatchEnemies).
    public IEnumerator Execute()
    {
        bool validPrefabs = enemyPrefabs != null && enemyPrefabs.Length > 0;

        if (!validPrefabs)
        {
            Debug.LogError("BossSummonAttack: arraste ao menos um prefab de inimigo em 'Enemy Prefabs'.");
            yield return new WaitForSeconds(bossAnimator.PlayAttack3());
            yield break;
        }

        // Inalvejável já durante a invocação
        castsInProgress++;
        SetInvulnerable(true);

        if (!watching) StartCoroutine(WatchEnemies());

        float startTime = Time.time;
        float length = bossAnimator.PlayAttack3();

        yield return new WaitForSeconds(spawnDelay);
        yield return SpawnEnemies();

        castsInProgress--;

        // O boss fica livre para outros ataques quando a animação terminar
        float remaining = length - (Time.time - startTime);
        if (remaining > 0f) yield return new WaitForSeconds(remaining);
    }

    private IEnumerator SpawnEnemies()
    {
        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < enemyCount; i++)
        {
            float angle = (startAngle + i * 360f / enemyCount) * Mathf.Deg2Rad;
            Vector3 position = transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            if (prefab != null)
            {
                GameObject enemy = Instantiate(prefab, position, Quaternion.identity);

                EnemyHealth health = enemy.GetComponentInChildren<EnemyHealth>();

                if (health != null)
                {
                    aliveEnemies.Add(health);
                }
                else
                {
                    Debug.LogWarning("BossSummonAttack: o prefab '" + prefab.name + "' não tem EnemyHealth, então o boss não espera por ele.");
                }

                // Os invocados precisam perseguir o player mesmo de longe
                MeleeMove move = enemy.GetComponent<MeleeMove>();
                if (move != null) move.SetDetectionRange(enemyDetectionRange);
            }

            if (i < enemyCount - 1 && spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Espera todos os inimigos serem derrotados e então tira a invulnerabilidade
    private IEnumerator WatchEnemies()
    {
        watching = true;

        while (true)
        {
            // derrotado = destruído ou já morrendo (animação de morte)
            aliveEnemies.RemoveAll(e => e == null || e.IsDead);

            if (castsInProgress == 0 && aliveEnemies.Count == 0) break;

            yield return null;
        }

        watching = false;
        SetInvulnerable(false);
        Debug.Log("[Boss] Todos os inimigos foram derrotados: boss vulnerável de novo");
    }

    private void SetInvulnerable(bool value)
    {
        bossHealth.SetInvulnerable(value);

        if (bossSprite != null)
        {
            bossSprite.color = value ? invulnerableColor : originalColor;
        }
    }
}
