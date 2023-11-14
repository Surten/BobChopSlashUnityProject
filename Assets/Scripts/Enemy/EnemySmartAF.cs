using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySmartAF : MonoBehaviour
{
    private float rotateSpeed;
    private float walkSpeed;
    private float runSpeed;
    private float awarenessAwareRange;
    private float awarenessChaseRange;
    private float awarenessAttackRange;
    private float groundDistance = 0.2f; //enemy's height from origin
    private float jumpRange = 1.0f;

    private float attackRadius;
    private int attackDamage;

    private EnemyState currentState = EnemyState.Idle;

    public Enemy1ScriptableObject enemyScriptableObject;
    public Transform target;
    public AttackMelee attackMelee;

    private Vector3 previousTargetPosition;

    private Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundLayerMask;

    public bool isCharging;
    public bool isIdleJumping;
    private bool isGrounded;

    public bool canExplode;
    private ParticleSystem explosionEffect;


    public enum EnemyState{ Idle, Jumping, Rotating, Walking, Charging, Attack, Explode, Dead }

    /* Initialization and Updates per Frame */
    private void Start()
    {
        rotateSpeed = enemyScriptableObject.rotateSpeed;
        walkSpeed = enemyScriptableObject.walkSpeed;
        runSpeed = enemyScriptableObject.runSpeed;

        awarenessAwareRange = enemyScriptableObject.awarenessAwareRange;
        awarenessChaseRange = enemyScriptableObject.awarenessChaseRange;
        awarenessAttackRange = enemyScriptableObject.awarenessAttackRange;

        attackMelee.attackRadius = attackRadius = enemyScriptableObject.attackRadius;
        attackMelee.attackDamage = attackDamage = enemyScriptableObject.attackDamage;

        isCharging = enemyScriptableObject.chargeProbability > UnityEngine.Random.value;
        canExplode = (enemyScriptableObject.kamikazeProbability > UnityEngine.Random.value) && isCharging;
        if (canExplode) attackMelee.attackDamage *= 3;
        isIdleJumping = (enemyScriptableObject.idleJumpProbability) > UnityEngine.Random.value;

        previousTargetPosition = target.position;

        rb = GetComponent<Rigidbody>();
        explosionEffect = GetComponent<ParticleSystem>();
    }

    private void Update()
    {

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Jumping:
                Jump();
                break;

            case EnemyState.Rotating:
                RotateToTarget();
                break;

            case EnemyState.Walking:
                RotateToTarget();
                Transition2Position(walkSpeed);
                break;

            case EnemyState.Charging:
                renderTextureColor();
                if (canExplode) RotateToTarget(25f);
                else RotateToTarget(-25f);
                Transition2Position(runSpeed);
                break;

            case EnemyState.Attack:
                RotateToTarget();
                attackMelee.SwingSword();
                break;

            case EnemyState.Explode:
                RotateToTarget();
                Explode();
                break;

        }

    }

    /* Status Functions */

    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetEnemyState(EnemyState state)
    {
        currentState = state;
    }
    public EnemyState GetEnemyState()
    {
        return currentState;
    }

    public void renderTextureColor() {
        // Ensure the object has a renderer component
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && isCharging)
        {
            // Create a new material instance to avoid modifying the shared material
            Material material = renderer.material;
            Material newMaterial = new Material(material);

            if (canExplode)
            {
                // Set the new color
                newMaterial.color = Color.red;
            }
            else
            {
                // Set the new color
                newMaterial.color = Color.blue;
            }

            // Assign the new material to the renderer
            renderer.material = newMaterial;
        }
    }

    private void Explode()
    {
        attackMelee.SwingSword();
        if (GetComponent<ParticleSystem>() != null)
        {
            explosionEffect.Play();

        }
        currentState = EnemyState.Dead;
    }


    /* Action Functions */

    private void RotateToTarget(float inclinationAngle = 0)
    {
        Vector3 v = target.position - transform.position;
        v.y = 0;
        var q = Quaternion.LookRotation(v);

        // Introduce a small inclination towards the target
        Quaternion inclination = Quaternion.AngleAxis(inclinationAngle, -transform.right);

        // Combine the original rotation with the inclination
        q *= inclination;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
        if (isGrounded)
        {

            float jumpVelocity = _calculateJumpVelocity(jumpRange, 90f); //Adquire the initial jump velocity according to the provided height
            
            // Set the rigidbody's velocity directly
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    private void Transition2Position(float speed)
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/

    private float _calculateJumpVelocity(float jumpHeight, float angle = 0f)
    {
        // Calculate the Straight Up Jump Velocity: v_0 = sqrt(2 * g * h / sin^2(angle))
        float sinSquared = _calculateSinSquared(angle);
        if (sinSquared < 1e-3) return 0f;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float jumpVelocity = Mathf.Sqrt(2f * gravity * jumpHeight / sinSquared);
        return jumpVelocity;
    }

    private float _calculateSinSquared(float angleInDegrees)
    {
        // Convert angle to radians since Mathf.Sin uses radians
        float angleInRadians = Mathf.Deg2Rad * angleInDegrees;

        // Calculate sin^2(theta)
        return _squared(Mathf.Sin(angleInRadians));
    }

    private float _squared(float value) {
        return value * value;
    }
}
