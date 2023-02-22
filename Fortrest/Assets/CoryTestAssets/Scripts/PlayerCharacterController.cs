using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    // Player Variables
    [Header("Player Variables")]
    public float playerCurrSpeed = 5f;
    private float playerGrav = -9.81f;
    public float playerGravMultiplier = 3f;
    public float playerJumpHeight = 10f;
    CharacterController playerCC;

    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;

    // Variable for movement direction
    private Vector3 moveDirection;
    private float playerVelocity;

    private void Awake()
    {
        // Add character controller to the game object 
        gameObject.AddComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get Character controller that is attached to the player
        playerCC = GetComponent<CharacterController>();
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
        }
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

        if (Input.GetKey(KeyCode.Space))
        {
            playerVelocity += playerJumpHeight;
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
}