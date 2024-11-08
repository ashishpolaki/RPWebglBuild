using UnityEngine;

public class RotateOnClick : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation

    private void OnMouseDrag()
    {
        float mouseX = Input.GetAxis("Mouse X"); // Get horizontal mouse movement
        RotateObject(mouseX);
    }

    private void RotateObject(float mouseX)
    {
        // Rotate the object around the y-axis based on mouse movement
        transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
    }
}
