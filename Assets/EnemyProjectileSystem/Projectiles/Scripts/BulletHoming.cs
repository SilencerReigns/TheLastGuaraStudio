using UnityEngine;
using UnityEngine.UIElements;

public class BulletHoming : Projectile
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
        // Moves Projectile
        transform.position = transform.position + (velocity * dt);
        // Rotates Velocity Towards Player
        Vector3 toTarget = target.position - transform.position;
        var targetDir = toTarget / toTarget.magnitude;

        float currentAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float targetAngle  = Mathf.Atan2(targetDir.y, targetDir.x)  * Mathf.Rad2Deg;

        float maxDelta = turnRate * dt;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, maxDelta);

        float rad = newAngle * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        // float maxRadians = turnRate * Mathf.Deg2Rad * dt;
        // velocity = Vector3.RotateTowards(
        //     velocity,
        //     targetDir,
        //     maxRadians,
        // 0f);
        // Normalizes Velocity and Rotates Player
        velocity = velocity.normalized * initialSpeed;
        transform.right = velocity.normalized;
    }
}
