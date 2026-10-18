using Unity.Mathematics;
using UnityEngine;

public class BulletReflective : Projectile
{
    [SerializeField]private float screenWidth=9f;
    [SerializeField]private float screenHeight=5f;
    protected override void MoveProjectile(float dt) {
        // Reverse in case it gets past the wall
        if(transform.position.x + velocity.x*dt < -screenWidth)
        {
            velocity.x = -1 * velocity.x;
        }
        if(math.abs(transform.position.y + velocity.y*dt) > screenHeight)
        {
            velocity.y = -1 * velocity.y;
        }
        // Moves
        transform.position += velocity * dt;

        velocity = velocity.normalized * initialSpeed;

        transform.right = velocity.normalized;
    }
}
