using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    private GameObject player;
    private GameObject text;
    private bool mounted = false;
    private bool inRange = false;

    public float maxSpeed = 0.75f;
    public float acceleration = 0.2f;
    public float deceleration;
    public float forwardVelocity;
    private float currentTurn;
    public float turnAnglePerSec = 90.0f;
    private float verticalVelocity;
    private float gravity = -20.0f;

    CharacterController cc;

    private void Awake()
    {
        forwardVelocity = 0.0f;
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
    }

    private void LateUpdate()
    {
        if (forwardVelocity > 0.0f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, currentTurn, 0.0f));
        }
        cc.Move(transform.forward * (forwardVelocity / 10.0f) + new Vector3(0.0f, verticalVelocity, 0.0f));
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
            player.transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = false;
            player.GetComponent<BoxCollider>().enabled = false;
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
        player.transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        player.transform.rotation = transform.rotation;
    }

    void Ride()
    {
        float min = 0.5f, max = 2.5f;
        float divider = max - min;
        float i = forwardVelocity / divider;
        deceleration = Mathf.Lerp(min, max, i);

        if (Input.GetKey(KeyCode.W))
        {
            forwardVelocity += acceleration * Time.fixedDeltaTime;
            forwardVelocity = Mathf.Min(forwardVelocity, maxSpeed);
        }
        else
        {
            forwardVelocity -= deceleration * Time.fixedDeltaTime;
            forwardVelocity = Mathf.Max(forwardVelocity, 0.0f);
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
}