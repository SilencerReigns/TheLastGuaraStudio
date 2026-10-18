using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    [SerializeField]private GameObject teleportList;
    private Transform[] teleportLocations;
    private Animator animator;

    void Awake()
    {
        // Pega o Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    void Start()
    {
        // Pega a lista de transforms excluindo o objeto em si
        teleportLocations = teleportList.GetComponentsInChildren<Transform>();
        teleportLocations = teleportLocations.Skip(1).ToArray();
    }

    public int TeleportRandomLocation(int excludeIndex = -1)
    {
        // Random Choice
        int choice = -1;
        while (choice < 0 || choice == excludeIndex){choice = Random.Range(0,teleportLocations.Length);}
        // Teleporta
        StartCoroutine(TeleporAnim(teleportLocations[choice].position));
        return choice;
    }
    public void TeleportToChosenIndex(int chosenIndex)
    {
        // Teleporta
        StartCoroutine(TeleporAnim(teleportLocations[chosenIndex].position));
    }
    private IEnumerator TeleporAnim(UnityEngine.Vector3 desiredPosition)
    {   // talvez usar essa funcao pra animar ou fazer efeito?
        animator.SetBool("isTeleporting",true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isTeleporting",false);
        transform.position = desiredPosition;
    }

    public void PrintAllTeleportLocations()
    {   // Funcao de teste
        foreach (Transform t in teleportLocations)
        {
            Debug.Log(t.position);
        }
    }
}
