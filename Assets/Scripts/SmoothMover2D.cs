using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SmoothMover2D : MonoBehaviour
{
    [Header("Movement Points")]
    public Vector3 originPoint; // The starting point
    public Vector3 frontOfDeskPoint; // The first point
    public Vector3 leavingDeskPoint; // The second point

    [Header("Movement Settings")]
    public float moveSpeed = 2f; // Speed of movement
    public float bounceHeight = 0.2f; // Height of the bounce (sine wave)
    public float bounceFrequency = 2f; // Frequency of the sine wave

    private bool isMoving = false; // To prevent overlapping movements
    private int movementProgress = 0;
    private float initialWorldZ; // To store the original Z position

    void Start()
    {
        // Store the initial Z-axis value
        initialWorldZ = 8;
    }

    // Public method to trigger movement
    public void StartMovement()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveToPoints());
        }
    }

    private IEnumerator MoveToPoints()
    {
        isMoving = true;
        if (movementProgress == 0)
        {
            // Move to point1
            yield return StartCoroutine(MoveToPoint(frontOfDeskPoint));
            movementProgress++;
            isMoving = false;
        }
        else if (movementProgress == 1) 
        {
            // Move to point2
            yield return StartCoroutine(MoveToPoint(leavingDeskPoint));
            movementProgress++;
            isMoving = false;
        }
        else
        {
            // Move to origin point
            transform.localPosition = originPoint;
            movementProgress = 0;
            isMoving = false;
        }
    }

    private IEnumerator MoveToPoint(Vector3 targetPoint)
    {
        // Convert the 2D target point to a 3D point in world space
        Vector3 startPoint = transform.localPosition; // Start position in world space
        Vector3 endPoint = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z); // Target position in world space
        float distance = Vector2.Distance(new Vector2(startPoint.x, startPoint.y), targetPoint);
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            // Calculate the percentage of the journey completed
            elapsedTime += Time.deltaTime * moveSpeed / distance;
            float t = Mathf.Clamp01(elapsedTime);

            // Smooth interpolation for X and Y values
            Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, Mathf.SmoothStep(0, 1, t));

            // Add sine wave bounce effect (only affect the Y axis)
            newPosition.y += Mathf.Sin(t * Mathf.PI * bounceFrequency) * bounceHeight;

            // Ensure Z remains constant in world space
            //newPosition.z = initialWorldZ;

            // Update position in world space
            transform.localPosition = newPosition;

            yield return null; // Wait for the next frame
        }

        // Snap to the final target position
        transform.localPosition = endPoint;
    }
}
