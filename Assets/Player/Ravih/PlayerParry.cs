
using UnityEngine;
using System.Collections;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry")]
    [SerializeField] private KeyCode parryKey = KeyCode.E;
    [SerializeField] private float parryDuration = 0.6f;
    [SerializeField] private float parryCooldown = 2f;

    private PlayerMovement playerMovement;
    private bool isParrying;
    private bool isOnCooldown;

    public bool IsParrying => isParrying;
    public bool IsOnCooldown => isOnCooldown;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
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

        yield return new WaitForSeconds(parryCooldown);

        isOnCooldown = false;
    }
}