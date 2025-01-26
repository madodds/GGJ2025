using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLogicManager : MonoBehaviour
{
    // Public
    public RandomSpriteManager spriteManager;
    public GameObject originalCustomer;
    public List<GameObject> customers;
    public AudioSource backgroundAudioSource;

    // Speech 
    public GameObject speechBubbleObject;

    private VisualElement speechBubbleUiRoot;

    // Customer
    private GameObject gameSpaceObject; // Ref to GameSpace to parent created customers
    private int currentCustomerIndex = 0; // Index to track the next customer
    private Vector3 customerSpawnPosition;
    private GameObject currentCustomer;

    // Phone & tools
    private GameObject screenProtector; // Ref to the screen protector, to manage its state
    private GameObject phone; // Ref to the phone, to manage its state.

    void Start()
    {
        gameSpaceObject = GameObject.Find("GameSpace");
        screenProtector = GameObject.Find("ScreenProtector");
        backgroundAudioSource.volume = 0.10f;
        backgroundAudioSource.playOnAwake = true;
        if (speechBubbleObject != null)
        {
            speechBubbleUiRoot = speechBubbleObject.GetComponent<UIDocument>().rootVisualElement;
            speechBubbleUiRoot.style.display = DisplayStyle.None;
        }
        customerSpawnPosition = GameObject.Find("CustomerSpawnPoint").transform.position;
        RandomizeCustomerList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            speechBubbleUiRoot.style.display = DisplayStyle.Flex;
            NextCustomer();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteCustomer();
        }
    }

    // Randomize the order of the customers in the list
    private void RandomizeCustomerList()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            int randomIndex = Random.Range(0, customers.Count);
            GameObject temp = customers[i];
            customers[i] = customers[randomIndex];
            customers[randomIndex] = temp;
        }
    }

    public void NextCustomer()
    {
        // Duplicate the object and assign the next sprite
        if (currentCustomer == null)
        {
            currentCustomer = CreateCustomer();
        }

        // Call a method on the new object
        if (currentCustomer != null)
        {
            CallMethodOnObject(currentCustomer, "StartMovement");
        }
    }

    private GameObject CreateCustomer()
    {
        // Check if end of customer list, reset and shuffle if so.
        if (currentCustomerIndex >= customers.Count)
        {
            currentCustomerIndex = 0;
            RandomizeCustomerList();
        }

        // Instantiate from the shuffled customer list
        GameObject newObject = Instantiate(customers[currentCustomerIndex]);

        // Set the parent to GameSpace
        newObject.transform.SetParent(gameSpaceObject.transform);

        // Move to the customer spawn point
        newObject.transform.position = customerSpawnPosition;

        // Set the customer SmoothMover2D.audioSource
        newObject.GetComponent<SmoothMover2D>().audioSource = backgroundAudioSource;

        // Increment customer list index
        currentCustomerIndex ++;

        return newObject;
    }

    public void DeleteCustomer()
    {
        // Destroy the object (example)
        DestroyGameObject(currentCustomer);
        currentCustomer = null;
    }

    // Public method to call a method on the new object
    public void CallMethodOnObject(GameObject obj, string methodName)
    {
        obj.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
    }

    // Public method to destroy the object
    public void DestroyGameObject(GameObject obj)
    {
        Destroy(obj);
    }
}
