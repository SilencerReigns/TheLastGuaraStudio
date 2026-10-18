using System.Collections;
using DG.Tweening;
using UnityEngine;
using Unity.Cinemachine;
using HeyHeyThere.PixelBarsFree;

public class HealthBar : MonoBehaviour
{
    [Header("Objetos")]
    [SerializeField] private BossTriggers triggerScript;
    [SerializeField] PixelStatBar bossBar;  
    [SerializeField] ScreenEffectsMain effectsScript;
    [SerializeField] Texture2D bossTexture;

    [SerializeField] EnemyAttackController c1;
    [SerializeField] EnemyAttackController c2;
    [SerializeField] EnemyAttackController c3;
    
    [Header("Configs")]
    [SerializeField] float maxHealth = 1000;
    [SerializeField] float DecayDuration = 0.5f;   
    [SerializeField] float hitFlashDuration = 0.1f;

    [SerializeField] float[] Triggers;
    private int tIndex = 0;
    
    private float health;
    private bool imune;

    private Material bossMaterial;

    void Awake()
    {
        bossMaterial = GetComponent<SpriteRenderer>().material;
        bossMaterial.SetTexture("_Texture", bossTexture);
    }

    void Start()
    {
        health = maxHealth;

        bossBar.maxValue = maxHealth;
        bossBar.value = health;
        bossBar.Settle();
    }

    private void destroyHealth()
    {
        bossBar.transform.DOKill();

        bossBar.transform.DOScaleX(0f, DecayDuration).SetEase(Ease.InOutElastic);
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

    
        bossBar.value = Mathf.Max(health, 0f);

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