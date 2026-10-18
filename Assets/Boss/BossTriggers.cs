using System.Collections;
using UnityEngine;

public class BossTriggers : MonoBehaviour
{    
    [SerializeField] private EnemyAttackController fase1Pattern;
    [SerializeField] private EnemyAttackController fase2Pattern;
    [SerializeField] private EnemyAttackController fase3Pattern;
    [SerializeField] AudioController audioController;
    // Teleport Logic
    private Coroutine teleportCoroutine;
    private TeleportControl teleportControl;
    private int locationIndex = 0;
    [Tooltip("Teleporte Cooldown")]
    [SerializeField]private float teleportInterval = 10f;
    // Enemy Spawn Logic
    [Tooltip("Spawn Cooldown")]
    [SerializeField]private float spawnInterval = 20f;
    private SpawningControl spawningControl;
    private Coroutine spawningCoroutine;
    private Coroutine teleportingCoroutine;
    private EnemyAttackController currentattackController;

    private void Awake()
    {
        audioController = FindAnyObjectByType<AudioController>();

        teleportControl = GetComponent<TeleportControl>();
        spawningControl = GetComponent<SpawningControl>();

        currentattackController = fase1Pattern;

        StartCoroutine(TestSequence());
    }

    public void EnterFase(int i)
    {
        if (i == 1)
        {
            fase1Pattern.DeactivateAttackSequence();
            fase1Pattern.enabled = false;

            fase2Pattern.enabled = true;
            fase2Pattern.ActivateAttackSequence();
            currentattackController = fase2Pattern;
            // Comeca a teleportar
            teleportCoroutine = StartCoroutine(UseTeleport());
        }
        else if (i == 2)
        {
            fase2Pattern.DeactivateAttackSequence();
            fase2Pattern.enabled = false;

            fase3Pattern.enabled = true;
            fase3Pattern.ActivateAttackSequence();
            currentattackController = fase3Pattern;
            // Comeca a spawnar inimigos
            StartCoroutine(UseSpawn());
        }
    }

    private IEnumerator UseTeleport()
    {
        while (true)
        {
            var aux = teleportControl.TeleportRandomLocation(locationIndex);
            locationIndex = aux;
            yield return new WaitForSeconds(teleportInterval);
        }
    }
    private IEnumerator UseSpawn()
    {
        while (true)
        {
            spawningControl.SpawnMonsters();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator TestSequence()
    {
        yield return new WaitForSeconds(5f);
        EnterFase(1);
        yield return new WaitForSeconds(15f);
        EnterFase(2);
    }
}
