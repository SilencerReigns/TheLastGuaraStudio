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

    // [SerializeField] private DeathCutscene deathScript;

    [SerializeField] private StateController stateController;
    private Core core => (Core)stateController.GetCore();

    [Header("Potion")]
    [SerializeField] private KeyCode potionKey = KeyCode.Q;
    [SerializeField] private float potionHealAmount = 30f;
    [SerializeField] private float potionDrinkDuration = 1f;  
    [SerializeField] private float potionCooldown = 10f;   
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PixelStatBar potionRing;       

    private bool isHealing;
    private bool isPotionOnCooldown;

    public bool IsHealing => isHealing;
    public bool IsPotionOnCooldown => isPotionOnCooldown;

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

        if (Input.GetKeyDown(potionKey) &&
            !isHealing &&
            !isPotionOnCooldown)
        {
            StartCoroutine(DrinkPotion());
        }
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, MaxHealth);

      /*  if (health <= 0f)
        {
            deathScript.DeathScene();
        }*/
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, MaxHealth);
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