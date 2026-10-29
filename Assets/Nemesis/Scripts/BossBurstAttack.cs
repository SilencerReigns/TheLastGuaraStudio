using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ataque 1 do boss: rajada de projéteis teleguiados em leque.
// Os projéteis são BossProjectile (prefab). Se o parry estiver ativo quando um deles chega ao player, ele é bloqueado.
public class BossBurstAttack : MonoBehaviour
{
    [Header("Projéteis")]
    [SerializeField] private BossProjectile projectilePrefab;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileSpeed = 6f;
    [Tooltip("Quanto o projétil consegue curvar em direção ao player (graus por segundo)")]
    [SerializeField] private float homingTurnRate = 60f;
    [Tooltip("Distância do projétil ao player para contar como acerto")]
    [SerializeField] private float hitRadius = 0.5f;
    [Tooltip("Tempo de vida do projétil, em segundos")]
    [SerializeField] private float projectileLifetime = 8f;

    [Header("Rajada")]
    [SerializeField] private int projectilesPerWave = 7;
    [SerializeField] private int waves = 3;
    [SerializeField] private float waveInterval = 0.25f;
    [Tooltip("Abertura total do leque, em graus")]
    [SerializeField] private float spreadAngle = 60f;
    [Tooltip("Tempo desde o início da animação até a rajada sair")]
    [SerializeField] private float fireDelay = 0.65f;
    [Tooltip("De onde os projéteis saem (filho ProjectileOrigin). Se vazio, sai do centro do boss.")]
    [SerializeField] private Transform projectileOrigin;

    private Transform playerPosition;
    private HealthPlayer playerHealth;
    private PlayerParry playerParry;
    private readonly List<BossProjectile> bullets = new List<BossProjectile>();

    public float FireDelay => fireDelay;

    private void Start()
    {
        GameObject targetObj = GameObject.Find("PlayerRF/Body/Hitbox");

        if (targetObj != null)
        {
            playerPosition = targetObj.transform;
            playerHealth = targetObj.GetComponentInParent<HealthPlayer>();
            playerParry = targetObj.GetComponentInParent<PlayerParry>();
        }
        else
        {
            Debug.LogError("BossBurstAttack: O caminho 'PlayerRF/Body/Hitbox' não foi encontrado na cena!");
        }
    }

    // Dispara todas as ondas e só termina quando a última sai
    public IEnumerator FireWaves()
    {
        for (int w = 0; w < waves; w++)
        {
            FireWave();

            if (w < waves - 1) yield return new WaitForSeconds(waveInterval);
        }
    }

    // Some com os projéteis que ainda estão no ar (ex.: quando o boss morre)
    public void ClearBullets()
    {
        foreach (BossProjectile p in bullets)
        {
            if (p != null) Destroy(p.gameObject);
        }

        bullets.Clear();
    }

    private void FireWave()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("BossBurstAttack: falta arrastar o prefab do projétil em 'Projectile Prefab'.");
            return;
        }

        if (playerPosition == null) return;

        Vector3 origin = projectileOrigin != null ? projectileOrigin.position : transform.position;

        // Direção do player: o centro do leque aponta para ele
        Vector2 toPlayer = (Vector2)playerPosition.position - (Vector2)origin;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        bullets.RemoveAll(b => b == null);

        for (int i = 0; i < projectilesPerWave; i++)
        {
            // de -0.5 a +0.5, distribuído igualmente pelo leque
            float t = projectilesPerWave == 1 ? 0f : (float)i / (projectilesPerWave - 1) - 0.5f;
            float angle = (baseAngle + t * spreadAngle) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            BossProjectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
            projectile.Launch(direction, projectileSpeed, projectileDamage, homingTurnRate,
                              hitRadius, projectileLifetime, playerPosition, playerHealth, playerParry);

            bullets.Add(projectile);
        }
    }

    private void OnDestroy()
    {
        ClearBullets();
    }
}
