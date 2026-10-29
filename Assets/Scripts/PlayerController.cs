using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Reads keyboard input and moves the player using its Rigidbody.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Lab wiring")]
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 12f, -8f);

    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector3 lastPosition;
    private static bool isQuitting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = rb.position;
        isQuitting = false;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Read input in Update because keyboard button states can change each frame.
        movementInput = Vector2.zero;
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) movementInput.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) movementInput.y -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) movementInput.x += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) movementInput.x -= 1f;
        movementInput = movementInput.normalized;
    }

    private void FixedUpdate()
    {
        // MovePosition lets the player's collider be blocked by walls.
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore contacts pointing up because they are from the floor.
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y <= 0.5f)
            {
                Debug.Log("[OnCollisionEnter] Bumped into " + collision.gameObject.name, this);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Orb"))
            return;

        Debug.Log("[OnTriggerEnter] Collected " + other.gameObject.name, this);
        Destroy(other.gameObject);

        if (GameManager.Instance != null)
            GameManager.Instance.Collect();
    }

    private void LateUpdate()
    {
        lastPosition = rb.position;

        // CameraFollow handles the camera when it is attached.
        if (cameraTransform == null || cameraTransform.GetComponent<CameraFollow>() != null)
            return;

        cameraTransform.position = transform.position + cameraOffset;
        cameraTransform.LookAt(transform.position);
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        // Do not spawn a bonus while quitting or unloading the scene.
        if (!Application.isPlaying || isQuitting || orbPrefab == null ||
            !gameObject.scene.IsValid() || !gameObject.scene.isLoaded ||
            SceneManager.GetActiveScene() != gameObject.scene)
            return;

        Instantiate(orbPrefab, lastPosition, Quaternion.identity);
    }
}
