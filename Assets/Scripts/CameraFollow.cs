using UnityEngine;
using UnityEngine.InputSystem;

// Smoothly follows the player and zooms without changing camera angle.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Framing")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f);
    [SerializeField] private float followSharpness = 5f;

    [Header("Zoom")]
    [SerializeField] private float minZoom = 6f;
    [SerializeField] private float maxZoom = 18f;
    [SerializeField] private float zoomSpeed = 0.02f;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.Find("CubePlayer");
            if (player != null)
                target = player.transform;
        }

        if (offset.sqrMagnitude < 0.001f)
            offset = new Vector3(0f, 12f, -8f);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Mouse.current is the new Input System replacement for the old axis.
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            float scroll = mouse.scroll.ReadValue().y;
            float distance = Mathf.Clamp(offset.magnitude - scroll * zoomSpeed,
                minZoom, maxZoom);
            offset = offset.normalized * distance;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position, desiredPosition, followSharpness * Time.deltaTime);
        transform.LookAt(target);
    }
}
