using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    // Makes class a singleton
    public static ProjectilePool Instance { get; private set; }

    // Settings for each pool
    [System.Serializable]
    public class Entry {
        public int prewarm = 100;
        [Header("Identifiers")]
        public Projectile prefab;
    }
    [SerializeField] private Entry[] entries;

    // Creates a dictionary that connects a stack of projectiles to their prefab
    private readonly Dictionary<Projectile, Stack<Projectile>> pools = new();
    private readonly Dictionary<Projectile, System.Action<Projectile>> defaultConfigs = new();
    // Creates a dictionary that connects a prefab to a ID
    public readonly Dictionary<int, Projectile> poolsID = new();

    [SerializeField] private string targetTag = "Player";
    private GameObject[] taggedObjects;

    void Awake() {
        // Singleton logic
        Instance = this;

        // Gets Player
        taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        if (taggedObjects.Length < 1)
        {
            Debug.Log("Colocar Tag Player no objeto player!!!");
        }

        // For each type of projectole
        int id = 0;
        foreach (var e in entries) {
            // Creates a stack of 'prewarm' size
            var stack = new Stack<Projectile>(e.prewarm);
            // Instantiates Objects, deactivates them and puts them on the stack
            for (int i = 0; i < e.prewarm; i++) {
                var p = Instantiate(e.prefab, transform);
                p.gameObject.SetActive(false);
                p.prefab = e.prefab;
                p.target = taggedObjects[0].transform;
                stack.Push(p);
            }
            // Stores default values
            defaultConfigs[e.prefab] = p => {
                        p.initialSpeed = e.prefab.initialSpeed;
                        p.damage = e.prefab.damage;
                        // p.lifeTime = attack.lifeTime;
                        p.turnRate = e.prefab.turnRate;
                        p.originPoint = e.prefab.originPoint;
                        p.radius = e.prefab.radius;
                    };
            // Creates a poolID
            poolsID[id] = e.prefab;
            id += 1;
            // Adds the stack to the pools
            pools[e.prefab] = stack;
        }
    }

    public Projectile Get(Projectile prefab) {
        // Chooses the stack based on the prefab called
        var stack = pools[prefab];

        // If there is a projectile available, activate it
        // Otherwise Instantiate a new one
        Projectile p;
        if (stack.Count > 0){p = stack.Pop();}
        else{
            p = Instantiate(prefab, transform);
            p.prefab = prefab;
        }

        // Activates the projectile and returns it
        p.gameObject.SetActive(true);
        return p;
    }
    public Projectile Get(int poolID) {
        // Chooses the stack based on the int given
        var prefab = poolsID[poolID];
        var stack = pools[prefab];

        // If there is a projectile available, activate it
        // Otherwise Instantiate a new one
        Projectile p;
        if (stack.Count > 1){p = stack.Pop();}
        else{
            p = Instantiate(prefab, transform);
            p.prefab = prefab;
            p.target = taggedObjects[0].transform;
        }

        // Activates the projectile and returns it
        p.gameObject.SetActive(true);
        return p;
    }

    // Returns Projectiles to pool
    public void Return(Projectile p) {
        // Returns to pool
        pools[p.prefab].Push(p);
        // Returns default values
        defaultConfigs[p.prefab].Invoke(p);
        // Deactivates
        p.gameObject.SetActive(false);
        p.transform.SetParent(transform, false);
    }
}
public enum ProjectileMovement
{
    Straight,
    Curve, 
    Circling,
    Homing,
    Fast,
    Reflective
}
