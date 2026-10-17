using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Projectile : MonoBehaviour {

    [Header("Life Logic")]
    protected float timeAlive = 0f;
    public bool dead = false;

    [Header("References")]
    public Projectile prefab;

    [Header("Attributes")]
    public float initialSpeed = 5f;
    public float damage = 10f;
    public float lifeTime = 10f;
    public float turnRate = 45f;
    public Vector3 originPoint;
    public float radius = 5f;
    public Transform target;


    // [Header("State")]
    protected Vector3 velocity;

    public virtual void Launch(Vector3 pos, Quaternion rot) {
        // Sets initial position and rotation
        transform.SetPositionAndRotation(pos, rot);
        // Sets initial velocity
        velocity = rot * Vector3.left * initialSpeed;
        // Reset each launch
        dead = false;
        timeAlive = 0f;
    }

    public void Tick(float dt) {
        MoveProjectile(dt);
        timeAlive += dt;
        if(timeAlive>lifeTime){dead = true;}
    }

    protected abstract void MoveProjectile(float dt);

    void Despawn()
    {
        dead = true;
    }
}
