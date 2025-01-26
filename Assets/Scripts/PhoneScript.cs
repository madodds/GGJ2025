using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PhoneScript : MonoBehaviour
{
    public GameObject customerObject;
    public float moveDuration = 1.5f;

    public ScreenProtectorScript screenProtector;
    private GameLogicManager gameLogicManager;
    private TextMeshPro clockTextMesh;
    private Vector3 workingPosition;
    private Vector3 scaleAtWorkingPosition = new Vector3(1.5f, 1.5f, 1.5f);
    private Vector3 scaleAtCustomer = new Vector3(0.25f, 0.25f, 0.25f);
    private bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = scaleAtCustomer;
        clockTextMesh = transform.Find("Clock").GetComponent<TextMeshPro>();
        clockTextMesh.text = System.DateTime.Now.ToString("HH:mm");
        workingPosition = GameObject.Find("PhoneWorkPoint").transform.position;
        isMoving = false;
        // Hide this object and children initially
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameLogicManager != null)
        {

        }
    }

    public void MoveToWorkingPosition()
    {
        // Show this object and children
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
        StartCoroutine(DoMove(workingPosition, scaleAtWorkingPosition, false));
    }

    public void MoveToCustomer()
    {
        if (customerObject!= null)
        {
            StartCoroutine(DoMove(customerObject.transform.position, scaleAtCustomer, true));
        }
        else
        {
            Debug.LogWarning("Phone has no customer!");
        }
    }

    public IEnumerator DoMove(Vector3 targetPosition, Vector3 targetScale, bool destroyWhenDone)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;
        Vector3 startingScale = transform.localScale;

        while(elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime/moveDuration);
            transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime/moveDuration);
            yield return null;
        }

        transform.position = targetPosition;
        transform.localScale = targetScale;
        isMoving = false;
        if (destroyWhenDone)
        {
            // Let it live a little longer so the people can admire.
            yield return new WaitForSeconds(2.0f);
            if (screenProtector)
            {
                screenProtector.Reset();
            }
            Destroy(gameObject);
            // foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
            // {
            //     renderer.enabled = false;
            // }
        }
    }
}
