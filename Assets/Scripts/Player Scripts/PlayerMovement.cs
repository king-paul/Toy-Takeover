using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerSound))]
public class PlayerMovement : MonoBehaviour
{
    public Transform camera;

    [Tooltip("Sets the initial face direction of the player when the scene loads")]
    [Range(-180, 180)]
    public float startingAngle = 0;

    // Basic movement
    [Header("Basic Movement")]
    [SerializeField] [Range(0, 50)]
    float runSpeed = 20;    
    [SerializeField][Range(0, 10)]
    [Tooltip("Sets how high the player can jump")]
    float jumpPower = 7;
    [SerializeField][Range(0, 20)]
    [Tooltip("Sets how fast the player falls without using jetpack")]
    float gravity = -Physics.gravity.y;   
    
    // Camera
    [Header("Camera")]
    [SerializeField][Range(0, 10)]
    float verticalRotationSpeed = 2;
    [SerializeField][Range(0, 10)]
    float horizontalRotationSpeed = 2;
    [SerializeField][Range(0, 180)]
    float verticalRotationLimit = 90;

    // Jetpack
    [Header("Jetpack")]
    [SerializeField][Range(1, 10)]
    [Tooltip("The maxmimum fuel capacity that the jetpack can store")]
    float maxFuel = 4f;
    [SerializeField][Range(0, 10)]
    float startingFuel = 4f;
    [SerializeField][Range(0, 20)]
    [Tooltip("The upwards speed while on the jetpack")]
    float lift = 7f;
    [SerializeField][Range(0, 20)]
    [Tooltip("The forward, back and sideways movement speed while on jetpack")]
    float thrust = 7f;
    [SerializeField][Range(0, 20)]
    [Tooltip("The speed in which the character falls while there is still jetpack fuel")]
    float weight = 9.8f;

    // Private variables
    private Rigidbody rigidBody;
    private CharacterController controller;
    private PlayerController player;
    private PlayerSound audio;
    private GameManager game;
    private float moveSpeed;

    Vector3 forwardVector, sidewaysVector;
    Vector3 movementVector;
    private float rotateX, rotateY;
    private float verticalVelocity;

    private bool onGround;
    private bool usingJetpack;
    private bool landed;
    private Transform grounded;

    // Start is called before the first frame update
    void Start()
    {
        // Get the PlayerController and GameManger scripts
        player = GetComponent<PlayerController>();
        audio = GetComponent<PlayerSound>();
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
        grounded = transform.Find("Grounded");

        // Initialize Rigid body and camera variables
        rigidBody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        rotateX = 0;
        rotateY = startingAngle;

        // Initialize jetpack variables
        usingJetpack = false;
        movementVector = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (game.State == GameState.Running)
        {
            Physics.gravity = new Vector3(0, -gravity, 0);
            UpdateFaceDirection();
            UpdateHorizontalPosition();
            UpdateVerticalPosition();
        }
        
    }

    void UpdateFaceDirection()
    {
        rotateY += Input.GetAxis("Camera Horizontal") * horizontalRotationSpeed;

        if(Input.GetAxis("Camera Vertical") > 0 && rotateX > -verticalRotationLimit ||
           Input.GetAxis("Camera Vertical") < 0 && rotateX < verticalRotationLimit)
            rotateX += -Input.GetAxis("Camera Vertical") * verticalRotationSpeed;       

        // rotate player
        transform.rotation = Quaternion.Euler(0, rotateY, 0);
        // rotate camera

        camera.rotation = Quaternion.Euler(rotateX, rotateY, 0);
    }

    void UpdateHorizontalPosition()
    {
        if (usingJetpack)
            moveSpeed = thrust;
        else
            moveSpeed = runSpeed;

        forwardVector = transform.forward * Input.GetAxis("Vertical");
        sidewaysVector = transform.right * Input.GetAxis("Horizontal");
        movementVector = (forwardVector + sidewaysVector).normalized * runSpeed;

        //Debug.Log("Horizontal Input: " + Input.GetAxis("Horizontal"));
        //Debug.Log("Vertical Input: " + Input.GetAxis("Horizontal"));        

        controller.Move(movementVector * Time.deltaTime);

        // start and stop running sound effect
        if (movementVector != Vector3.zero)
            audio.PlaySound(audio.playerRunning, 1, true);
        else
            audio.StopPlaying(1);

        //Debug.Log("Movement Vector: " + movementVector);
    }

    void UpdateVerticalPosition()
    {
        if (isOnGround() || player.Fuel <= 0)
        {
            usingJetpack = false;
            //Debug.Log("Jetpack is off");

            // stop jetpack sound
            audio.StopPlaying(1);
        }

        // get jetpack input from player
        if (Input.GetButton("Jetpack") || Input.GetAxis("Jetpack") != 0)
        {
            // if there is jetpack fuel left, use the jetpack
            if (player.Fuel > 0f)
            {
                player.DrainFuel(); // reduce the fuel based on drain speed
                verticalVelocity = lift;
                usingJetpack = true;

                // play jetpack sound
                audio.PlaySound(audio.jetpackThrust, 1, true);
            }
            else if (isOnGround())// otherwise jump if on ground
            {
                verticalVelocity = jumpPower;
                audio.PlaySound(audio.playerJump);
            }
            else
            {
                audio.PlaySound(audio.jetpackRunout);
            }

            landed = false;
        }

        if(Input.GetButton("Jump") && isOnGround()) // space bar makes player jump
        {
            verticalVelocity = jumpPower;
            audio.PlaySound(audio.playerJump);

            landed = false;
        }

        // Release the jetpack when player releases the jump button
        if ((Input.GetButtonUp("Jump") || Input.GetButtonUp("Jetpack") || Input.GetAxis("Jetpack") == 0) 
            && usingJetpack)
        {
            verticalVelocity = -weight;
        }

        // Apply gravity, pulling the player down if they are above the ground
        if (!isOnGround() && !usingJetpack)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // Move the player vertically
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        //Debug.Log("Y Velocity: " + (Vector3.up * verticalVelocity * Time.deltaTime).y);
    }

    bool isOnGround()
    {
        RaycastHit hit;
        float distance = 0;

        Debug.DrawRay(grounded.position, Vector3.down);

        if (Physics.Raycast(grounded.position, Vector3.down, out hit))//, LayerMask.NameToLayer("Level")))
        {
            distance = transform.position.y - hit.point.y;

            if (distance <= 1.2) 
            {
                //Debug.Log("Player is on ground");
                return true;
            }

            //Debug.Log("Distance from ground: " + distance);
        }

        //Debug.Log("Player is off ground");
        return false;
    }

    // detect collision with ground and walls
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // play land sound effect when player lands on ground
        if (!landed)
        {
            Debug.Log("Player hit something");
            audio.PlaySound(audio.playerLanding);
            landed = true;
        }
    }

}