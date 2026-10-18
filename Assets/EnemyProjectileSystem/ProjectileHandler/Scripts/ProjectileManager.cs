using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Handles the calls of all active bullets
public class ProjectileManager : MonoBehaviour {
    // Makes class a singleton
    public static ProjectileManager Instance { get; private set; }
    List<Projectile> active = new();

    [Header("CoordLimits")]
    [SerializeField]private float yLimit = 6;
    [SerializeField]private float xLimit = 12;

    void Awake() {
        // Singleton logic
        Instance = this;
    }

    public void Spawn(int poolID, Vector3 pos, Quaternion rot, System.Action<Projectile> configure = null) {
        var p = ProjectilePool.Instance.Get(poolID);
        p.Launch( pos, rot);
        configure?.Invoke(p);   // mutate the instance
        active.Add(p);
        // Debug.Log(active.Count);
    }

    void Update() {
        // Only one Update called for all bullets to avoid a lot of Unity calls
        float dt = Time.deltaTime;
        for (int i = active.Count - 1; i >= 0; i--) {
            var b = active[i];
            b.Tick(dt);
            if( (b.transform.position.y > yLimit) || (b.transform.position.y < -yLimit) || (b.transform.position.x > xLimit) || (b.transform.position.x < -xLimit))
            {
                b.dead = true;
            }
            if (b.dead) { ProjectilePool.Instance.Return(b); active.RemoveAt(i); }
        }
    }
    public void DespawnAllProjectiles() {
        foreach (Projectile p in active)
        {
            p.dead = true;
        }
    }
}
