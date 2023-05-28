using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    private GameObject player;
    private GameObject text;
    private bool mounted = false;
    private bool inRange = false;

    private float maxSpeed = 0.75f;
    public float acceleration = 0.2f;
    public float deceleration = 0.0f;
    public float currentSpeed;
    private float currentTurn;
    public float turnAnglePerSec = 90.0f;
    private float verticalVelocity;
    private float gravity = -20.0f;

    CharacterController cc;

    private void Awake()
    {
        currentSpeed = 0.0f;
        currentTurn = 0.0f;
        verticalVelocity = 0.0f;
    }

    void Start()
    {
        text = transform.GetChild(0).gameObject;
        player = PlayerController.global.gameObject;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        DisplayText();
        if (Input.GetKeyDown(KeyCode.M) && inRange)
        {
            Mount();
        }
        if (mounted)
        {
            PlayerStick();
        }   
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        if (mounted)
        {
            Ride();
        }
        else
        {
            if (currentSpeed >= 0)
            {
                Lerping(0.5f, 2.0f, ref deceleration, 2);
                currentSpeed -= deceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0.0f);
            }
        }
    }

    private void LateUpdate()
    {
        if (currentSpeed > 0.0f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, currentTurn, 0.0f));
        }
        cc.Move(transform.forward * (currentSpeed / 10.0f) + new Vector3(0.0f, verticalVelocity, 0.0f));
        currentTurn = 0.0f;
    }

    private void DisplayText()
    {
        if (inRange)
        {
            if (mounted)
            {
                text.SetActive(false);
            }
            else
            {
                text.SetActive(true);
            }
        }
        else
        {
            text.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            inRange = false;
        }
    }

    void Mount()
    {      
        mounted = !mounted;

        player.GetComponent<CharacterController>().enabled = false;       
        if (mounted)
        {           
            player.transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = false;
            player.GetComponent<BoxCollider>().enabled = false;
            player.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Moving", false);
        }
        else
        {            
            player.transform.position += transform.right;
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = true;
            player.GetComponent<BoxCollider>().enabled = true;
        }
        player.GetComponent<CharacterController>().enabled = true;        
    }

    void PlayerStick()
    {
        player.transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
        player.transform.rotation = transform.rotation;
    }

    void Ride()
    {
        Lerping(0.2f, 0.4f, ref acceleration, 0.2667f); // Acceleration
        Lerping(0.5f, 2.0f, ref deceleration, 2); // Deceleration
        Lerping(75.0f, 90.0f, ref turnAnglePerSec, 20); // Turn

        if (Input.GetKey(KeyCode.W))
        {          
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {          
            currentSpeed -= deceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0.0f);
        }
        
        if (Input.GetKey(KeyCode.A))
        {          
            currentTurn = -turnAnglePerSec * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentTurn = turnAnglePerSec * Time.fixedDeltaTime;
        }
    }

    private void ApplyGravity()
    {
        if (cc.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -1.0f;
        }
        else
        {
            verticalVelocity += gravity * Time.fixedDeltaTime;
        }      
    }

    void Lerping(float min, float max, ref float value, float dividerCoefficient)
    {
        float divider = max - min;
        float i = currentSpeed / (divider / dividerCoefficient);
        value = Mathf.Lerp(min, max, i);       
    }
}