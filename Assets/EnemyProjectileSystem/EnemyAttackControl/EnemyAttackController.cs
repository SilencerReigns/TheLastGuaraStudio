using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    // Settings for each attack pattern
    [System.Serializable]
    public class AttackPattern {
        [Header("Attack Settings")]
        // public ProjectileMovement projectileType;
        public AttackSpawning attackType;
        public bool overrideProjectileAttributes = false;
        public int burstSize;

        [Header("Timing Settings")]
        public bool waitUntilPreviousAttackEnds = true;
        public float startDelay = 0f;
        public float duration = 1f;
        public float timeBetweenRepetitions = 0.1f;
        // [SerializeField]public float endDelay = 0f;

        [Header("Projectile Attributes")]
        public float initialSpeed;
        public float damage;
        // public float lifeTime;
        public float turnRate;
        public Vector3 originPoint;
        public float radius;

    }
    [SerializeField] private AttackPattern[] attacks;

    // [Header("Screen Boundaries")]
    private float screenY = 5;
    private float screenX = 11;
    private int activeAttacks = 0;
    [SerializeField]private int numberOfColums = 5;
    [SerializeField]private Transform player;
    private Coroutine mainCoroutine;
    private List<Coroutine> minorCoroutines = new List<Coroutine>();

    public void Start()
    {
        if(player == null)
        {
            Debug.Log("Adicionar referencia ao transform do player no inspetor!!!");
        }
    }

    public void ActivateAttackSequence()
    {
        // Stops infinite calling
        attacks[0].waitUntilPreviousAttackEnds = true;
        // Starts pattern loop
        mainCoroutine = StartCoroutine(Sequencing());
    }
    public void DeactivateAttackSequence()
    {
        StopCoroutine(mainCoroutine);
        foreach(Coroutine coroutine in minorCoroutines)
        {
            StopCoroutine(coroutine);
        }
    }
    void OnDisable()
    {
        DeactivateAttackSequence();
    }


    private IEnumerator Sequencing()
    {
        while (true)
        {
            // Iteration through the attacks
            foreach(AttackPattern attack in attacks)
            {
                // Override projectile attributes in runtime
                System.Action<Projectile> configure = null;
                if (attack.overrideProjectileAttributes)
                {
                    configure = p => {
                        p.target = player;
                        p.initialSpeed = attack.initialSpeed;
                        p.damage = attack.damage;
                        // p.lifeTime = attack.lifeTime;
                        p.turnRate = attack.turnRate;
                        p.originPoint = attack.originPoint;
                        p.radius = attack.radius;
                    };
                }

                // Executes the attack
                if(attack.duration > 0.01f && attack.timeBetweenRepetitions>0.01f)
                {
                    // Makes it possible to choose to use both attacks simultaneously or not
                    if(attack.waitUntilPreviousAttackEnds && activeAttacks > 0)
                    {
                        yield return new WaitUntil(() => activeAttacks == 0);
                        minorCoroutines.Add(StartCoroutine(AttackSummoner(attack,configure)));
                    }
                    else
                    {
                        minorCoroutines.Add(StartCoroutine(AttackSummoner(attack,configure)));
                    }
                }
                else
                {
                    Debug.LogError("Reconfigurar Attack Pattern!");
                }
            }
            yield return null;
        }
    }
    private IEnumerator AttackSummoner(AttackPattern attack, System.Action<Projectile> configure)
    {
        activeAttacks++;

        // Start Delay
        // Mitigates Frame Boundaries
        var interval = attack.startDelay;
        var end = Time.time + interval;
        bool timeCheck = true;
        while (timeCheck)
        {
            if (Time.time > end){timeCheck = false;}
            yield return null;
        }

        var loops = (int)(attack.duration/attack.timeBetweenRepetitions);
        if(loops<=0){loops=1;}

        var column = UnityEngine.Random.Range(0,numberOfColums);

        for (int i = 0; i < loops; i++)
        {
            // Chooses and calls an attack
            AttackPicker((int)attack.attackType, configure, attack.burstSize, column);

            // Repetition Delay
            // Mitigates Frame Boundaries
            interval = attack.timeBetweenRepetitions;
            end = Time.time + interval;
            timeCheck = true;
            while (timeCheck)
            {
                if (Time.time > end){timeCheck = false;}
                yield return null;
            }
        }

        // yield return new WaitForSeconds(attack.endDelay);

        activeAttacks--;

        yield return null;
    }

    private void AttackPicker(int attackToSpawn, System.Action<Projectile> configure, int burstSize = 0, int column = 0)
    {
        // Calls each attack function
        switch (attackToSpawn)
        {
            case (int)AttackSpawning.Forward:
                ForwardAttack(configure);
                break;
            case (int)AttackSpawning.Aimed:
                AimedAttack(configure);
                break;
            case (int)AttackSpawning.Radial:
                RadialAttack(configure, burstSize);
                break;
            case (int)AttackSpawning.TwistingRadial:
                TwistingRadialAttack(configure, burstSize);
                break;
            case (int)AttackSpawning.Checkerboard:
                CheckerboardAttack(configure, burstSize);
                break;
            case (int)AttackSpawning.Wall:
                WallAttack(configure, burstSize);
                break;
            case (int)AttackSpawning.Column:
                ColumnAttack(configure,column);
                break;
            case (int)AttackSpawning.SurpriseAttack:
                _ = SurpriseAttack(configure);
                break;
            case (int)AttackSpawning.WallBounce:
                WallBounceAttack(configure);
                break;
            case (int)AttackSpawning.TripleAttack:
                TripleAttack(configure);
                break;

            default:
                Debug.LogError("Erro ao spawnar ataque!");
                break;
        }
    }

    private void ForwardAttack(System.Action<Projectile> configure)
    {
        ProjectileManager.Instance.Spawn(
            (int)ProjectileMovement.Straight,
            transform.position,
            Quaternion.Euler(0,0,0),
            configure
        );
    }
    private void AimedAttack(System.Action<Projectile> configure)
    {
        // Geometry
        var direction = player.position - transform.position;
        var angle = Quaternion.Euler(0f, 0f, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);

        ProjectileManager.Instance.Spawn(
            (int)ProjectileMovement.Straight,
            transform.position,
            angle,
            configure
        );
    }
    private void RadialAttack(System.Action<Projectile> configure, int burstSize)
    {
        for (int i = 0; i <= burstSize; i++)
        {
            float firingAngle = i * 360f / burstSize;
            ProjectileManager.Instance.Spawn(
                (int)ProjectileMovement.Straight,
                transform.position,
                Quaternion.Euler(0,0,transform.rotation.eulerAngles.z+firingAngle),
                configure
            );
        }
    }
    private void TwistingRadialAttack(System.Action<Projectile> configure, int burstSize)
    {
        if(burstSize<=0){return;}
        
        for (int i = 0; i <= burstSize; i++)
        {
            float firingAngle = i * 360f / burstSize;
            ProjectileManager.Instance.Spawn(
                (int)ProjectileMovement.Curve,
                transform.position,
                Quaternion.Euler(0,0,transform.rotation.eulerAngles.z+firingAngle),
                configure
            );
        }
    }
    private void CheckerboardAttack(System.Action<Projectile> configure, int burstSize)
    {
        if(burstSize<=0){return;}
        if(burstSize%2!=0){burstSize-=1;}

        // Arbitrary Calculations
        var xStartingPoint = screenX + (screenY/1.5f);
        var yOffset = screenY+0.5f;

        for (int i = 0; i <= burstSize/2; i++)
        {
            var xOffset = -(screenX*4f)*(i / (float)burstSize);

            ProjectileManager.Instance.Spawn(
                (int)ProjectileMovement.Straight,
                new Vector3(xStartingPoint,0,0) + new Vector3(xOffset, yOffset, 0),
                Quaternion.Euler(0,0,45),
                configure
            );
            ProjectileManager.Instance.Spawn(
                (int)ProjectileMovement.Straight,
                new Vector3(xStartingPoint,0,0) + new Vector3(xOffset, -yOffset, 0),
                Quaternion.Euler(0,0,-45),
                configure
            );
        }
    }
    private void WallAttack(System.Action<Projectile> configure, int burstSize)
    {
        if(burstSize<=0){return;}

        // Arbitrary Calculations
        var xStartingPoint = screenX + 0.5f;
        var gap = UnityEngine.Random.Range(1,burstSize-1);

        for (int i = 0; i <= burstSize; i++)
        {
            if (i != gap)
            {
                var yOffset = ( ( 2*screenY+0.5f )*(i/(float)burstSize) ) - screenY;
                ProjectileManager.Instance.Spawn(
                    (int)ProjectileMovement.Straight,
                    new Vector3(xStartingPoint, yOffset, 0),
                    Quaternion.Euler(0,0,0),
                    configure
                );
            }
        }
    }
    private void ColumnAttack(System.Action<Projectile> configure, int column = 0)
    {
        // Arbitrary
        var burstSize = 8;

        var xStartingPoint = screenX + 0.5f;
        var yStartingPoint = ( column*2*screenY/numberOfColums ) - screenY;
        for (int i = 0; i <= burstSize/2; i++)
        {
            var yOffset = i*(4*screenY/numberOfColums)/burstSize;

            ProjectileManager.Instance.Spawn(
                (int)ProjectileMovement.Straight,
                new Vector3(xStartingPoint, yStartingPoint + yOffset, 0),
                Quaternion.Euler(0,0,0),
                configure
            );
        }
    }
    private async Task SurpriseAttack(System.Action<Projectile> configure)
    {
        var fastProjectileSpeed = ProjectilePool.Instance.poolsID[(int)ProjectileMovement.Fast].initialSpeed;
        ProjectileManager.Instance.Spawn(
            (int)ProjectileMovement.Homing,
            transform.position,
            Quaternion.Euler(0,0,0),
            configure
        );

        // Delay
        await Awaitable.WaitForSecondsAsync(1.2f);

        // Try to predict player position in .25 seconds
        var playerPosition = player.position;
        var playerVelocity = new Vector3(0f,0f,0f);
        try
        {
            playerVelocity = player.gameObject.GetComponent<Rigidbody2D>().linearVelocity;
        }
        catch (System.Exception)
        {
            Debug.LogError("Player nao tem RigidBody");
            throw;
        }
        var timeToHit = (playerPosition - transform.position).magnitude / fastProjectileSpeed;
        var playerFuturePosition = playerPosition + ( playerVelocity * timeToHit );

        // Shoot at desired position
        var direction = playerFuturePosition - transform.position;
        var angle = Quaternion.Euler(0f, 0f, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);
        ProjectileManager.Instance.Spawn(
            (int)ProjectileMovement.Fast,
            transform.position,
            angle,
            configure
        );
        
    }
    private void WallBounceAttack(System.Action<Projectile> configure)
    {
        // Geometry
        var direction = player.position - transform.position;
        var angle = Quaternion.Euler(0f, 0f, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);

        ProjectileManager.Instance.Spawn(
            (int)ProjectileMovement.Reflective,
            transform.position,
            angle,
            configure
        );
    }
    private void TripleAttack(System.Action<Projectile> configure)
    {
        // Geometry
        var dispersion = 10f;

        var direction = player.position - transform.position;
        var angle = Quaternion.Euler(0f, 0f, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);

        var burstSize = 3;
        for (int i = 0; i < burstSize; i++)
        {
            float firingAngle = -dispersion + (i * dispersion);
            ProjectileManager.Instance.Spawn(
                (int)ProjectileMovement.Straight,
                transform.position,
                Quaternion.Euler( angle.eulerAngles.x, angle.eulerAngles.y, angle.eulerAngles.z + transform.rotation.eulerAngles.z + firingAngle),
                configure
            );
        }
    }
    private void HomingAttack(System.Action<Projectile> configure)
    {
        // Geometry
        var direction = player.position - transform.position;
        var angle = Quaternion.Euler(0f, 0f, Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg);

        ProjectileManager.Instance.Spawn(
            (int)ProjectileMovement.Homing,
            transform.position,
            angle,
            configure
        );
    }

}
public enum AttackSpawning
{
    Forward,
    Aimed,
    Radial, 
    TwistingRadial,
    Checkerboard,
    Wall,
    Column,
    SurpriseAttack,
    WallBounce,
    TripleAttack,
    HomingAttack
}
