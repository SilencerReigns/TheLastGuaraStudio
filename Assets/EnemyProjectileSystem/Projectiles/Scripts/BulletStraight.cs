using UnityEngine;

public class BulletStraight : Projectile
{
    protected override void MoveProjectile(float dt) {
        transform.position += velocity * dt;

        velocity = velocity.normalized * initialSpeed;

        transform.right = velocity.normalized;
    }
}
