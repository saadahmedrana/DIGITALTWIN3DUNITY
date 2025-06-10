using UnityEngine;
using UnityEngine.InputSystem; // ← required for Keyboard.current & Mouse.current

[RequireComponent(typeof(Camera))]
public class NewInputFlyCam : MonoBehaviour
{
    [Header("Speeds")]
    public float moveSpeed = 8f;
    public float sprintMultiplier = 2f;
    public float dampingCoefficient = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.12f;
    public float clampAngle = 85f;

    private Vector3 velocity;
    private float rotX = 0f, rotY = 0f;
    private bool cursorLocked = true;

    private void Start()
    {
        // Initialize rotation
        Vector3 euler = transform.localRotation.eulerAngles;
        rotY = euler.y;
        rotX = euler.x;

        // Lock cursor
        SetCursorLock(true);
    }

    private void Update()
    {
        // Toggle cursor lock on Escape
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLock(false);

        if (!cursorLocked)
        {
            // Click LMB to re‐lock
            if (Mouse.current.leftButton.wasPressedThisFrame)
                SetCursorLock(true);
            return;
        }

        // 1) Mouse look
        Vector2 md = Mouse.current.delta.ReadValue(); // pixel change this frame
        rotY += md.x * mouseSensitivity;
        rotX -= md.y * mouseSensitivity;
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        transform.rotation = Quaternion.Euler(rotX, rotY, 0f);

        // 2) Movement input
        Vector3 inputDir = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) inputDir += Vector3.forward;
        if (Keyboard.current.sKey.isPressed) inputDir += Vector3.back;
        if (Keyboard.current.aKey.isPressed) inputDir += Vector3.left;
        if (Keyboard.current.dKey.isPressed) inputDir += Vector3.right;
        if (Keyboard.current.spaceKey.isPressed) inputDir += Vector3.up;
        if (Keyboard.current.leftCtrlKey.isPressed) inputDir += Vector3.down;

        // Apply damping if no key pressed
        if (inputDir == Vector3.zero)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
        }
        else
        {
            Vector3 worldDir = transform.TransformDirection(inputDir.normalized);
            float curSpeed = moveSpeed;
            if (Keyboard.current.leftShiftKey.isPressed)
                curSpeed *= sprintMultiplier;

            velocity += worldDir * curSpeed * Time.deltaTime;
        }

        // 3) Move camera
        transform.position += velocity * Time.deltaTime;
    }

    private void SetCursorLock(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
