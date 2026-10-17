using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class HealthBar : MonoBehaviour
{
    [Header("Objetos")]
    [SerializeField] private BossTriggers triggerScript;
    [SerializeField] RectTransform backgroundBar;
    [SerializeField] RectTransform chunkBar;
    [SerializeField] RectTransform fillBar;
    [SerializeField] ScreenEffectsMain effectsScript;
    [SerializeField] Texture2D bossTexture;

    [SerializeField] EnemyAttackController c1;
    [SerializeField] EnemyAttackController c2;
    [SerializeField] EnemyAttackController c3;
    
    [Header("Configs")]
    [SerializeField] float maxLenght = 1750;
    [SerializeField] float maxHealth = 1000;
    [SerializeField] float DecayDuration = 0.5f;
    [SerializeField] float ChunkDelay = 0.25f;
    [SerializeField] float hitFlashDuration = 0.1f;

    [SerializeField] float[] Triggers;
    private int tIndex = 0;
    
    private float health;
    private bool imune;

    private Image fillImage;
    private Image backgroundImage;

    private Color originalFillColor;
    private Color originalBgColor;

    private Coroutine damageCoroutine;
    private Material bossMaterial;

    void Awake()
    {
        bossMaterial = GetComponent<SpriteRenderer>().material;
        bossMaterial.SetTexture("_Texture", bossTexture);
    }

    void Start()
    {
        health = maxHealth;
        fillImage = fillBar.GetComponent<Image>();
        backgroundImage = backgroundBar.GetComponent<Image>();
    
        originalFillColor = fillImage.color;
        originalBgColor = backgroundImage.color;
    }

    private IEnumerator lerpDamage(float currentHealth)
    {
        float percent = currentHealth / maxHealth;
        float lenghtPercent = percent * maxLenght;

        fillBar.DOKill();
        chunkBar.DOKill();
        fillImage.DOKill();

        fillBar.DOSizeDelta(new Vector2(lenghtPercent, fillBar.sizeDelta.y), 0.15f).SetEase(Ease.OutCubic);
        
        fillImage.DOColor(Color.white, 0.05f);

        yield return new WaitForSeconds(0.05f);

        fillImage.DOColor(originalFillColor, DecayDuration);

        yield return new WaitForSeconds(ChunkDelay);

        chunkBar.DOSizeDelta(new Vector2(lenghtPercent, chunkBar.sizeDelta.y), DecayDuration).SetEase(Ease.OutQuint);
    }

    private void destroyHealth()
    {
        fillBar.DOKill();
        chunkBar.DOKill();
        backgroundBar.DOKill();

        fillBar.DOSizeDelta(new Vector2(0f, fillBar.sizeDelta.y), DecayDuration).SetEase(Ease.InOutElastic);
        chunkBar.DOSizeDelta(new Vector2(0f, chunkBar.sizeDelta.y), DecayDuration).SetEase(Ease.InOutElastic);
        backgroundBar.DOSizeDelta(new Vector2(0f, backgroundBar.sizeDelta.y), DecayDuration).SetEase(Ease.InOutElastic);
    }

    private IEnumerator FlashRoutine() {
        bossMaterial.SetFloat("_Active", 1f); 

        yield return new WaitForSeconds(hitFlashDuration);

        bossMaterial.SetFloat("_Active", 0f); 
    }

    public bool TakeDamage(float damage)
    {
        if (imune) return false;

        health -= damage;
        
        if (damageCoroutine != null) StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(lerpDamage(health));
        StartCoroutine(FlashRoutine());
        
        if (tIndex < Triggers.Length && health <= maxHealth * (Triggers[tIndex]) / 100)
        {
            tIndex++;
            effectsScript.FrameFreeze(0.2f);
            effectsScript.ScreenFlash(0.2f);
            effectsScript.ScreenShake(new Vector3(0f, -1f, 0f), 1f, CinemachineImpulseDefinition.ImpulseShapes.Explosion);
            triggerScript.EnterFase(tIndex);
        }

        if (health <= 0)
        {
            imune = true;

            //Time.timeScale = 0f;
            c1.enabled = false;
            c2.enabled = false;
            c3.enabled = false;
            
            StartCoroutine(bossDeath());

            destroyHealth();
        }

        return true;
    }

    private IEnumerator bossDeath()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(sprite.DOFade(0.5f, 0.5f).SetUpdate(true)).SetUpdate(true);
        sequence.Append(sprite.DOFade(1f, 0.5f).SetUpdate(true)).SetUpdate(true);

        sequence.SetLoops(-1);
        sequence.Play();

        Camera.main.DOShakePosition(3f, 3f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(3f);
        sprite.DOFade(0f, 1f).SetUpdate(true);
        Destroy(this.gameObject);
    }
}