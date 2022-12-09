using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 6f; 
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float jumpHeight = 3f;
    [SerializeField]
    private float sprintSpeed = 8f;

    private CharacterController controller;
    private InputManager inputManager;
    private Transform cameraTransform;
    private Vector3 playerVelocity;
    private bool isPlayerGrounded;
    private bool lerpCrouch = false;
    private bool isCrouching = false;
    private float crouchTimer = 0f;
    private bool isSprinting = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerGrounded = controller.isGrounded;

        if (isPlayerGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerFootMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * speed * Time.deltaTime);

        if (inputManager.GetPlayerFootJump() && isPlayerGrounded)
        {
            Jump();
        }
        playerVelocity.y += gravity * Time.deltaTime; 
        controller.Move(playerVelocity * Time.deltaTime);

        if (inputManager.GetPlayerFootSprint()) 
        {
            Sprint();
        }

        if (inputManager.GetPlayerFootCrouch())
        {
            Crouch();
        }

        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1f;
            p *= p;

            if (isCrouching)
                controller.height = Mathf.Lerp(controller.height, 1f, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2f, p);

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    public void Jump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    } 

    public void Crouch()
    {
        isCrouching = !isCrouching;
        crouchTimer = 0f;
        lerpCrouch = true;
    }

    public void Sprint()
    {
        isSprinting = !isSprinting;
        if(isSprinting)
            speed = sprintSpeed;
        else
            speed = 6f;
    }
}
