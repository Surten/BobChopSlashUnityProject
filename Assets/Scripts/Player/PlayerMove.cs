using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public Animator anim;
    public PlayerScriptableObject playerInfo;

    public Transform groundCheck;
    public LayerMask groundLayerMask;

    public float groundDistance = 0.2f; //player height from origin
    private float moveSpeed;
    private float sprintMultiplier = 2f;
    private float resetYHeightTreshold = -200f;

    bool isGrounded;
    Rigidbody rb;
    AttackMelee playerAttack;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAttack = GetComponent<AttackMelee>();

        moveSpeed = playerInfo.moveSpeed;
        playerAttack.attackDamage = playerInfo.attackDamage;
        
        ShopManager.Instance.onItemPickUpEvent += OnItemPickUp;
    }

    void Update()
    {
        Grounded();
        Jump();
        Move();
        if (Input.GetKeyDown(KeyCode.Mouse0) && isGrounded)
        {
            playerAttack.SwingSword();
        }
    }

    void Grounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !playerAttack.swingingSwordCR)
        {
            rb.AddForce(Vector3.up * 4f, ForceMode.Impulse);
            anim.SetTrigger("jump");
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        
        Vector3 move = transform.right * x + transform.forward * y;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move *= sprintMultiplier;
        }
        else
        {
            x *= 0.5f;
            y *= 0.5f;
        }
        anim.SetFloat("horizontal", x);
        anim.SetFloat("vertical", y);

        move.Normalize();

        transform.position += move * Time.deltaTime * moveSpeed;
        if(transform.position.y < resetYHeightTreshold)
        {
            transform.position += Vector3.up * (resetYHeightTreshold + 20f);
        }
    }


    private void OnItemPickUp(Item item)
    {
        moveSpeed += item.bonusMovementSpeed;
        playerAttack.attackDamage += item.bonusAttackDamage;
        playerAttack.UpdateAttackSpeed(item.bonusAttackSpeedPercentage);
        playerAttack.UpdateAttackRadius(item.bonusAttackRadiusPercentage);
    }

    public void OnPlayerDeath()
    {
        this.enabled = false;
    }
}
