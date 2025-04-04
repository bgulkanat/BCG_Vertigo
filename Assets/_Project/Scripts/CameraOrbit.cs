using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float rotationSpeed = 5.0f; // Adjust rotation speed
    public float smoothFactor = 0.1f; // Adjust smoothing factor

    private float targetRotation = 0.0f;
    private float currentRotation = 0.0f;
    private Vector2 startPosition;
    private bool isDragging = false;

    void Update()
    {
        // Mobile touch control
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float horizontalSwipe = (touch.position.x - startPosition.x) * rotationSpeed * Time.deltaTime;
                targetRotation += horizontalSwipe;
                startPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }
        // Mouse control in editor (optional)
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = Input.mousePosition;
                isDragging = true;
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                float horizontalSwipe = (Input.mousePosition.x - startPosition.x) * rotationSpeed * Time.deltaTime;
                targetRotation += horizontalSwipe;
                startPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        // Smooth rotation using Lerp
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, smoothFactor);

        // Rotate the parent object around the Y-axis
        transform.eulerAngles = new Vector3(0, currentRotation, 0);
    }
}