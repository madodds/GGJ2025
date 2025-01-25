using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Transform position1; // The first position
    public Transform position2; // The second position
    public float moveSpeed = 2.0f; // Speed of the camera transition
    public KeyCode toggleKey = KeyCode.Space; // Key to toggle positions

    private Transform targetPosition; // The target position the camera is moving toward
    private bool isMoving = false; // To track if the camera is in the process of moving

    void Start()
    {
        // Initialize the target position to the first position
        targetPosition = position1;
        // Set the camera's initial position
        transform.position = position1.position;
    }

    void Update()
    {
        // Check if the toggle key is pressed
        if (Input.GetKeyDown(toggleKey) && !isMoving)
        {
            // Toggle the target position
            targetPosition = (targetPosition == position1) ? position2 : position1;

            // Start moving
            StartCoroutine(MoveCamera());
        }
    }

    private System.Collections.IEnumerator MoveCamera()
    {
        isMoving = true; // Mark that the camera is moving
        float elapsedTime = 0f; // Time elapsed during the transition
        Vector3 startingPosition = transform.position; // The camera's current position

        // Smoothly move the camera over time
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startingPosition, targetPosition.position, elapsedTime);
            yield return null; // Wait for the next frame
        }

        // Snap to the final target position
        transform.position = targetPosition.position;
        isMoving = false; // Mark that the camera has stopped moving
    }
}
