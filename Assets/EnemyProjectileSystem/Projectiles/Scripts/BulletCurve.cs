using UnityEngine;
using UnityEngine.UIElements;

public class BulletCurve : Projectile
{
    // public override void Launch(Vector3 pos, Quaternion rot) {
    //     // Sets initial position and rotation
    //     transform.SetPositionAndRotation(pos, rot);
    //     // Sets initial velocity
    //     velocity = rot * Vector3.left * initialSpeed;
    //     // Reset each launch
    //     dead = false;
    //     timeAlive = 0f;
    // }

    protected override void MoveProjectile(float dt) {
        transform.position = transform.position + (velocity * dt);

        velocity = Quaternion.AngleAxis(turnRate*dt, Vector3.forward) * velocity;
        velocity = velocity.normalized * initialSpeed;

        transform.right = velocity.normalized;
    }
}
