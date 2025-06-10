using UnityEngine;
using UnityEngine.InputSystem;  // ← required for Mouse.current

/// <summary>
/// Attach this to your parent (TextMeshPro 3D) GameObject.
/// Drag the child cube’s Collider into “clickCollider” in the Inspector.
/// On each frame, we check Mouse.current.leftButton.wasPressedThisFrame,
/// then raycast—if it hits that exact child collider, we call OpenURL(url).
/// </summary>
public class OpenURLViaChildCollider_InputSystem : MonoBehaviour
{
    [Tooltip("Drag the child Cube's Collider here.")]
    public Collider clickCollider;

    [Tooltip("Enter the full URL you want to open when the collider is clicked.")]
    public string url = "https://www.example.com";

    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
        if (_mainCam == null)
            Debug.LogWarning($"[{nameof(OpenURLViaChildCollider_InputSystem)}] No MainCamera tagged in scene.");

        if (clickCollider == null)
            Debug.LogError($"[{nameof(OpenURLViaChildCollider_InputSystem)}] No clickCollider assigned on '{name}'.");
    }

    private void Update()
    {
        // 1) Ensure we have a main camera and a valid collider to test against:
        if (_mainCam == null || clickCollider == null)
            return;

        // 2) Check Input System's Mouse API (not Input.GetMouseButtonDown):
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 3) Raycast from the camera through the current mouse position:
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = _mainCam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                // 4) If we hit exactly the child collider, open the URL:
                if (hitInfo.collider == clickCollider)
                {
                    OpenURL();
                }
            }
        }
    }

    private void OpenURL()
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning($"[{nameof(OpenURLViaChildCollider_InputSystem)}] URL is empty on '{name}'.");
            return;
        }
        Application.OpenURL(url);
    }
}
