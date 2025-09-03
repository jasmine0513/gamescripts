using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class FPControllerInputSystem : MonoBehaviour
{
    [Header("Move")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Look")]
    public Transform cameraRoot;
    public float lookSensitivity = 0.1f; // scales delta directly
    public float pitchClamp = 85f;

    CharacterController cc;
    Vector2 moveInput;
    Vector2 lookInput;
    bool sprintHeld;
    bool jumpPressed;
    float pitch;
    Vector3 velocity;

    void Awake() => cc = GetComponent<CharacterController>();

    // PlayerInput -> Unity Events: hook these
    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    public void OnLook(InputValue v) => lookInput = v.Get<Vector2>();
    public void OnSprint(InputValue v) => sprintHeld = v.isPressed;
    public void OnJump(InputValue v) => jumpPressed = v.isPressed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Look
        pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, -pitchClamp, pitchClamp);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);

        // Move
        Vector3 input = Vector3.ClampMagnitude(new Vector3(moveInput.x, 0f, moveInput.y), 1f);
        Vector3 world = transform.TransformDirection(input);
        float speed = sprintHeld ? sprintSpeed : walkSpeed;

        if (cc.isGrounded)
        {
            velocity.y = -0.5f;
            if (jumpPressed)
            {
                velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
                jumpPressed = false;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        Vector3 motion = world * speed + Vector3.up * velocity.y;
        cc.Move(motion * Time.deltaTime);
    }
}
