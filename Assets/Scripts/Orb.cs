using UnityEngine;
using UnityEngine.InputSystem;

// Animates, recolours, and lets the player drag an orb.
[RequireComponent(typeof(Collider), typeof(Renderer))]
public class Orb : MonoBehaviour
{
    [Header("Click palette")]
    [SerializeField] private Color[] clickColors =
        { Color.cyan, Color.magenta, Color.yellow };

    [Header("Bob and spin")]
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private float bobSpeed = 3f;

    private Vector3 basePosition;
    private Vector3 dragOffset;
    private int clickIndex = -1;
    private bool isDragging;
    private Renderer orbRenderer;

    private void Awake()
    {
        basePosition = transform.position;
        orbRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        // Keep bobbing around the stored position so dragging still works.
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = basePosition + Vector3.up * bob;
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        Mouse mouse = Mouse.current;
        Camera mainCamera = Camera.main;
        if (mouse == null || mainCamera == null)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
            TryStartDrag(mouse, mainCamera);

        if (isDragging && mouse.leftButton.isPressed)
            DragToMouse(mouse, mainCamera);

        if (mouse.leftButton.wasReleasedThisFrame)
            isDragging = false;
    }

    private void TryStartDrag(Mouse mouse, Camera mainCamera)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit) ||
            (hit.transform != transform && !hit.transform.IsChildOf(transform)))
            return;

        // Raycast clicks work with the new Input System without mouse callbacks.
        if (clickColors.Length > 0)
        {
            clickIndex = (clickIndex + 1) % clickColors.Length;
            orbRenderer.material.color = clickColors[clickIndex];
            Debug.Log("[Orb] Clicked: " + clickColors[clickIndex], this);
        }

        isDragging = true;
        dragOffset = basePosition - GetFloorPoint(ray);
    }

    private void DragToMouse(Mouse mouse, Camera mainCamera)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        basePosition = GetFloorPoint(ray) + dragOffset;
    }

    private Vector3 GetFloorPoint(Ray ray)
    {
        Plane floorPlane = new Plane(Vector3.up, basePosition);
        if (floorPlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return basePosition;
    }
}
