using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawningControl : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Number of Monsters to Spawn")]
    [SerializeField]private int monsterCount = 15;
    
    [Tooltip("Spawning Distance")]
    [SerializeField]private float distance = 20f;
    
    private Transform center;
    
    private List<Vector3> foundSpots = new List<Vector3>();
    [Header("Prefabs dos Inimigos")]
    [SerializeField]private GameObject prefab1;
    [SerializeField]private GameObject prefab2;

    private void Awake()
    {
        center = transform;
        if(prefab1 == null || prefab2 == null)
        {
            Debug.Log("Colocar Referencia para os inimigos no script SpawningControl !!!");
        }
    }

    public void SpawnMonsters()
    {
        var spotList = FindSpots();
        foreach (Vector3 p in spotList)
        {
            Random.Range(0,2);
            if(Random.Range(0,2) == 1)
            {
                Instantiate(prefab1, p, Quaternion.identity);
            }else
            {
                Instantiate(prefab2, p, Quaternion.identity);
            }
        }
    }
    /// <summary>
    /// Call this to find the spots. Returns the list of positions.
    /// </summary>
    private List<Vector3> FindSpots()
    {
        Transform origin = center != null ? center : transform;
        foundSpots.Clear();

        int attempts = 0;

        while (foundSpots.Count < monsterCount)
        {
            attempts++;

            // Pick a random point on a circle around the origin
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float heightOffset = Random.Range(-0.5f, 0.5f) * distance * 0.2f;

            Vector3 candidate = origin.position + new Vector3(
                Mathf.Cos(angle) * distance,
                heightOffset,
                Mathf.Sin(angle) * distance
            );

            foundSpots.Add(candidate);
        }

        return foundSpots;
    }

    private void DrawCircle(Vector3 centerPos, float radius, int segments)
    {
        Vector3 prev = centerPos + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = centerPos + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
