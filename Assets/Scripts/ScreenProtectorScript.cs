using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ScreenProtectorStatus
{
    New,
    Held,
    Placed,
    Bubbled,
    Submitted,
}

public class ScreenProtectorScript : MonoBehaviour
{
    public GameObject bubblePrefab;
    private BoxCollider2D screenCollider;
    private List<GameObject> bubbles;
    private int bubbleCount;
    private float minBubbleSize = 0.25f;
    private float maxBubbleSize = 0.35f;
    private Vector3 homePosition;
    private ScreenProtectorStatus status;    // If this item is new, held, or placed.
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);
        homePosition = transform.position;
        screenCollider = transform.GetComponent<BoxCollider2D>();
        status = ScreenProtectorStatus.New;
    }

    // Update is called once per frame
    void Update()
    {
        switch(status)
        {
            case ScreenProtectorStatus.New:
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 msp = Input.mousePosition;
                    Vector3 mp = Camera.main.ScreenToWorldPoint(new Vector3(msp.x, msp.y, 0));
                    if (Vector3.Distance(mp, transform.position) < 3.0f)
                    {
                        status = ScreenProtectorStatus.Held;
                    }
                }
                break;
            case ScreenProtectorStatus.Held:
                Vector3 mouseScreenPos = Input.mousePosition;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 1.0f));
                transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                // TODO: check that this position is  'close enough' to the phone to be considered attached.
                if (Input.GetMouseButtonUp(0))
                {  
                    status = ScreenProtectorStatus.Placed;
                }
                break;
            case ScreenProtectorStatus.Placed:
                status = ScreenProtectorStatus.Bubbled;
                SpawnBubbles();
                break;
            case ScreenProtectorStatus.Bubbled:
                if (bubbles != null)
                {
                    foreach(GameObject bubble in bubbles)
                    {
                        BubbleScript bubbleScript = bubble.GetComponent<BubbleScript>();
                        if (bubbleScript.active && bubbleScript.OutsideRect())
                        {
                            bubbleScript.active = false;
                        }
                    }
                }
                break;
            case ScreenProtectorStatus.Submitted:
                
                break;
        }
    }

    public void Reset()
    {
        status = ScreenProtectorStatus.New;
        transform.position = homePosition;
        // Destroy current bubbles if they exist
        if (bubbles != null)
        {
            foreach (GameObject bubble in bubbles)
            {
                Destroy(bubble);
            }
        }
    }

    public void Submit()
    {
        switch(status)
        {
            case ScreenProtectorStatus.New:
            case ScreenProtectorStatus.Held:
                break;
            case ScreenProtectorStatus.Placed:
            case ScreenProtectorStatus.Bubbled:           
                Transform phoneRootTransform = GameObject.Find("PhoneRoot").transform;
                transform.SetParent(phoneRootTransform, true);
                break;

        }
        status = ScreenProtectorStatus.Submitted;
    }

    void SpawnBubbles()
    {
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
