using HeyHeyThere.PixelBarsFree;
using Player;
using System.Collections;
using UnityEngine;

public class HealthPlayer : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private PixelStatBar healthBar;

    public float MaxHealth = 100f;
    public float health;

    [SerializeField] private StateController stateController;
    private Core core => (Core)stateController.GetCore();

    [Header("Potion")]
    [SerializeField] private KeyCode potionKey = KeyCode.Q;
    [SerializeField] private float potionHealAmount = 30f;
    [SerializeField] private float potionDrinkDuration = 1f;
    [SerializeField] private float potionCooldown = 10f;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PixelStatBar potionRing;
    [SerializeField] private KeyCode debugKillKey = KeyCode.R;




    private bool isHealing;
    private bool isPotionOnCooldown;
    private bool isDead;

    public bool IsHealing => isHealing;
    public bool IsPotionOnCooldown => isPotionOnCooldown;
    public bool IsDead => isDead;


    public static event System.Action OnPlayerDied;

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
    }

    private void Start()
    {
        core.canheal = true;
        health = MaxHealth;

        healthBar.maxValue = MaxHealth;
        healthBar.value = health;
        healthBar.Settle();

        // anel cheio = poção pronta
        if (potionRing != null)
        {
            potionRing.trail = potionRing.flash = false;
            potionRing.value = potionRing.maxValue;
            potionRing.Settle();
        }
    }

    private void Update()
    {
        healthBar.value = health;

        if (isDead) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(debugKillKey))
        {
            Die();
            return;
        }
#endif

        if (Input.GetKeyDown(potionKey) &&
            !isHealing &&
            !isPotionOnCooldown)
        {
            StartCoroutine(DrinkPotion());
        }
    }

    public void TakeDamage(float damage, Vector2? damageSource = null)
    {
        if (isDead) return;

        health = Mathf.Clamp(health - damage, 0f, MaxHealth);

        if (health <= 0f)
        {
            Die(damageSource);
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        health = Mathf.Clamp(health + amount, 0f, MaxHealth);
    }
    public void Die(Vector2? damageSource = null)
    {
        if (isDead) return;
        isDead = true;
        health = 0f;

        StopAllCoroutines();
        isHealing = false;

        if (playerMovement != null)
        {
            playerMovement.SetMovementLocked(true);
        }

        if (damageSource.HasValue)
        {
            core.deathDirection = ((Vector3)(damageSource.Value - (Vector2)transform.position)).normalized;
        }
        else
        {
            core.deathDirection = new Vector3(core.facingDirection == 0 ? 1 : core.facingDirection, 0f, 0f);
        }
        stateController.SetState<DeathState>(false);
        OnPlayerDied?.Invoke();
    }

    private IEnumerator DrinkPotion()
    {
        isHealing = true;
        isPotionOnCooldown = true;

        if (potionRing != null)
        {
            potionRing.value = 0f;
            potionRing.Settle();
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementLocked(true);
        }

        yield return new WaitForSeconds(potionDrinkDuration);

        if (health > 0f)
        {
            Heal(potionHealAmount);
        }

        isHealing = false;

        if (playerMovement != null)
        {
            playerMovement.SetMovementLocked(false);
        }

        float elapsed = 0f;

        while (elapsed < potionCooldown)
        {
            elapsed += Time.deltaTime;

            if (potionRing != null)
            {
                potionRing.value = potionRing.maxValue * (elapsed / potionCooldown);
            }

            yield return null;
        }

        if (potionRing != null)
        {
            potionRing.value = potionRing.maxValue;
        }

        isPotionOnCooldown = false;
        core.canheal = true;
}   }