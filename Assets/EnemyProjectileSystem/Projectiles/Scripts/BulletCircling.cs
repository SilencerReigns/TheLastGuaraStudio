using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletCircling : Projectile
{
    public override void Launch(Vector3 pos, Quaternion rot) {
        // Sets initial position and rotation
        transform.SetPositionAndRotation(pos, rot);
        // Sets initial velocity
        velocity = rot * Vector3.left * initialSpeed;
        // Reset each launch
        dead = false;
        timeAlive = 0f;
    }

    protected override void MoveProjectile(float dt) {
        // DEEPSEEK MADE THIS ONE
        float omega = initialSpeed / radius;            // rad/s
        float dTheta = omega * dt;                      // radians this frame

        // rotate current offset from center by dTheta
        Vector3 offset = transform.position - originPoint;
        offset = Quaternion.AngleAxis(dTheta * Mathf.Rad2Deg, Vector3.forward) * offset;
        transform.position = originPoint + offset;

        // keep velocity tangential (for collisions / facing / etc.)
        velocity = Quaternion.AngleAxis(dTheta * Mathf.Rad2Deg, Vector3.forward) * velocity;
        velocity = velocity.normalized * initialSpeed;

        // optional: face the direction of travel
        transform.right = velocity.normalized;
    }
}
