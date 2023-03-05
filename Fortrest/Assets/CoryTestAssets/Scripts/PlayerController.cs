using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController global;
    // Player Variables
    [Header("Player Variables")]
    public float playerCurrSpeed = 5f;
    public float playerMaxSpeed = 5f;
    public float playerSlowedSpeed = 5f;
    public float playerGravMultiplier = 3f;
    public float playerJumpHeight = 10f;
    public float playerEnergy = 100f;
    public float maxPlayerEnergy = 100f;

    public Image playerEnergyBarImage;

    private float playerGrav = -9.81f;
    private float playerVelocity;

    CharacterController playerCC;

    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;
    public bool playerisAttacking = false;

    public List<Transform> enemyList = new List<Transform>();

    // Variable for movement direction
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Awake()
    {
        // Get Character controller that is attached to the player
        playerCC = GetComponent<CharacterController>();
        global = this;
    }

    private void Start()
    {
        playerEnergy = maxPlayerEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        // Only take input if movement isn't inhibited
        if (playerCanMove)
        {
            // Local veriables for input keys
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            Jump();
            ApplyGravity();
            ApplyMovement(horizontalMovement, verticalMovement);
            Eat();
            Attack();
        }

        if (playerEnergy >= maxPlayerEnergy)
        {
            playerEnergy = maxPlayerEnergy;
        }
        else if (playerEnergy > maxPlayerEnergy / 2f)
        {
            playerEnergyBarImage.color = Color.green;
        }
        else if (playerEnergy <= maxPlayerEnergy / 2f && playerEnergy > maxPlayerEnergy / 4f)
        {
            playerEnergyBarImage.color = Color.yellow;
        }
        else if (playerEnergy <= maxPlayerEnergy / 4f && playerEnergy != 0f)
        {
            playerEnergyBarImage.color = Color.red;
        }
        else
        {
            playerCurrSpeed = playerSlowedSpeed;
        }

        playerEnergyBarImage.fillAmount = Mathf.Clamp(playerEnergy / maxPlayerEnergy, 0, 1f);
    }

    // Player movement 
    private void ApplyMovement(float _horizontalMove, float _verticalMove)
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            playerisMoving = true;

            moveDirection = new Vector3(_horizontalMove, 0.0f, _verticalMove);

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            moveDirection *= playerCurrSpeed;

            moveDirection = Quaternion.AngleAxis(45, Vector3.up) * moveDirection;

            if (moveDirection != Vector3.zero)
            {
                transform.forward = moveDirection;
            }

            ApplyGravity();
        }
        else
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
        }

        playerCC.Move(moveDirection * Time.deltaTime);
    }

    private void Jump()
    {
        if (!playerCC.isGrounded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerVelocity += playerJumpHeight;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerJumpSound);
        }
    }

    private void ApplyGravity()
    {
        // Run gravity
        if (playerCC.isGrounded && playerVelocity < 0.0f)
        {
            playerVelocity = -1.0f;
        }
        else
        {
            playerVelocity += playerGrav * playerGravMultiplier * Time.deltaTime;
        }

        moveDirection.y = playerVelocity;
    }

    public void ApplyEnergyDamage(float amount)
    {
        playerEnergy -= amount;
    }

    public void ApplyEnergyRestore(float amount)
    {
        playerEnergy += amount;
    }

    private void Eat()
    {
        InventoryManager inventory = GameObject.Find("Level Manager").GetComponent<InventoryManager>();

        if (Input.GetKeyDown(KeyCode.E) && inventory.food > 0 && playerEnergy < maxPlayerEnergy)
        {
            ApplyEnergyRestore(5f);
            inventory.MinusFood(1);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttackSound);
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.SwordSwing1Sound : GameManager.global.SwordSwing2Sound);
            for (int i = 0; i < enemyList.Count; i++) // Goes through the list of targets
            {                
                if (Vector3.Distance(transform.position, enemyList[i].transform.position) <= 3.0f && FacingEnemy(enemyList[i].transform.position)) // Distance from player to enemy
                {                   
                    playerisAttacking = true;
                    enemyList[i].GetComponent<EnemyController>().chasing = true;
                    break;
                }
            }
        }
    }

    private bool FacingEnemy(Vector3 enemyPosition) // Making sure the enemy always faces what it is attacking
    {
        Vector3 enemyDirection = (enemyPosition - transform.position).normalized; // Gets a direction using a normalized vector
        float angle = Vector3.Angle(transform.forward, enemyDirection);
        if (angle > -75.0f && angle < 75.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}