using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.SceneView;

public class ScreenProtectorScript : MonoBehaviour
{
    public GameObject bubblePrefab;
    public GameLogicManager gameLogicManager;
    public CameraMover cameraMover;
    private BoxCollider2D screenCollider;
    private List<GameObject> bubbles;
    private int bubbleCount;
    private float minBubbleSize = 0.25f;
    private float maxBubbleSize = 0.35f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);
        screenCollider = transform.GetComponent<BoxCollider2D>();
        // SpawnBubbles();
    }

    // Update is called once per frame
    void Update()
    {
        if (bubbles != null)
        {
            foreach(GameObject bubble in bubbles)
            {
                BubbleScript bubbleScript = bubble.GetComponent<BubbleScript>();
                if (bubbleScript.active && bubbleScript.OutsideRect())
                {
                    cameraMover.ResetCamera();
                    gameLogicManager.NextCustomer();
                    StartCoroutine(WaitBeforeAction());
                    bubbleScript.active = false;
                }
            }
        }
    }

    IEnumerator WaitBeforeAction()
    {
        Debug.Log("Starting wait...");
        yield return new WaitForSeconds(2f); // Wait for the specified time
        Debug.Log("Wait complete! Continuing...");

        gameLogicManager.DeleteCustomer();
        gameLogicManager.NextCustomer();
        // Perform the next action here
    }
    public void SpawnBubbles()
    {
        // Destroy current bubbles if they exist
        if (bubbles != null)
        {
            foreach (GameObject bubble in bubbles)
            {
                Destroy(bubble);
            }
        }

        // Make new bubbles
        bubbles = new List<GameObject>();
        bubbleCount = Random.Range(1, 4);
        for (int i = 0; i < bubbleCount; i++)
        {
            float bubbleCenterX = Random.Range(screenCollider.bounds.min.x+maxBubbleSize, screenCollider.bounds.max.x-maxBubbleSize)-transform.position.x;
            // float bubbleCenterX = screenCollider.bounds.min.x;
            float bubbleCenterY = Random.Range(screenCollider.bounds.min.y+maxBubbleSize, screenCollider.bounds.max.y-maxBubbleSize)-transform.position.y;
            // float bubbleCenterY = screenCollider.bounds.min.y;
            Vector3 bubbleCenter = new Vector3(bubbleCenterX, bubbleCenterY, 0);
            GameObject bubble = Instantiate(bubblePrefab, new Vector3(0,0,0), Quaternion.identity);
            bubbles.Add(bubble);
            bubble.transform.SetParent(transform, false);
            bubble.GetComponent<BubbleScript>().InitBubble(bubbleCenter, Random.Range(minBubbleSize, maxBubbleSize), screenCollider);
        }
    }
}
