using System.Linq;
using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    [SerializeField]private GameObject teleportList;
    private Transform[] teleportLocations;

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
        //Teleport();
        transform.position = teleportLocations[choice].position;
        return choice;
    }
    public void TeleportToChosenIndex(int chosenIndex)
    {
        // Teleporta
        //Teleport();
        transform.position = teleportLocations[chosenIndex].position;
    }
    private void Teleport()
    {   // talvez usar essa funcao pra animar ou fazer efeito?
        
    }

    public void PrintAllTeleportLocations()
    {   // Funcao de teste
        foreach (Transform t in teleportLocations)
        {
            Debug.Log(t.position);
        }
    }
}
