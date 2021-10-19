using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float rotationSpeed = 2;
    [SerializeField][Range(0, 10)]
    [Tooltip("Sets how high the player can jump")]
    float jumpPower = 7;
    [SerializeField][Range(0, 20)]
    [Tooltip("Sets how fast the player falls without using jetpack")]
    float gravity = -Physics.gravity.y;    

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

    public RectTransform fuelBar;

    // Private variables
    private Rigidbody rigidBody;
    private CharacterController controller;
    private float moveSpeed;

    Vector3 forwardVector, sidewaysVector;
    private float rotateX, rotateY;
    private float verticalVelocity;
    private bool grounded;

    private bool usingJetpack;
    private float curFuel;
    private float barWidth;

    float MAX_WIDTH;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Rigid body and camera variables
        rigidBody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        rotateX = 0;
        rotateY = startingAngle;

        // Initialize jetpack variables
        curFuel = startingFuel;
        usingJetpack = false;

        // Initialize GUI
        MAX_WIDTH = fuelBar.rect.width;
        barWidth = MAX_WIDTH / maxFuel * curFuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.gravity = new Vector3(0, -gravity, 0);
        UpdateFaceDirection();
        UpdateHorizontalPosition();
        UpdateVerticalPosition();
        UpdateGUI();
    }

    void UpdateFaceDirection()
    {
        rotateY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotateX += -Input.GetAxis("Mouse Y") * rotationSpeed;

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

        forwardVector = transform.forward * Input.GetAxis("Vertical") * moveSpeed;
        sidewaysVector = transform.right * Input.GetAxis("Horizontal") * moveSpeed;

        // move player forward and back
        if (Input.GetAxis("Vertical") != 0)
        {
            controller.Move(forwardVector * Time.deltaTime);
        }

        // move player sideways
        if (Input.GetAxis("Horizontal") != 0)
        {
            controller.Move(sidewaysVector * Time.deltaTime);
        }

    }

    void UpdateVerticalPosition()
    {
        if (controller.isGrounded || curFuel <= 0)
        {
            usingJetpack = false;
            //Debug.Log("Jetpack is off");
        }

        // get input from player
        if (Input.GetButton("Jump") || Input.GetMouseButton(1))
        {
            // if there is jetpack fuel left, use the jetpack
            if (curFuel > 0f)
            {
                curFuel -= Time.deltaTime; // reduce the fuel based on the deltatime
                verticalVelocity = lift;
                usingJetpack = true;
            }
            else if (controller.isGrounded)// otherwise jump if on ground
            {
                verticalVelocity = jumpPower;
            }
        }

        // Release the jetpack when player releases the hump button
        if((Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(1)) && usingJetpack)
        {
            verticalVelocity = -weight;
        }

        // Apply gravity, pulling the player down if they are above the ground
        if (!controller.isGrounded && !usingJetpack)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // Move the player vertically
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        //Debug.Log("Y Velocity: " + (Vector3.up * verticalVelocity * Time.deltaTime).y);
    }

    void UpdateGUI()
    {
        // update the jetpack fuel bar
        barWidth = MAX_WIDTH / maxFuel * curFuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        /*
        if(hit.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Collision with ground");
        }*/

    }

}