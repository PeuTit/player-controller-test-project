using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 6f;
    [SerializeField]
    private float turnSmoothTime = 0.1f;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float jumpHeight = 3f;
    [SerializeField]
    private float sprintSpeed = 8f;

    private float turnSmoothVelocity;

    private CharacterController controller;
    private InputManager inputManager;
    private Transform cameraTransform;
    private Vector3 playerVelocity;

    private bool isPlayerGrounded;
    private bool isPlayerSprinting = false;
    private bool isPlayerCrouching = false;
    private float crouchTimer = 0f;
    private bool lerpCrouch = false;

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
        Vector3 move = new Vector3(movement.x, 0f, movement.y).normalized;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            move.y = 0f;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }

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

            if (isPlayerCrouching)
                controller.height = Mathf.Lerp(controller.height, 0.9f, p);
            else
                controller.height = Mathf.Lerp(controller.height, 1.8f, p);

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

    public void Sprint()
    {
        isPlayerSprinting = !isPlayerSprinting;

        if (isPlayerSprinting)
            speed = sprintSpeed;
        else
            speed = 6f;
    }

    public void Crouch()
    {
        isPlayerCrouching = !isPlayerCrouching;
        crouchTimer = 0f;
        lerpCrouch = true;
    }
}
