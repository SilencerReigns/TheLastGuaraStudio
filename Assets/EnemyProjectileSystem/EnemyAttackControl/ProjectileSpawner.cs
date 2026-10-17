using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]private EnemyAttackController controller;    
    void Start()
    {
        StartCoroutine(Testing());
    }
    IEnumerator Testing()
    {
        controller.ActivateAttackSequence();
        yield return new WaitForSeconds(20);
        controller.DeactivateAttackSequence();
    }

}
