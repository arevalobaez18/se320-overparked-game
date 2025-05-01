using UnityEngine;

// Stole this from an older project of mine -Jesse

public class ParallaxMouseCam : MonoBehaviour
{
    [Header("Parallax Rotation Settings")]
    [Tooltip("How much the camera rotates toward the mouse")]
    public float rotationStrength = 5f;

    [Tooltip("How smooth the rotation is")]
    public float smoothSpeed = 5f;

    [Tooltip("Maximum rotation angles in degrees (X = pitch, Y = yaw)")]
    public Vector2 maxRotation = new Vector2(5f, 5f);

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Get normalized position of mouse in viewport (-1 to 1)
        Vector2 mouseViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 offset = (mouseViewport - new Vector2(0.5f, 0.5f)) * 2f;

        // Calculate rotation around X (pitch) and Y (yaw)
        float rotationX = Mathf.Clamp(-offset.y * rotationStrength, -maxRotation.x, maxRotation.x);
        float rotationY = Mathf.Clamp(offset.x * rotationStrength, -maxRotation.y, maxRotation.y);

        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0f) * initialRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}