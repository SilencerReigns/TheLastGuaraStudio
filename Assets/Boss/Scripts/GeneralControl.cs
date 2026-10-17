using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TeleportControl))]
public class GeneralControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TeleportControl teleportControl;
    private EnemyAttackController enemyAttackController;
    void Start()
    {
        teleportControl = GetComponent<TeleportControl>();
        enemyAttackController = GetComponent<EnemyAttackController>();

        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Test()
    {
        enemyAttackController.ActivateAttackSequence();
        yield return new WaitForSeconds(3f);
        teleportControl.TeleportRandomLocation(0);
    }
}
