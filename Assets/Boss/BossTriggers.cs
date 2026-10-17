using System.Collections;
using UnityEngine;

public class BossTriggers : MonoBehaviour
{
    [SerializeField] SpriteRenderer bossSpriteRenderer;
    [SerializeField] Animator bossAnimator;
    
    [SerializeField] private Material bossMaterial;
    [SerializeField] private Texture2D fase2;
    [SerializeField] private Texture2D fase3;
    [SerializeField] private EnemyAttackController fase1Pattern;
    [SerializeField] private EnemyAttackController fase2Pattern;
    [SerializeField] private EnemyAttackController fase3Pattern;
    [SerializeField] AudioController audioController;
    [SerializeField] AudioClip clip;

    private Coroutine teleportCoroutine;
    private TeleportControl teleportControl;
    private int locationIndex = 0;
    [Tooltip("Teleporte Cooldown")]
    [SerializeField]private float teleportInterval = 10f;

    [Tooltip("Spawn Cooldown")]
    [SerializeField]private float spawnInterval = 20f;
    private SpawningControl spawningControl;
    private Coroutine spawningCoroutine;

    private void Awake()
    {   
        audioController = FindAnyObjectByType<AudioController>();
        bossMaterial = GetComponent<SpriteRenderer>().material;

        teleportControl = GetComponent<TeleportControl>();
        spawningControl = GetComponent<SpawningControl>();

        if (bossSpriteRenderer == null)
        {
            Debug.LogWarning("Boss Sprite Renderer nao foi declarado");
            bossSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void EnterFase(int i)
    {
        audioController.PlaySFX(clip);
        if (i == 1)
        {
            bossAnimator.SetInteger("Fase", 2);
            bossMaterial.SetTexture("_Texture", fase2);
            fase1Pattern.DeactivateAttackSequence();
            fase1Pattern.enabled = false;
            fase2Pattern.enabled = true;
            fase2Pattern.ActivateAttackSequence();
            // Comeca a teleportar
            teleportCoroutine = StartCoroutine(UseTeleport());
        }
        else if (i == 2)
        {
            bossAnimator.SetInteger("Fase", 3);
            bossMaterial.SetTexture("_Texture", fase3);
            fase2Pattern.DeactivateAttackSequence();
            fase2Pattern.enabled = false;
            fase3Pattern.enabled = true;
            fase3Pattern.ActivateAttackSequence();
            // Comeca a spawnar inimigos
            StartCoroutine(UseSpawn());
        }
    }

    private IEnumerator UseTeleport()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);
            var aux = teleportControl.TeleportRandomLocation(locationIndex);
            locationIndex = aux;
        }
    }

    private IEnumerator UseSpawn()
    {
        yield return new WaitForSeconds(spawnInterval);
        spawningControl.SpawnMonsters();
    }

}
