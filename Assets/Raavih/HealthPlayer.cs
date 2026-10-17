using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : MonoBehaviour
{
    [Header("Health")]
    //public Slider easeHealthSlider;
    //public Slider healthSlider;

    public float MaxHealth = 100f;
    public float health;

    //[SerializeField] private DeathCutscene deathScript;

    private float lerpSpeed = 0.05f;

    [Header("Potion")]
    [SerializeField] private KeyCode potionKey = KeyCode.Q;
    [SerializeField] private float potionHealAmount = 30f;
    [SerializeField] private float potionDrinkDuration = 1f; 
    [SerializeField] private float potionCooldown = 10f;     
    [SerializeField] private PlayerMovement playerMovement;

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
        health = MaxHealth;

       // healthSlider.maxValue = MaxHealth;
      //  easeHealthSlider.maxValue = MaxHealth;

       // healthSlider.value = health;
       // easeHealthSlider.value = health;
    }

    private void Update()
    {
        /*if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (easeHealthSlider.value != health)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
        }*/

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

       /* if (health <= 0f)
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

        yield return new WaitForSeconds(potionCooldown);

        isPotionOnCooldown = false;
    }
}