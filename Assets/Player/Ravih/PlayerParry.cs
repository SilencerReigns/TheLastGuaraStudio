using UnityEngine;
using System.Collections;
using HeyHeyThere.PixelBarsFree;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry")]
    [SerializeField] private KeyCode parryKey = KeyCode.E;
    [SerializeField] private float parryDuration = 0.6f;
    [SerializeField] private float parryCooldown = 2f;

    [Header("UI")]
    [SerializeField] private PixelStatBar parryRing; 

    private PlayerMovement playerMovement;
    private bool isParrying;
    private bool isOnCooldown;

    public bool IsParrying => isParrying;
    public bool IsOnCooldown => isOnCooldown;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
     
        if (parryRing != null)
        {
            parryRing.trail = parryRing.flash = false;
            parryRing.value = parryRing.maxValue;
            parryRing.Settle();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(parryKey) &&
            !isParrying &&
            !isOnCooldown)
        {
            StartCoroutine(PerformParry());
        }
    }

    private IEnumerator PerformParry()
    {
        isParrying = true;
        isOnCooldown = true;

        if (parryRing != null)
        {
            parryRing.value = 0f;
            parryRing.Settle();
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementLocked(true);
        }

        yield return new WaitForSeconds(parryDuration);

        isParrying = false;

        if (playerMovement != null)
        {
            playerMovement.SetMovementLocked(false);
        }

        float elapsed = 0f;

        while (elapsed < parryCooldown)
        {
            elapsed += Time.deltaTime;

            if (parryRing != null)
            {
                parryRing.value = parryRing.maxValue * (elapsed / parryCooldown);
            }

            yield return null;
        }

        if (parryRing != null)
        {
            parryRing.value = parryRing.maxValue;
        }

        isOnCooldown = false;
}   }