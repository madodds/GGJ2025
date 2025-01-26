using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum CustomerStateEnum
{
    ShiftStart,
    Inbound,
    Intro,
    Working,
    Evaluation,
    Outbound,
    ShiftOver

}
public class GameLogicManager : MonoBehaviour
{
    // Public
    public RandomSpriteManager spriteManager;
    public GameObject originalCustomer;
    public List<GameObject> customers;
    public AudioSource backgroundAudioSource;
    public CameraMover cameraMover;
    public bool RandomizeCustomers = true;

    // Speech 
    public GameObject speechBubbleObject;

    private VisualElement speechBubbleUiRoot;

    // Customer
    public GameObject gameSpaceObject; // Ref to GameSpace to parent created customers
    private int currentCustomerIndex = 0; // Index to track the next customer
    private Vector3 customerSpawnPosition;
    private GameObject currentCustomer;

        // Keep track of what's going on in relation to customer
    private CustomerStateEnum customerState;  

    // Phone & tools
    private GameObject screenProtector; // Ref to the screen protector, to manage its state

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
        if (RandomizeCustomers) 
            RandomizeCustomerList();
        customerState = CustomerStateEnum.ShiftStart;
    }

    void Update()
    {
        // Loose attempt at a state machine. At each customerState, we check for different
        // conditions to be met before performing behaviour/action and moving on to the next state.
        switch(customerState)
        {
            // At ShiftStart, listen for the game start key (TODO: make a button on the desk)
            case CustomerStateEnum.ShiftStart:
                if (Input.GetKeyDown(KeyCode.D))
                {
                    // Create (and move) the next customer
                    NextCustomer();
                    
                    customerState = CustomerStateEnum.Inbound;
                }
                break;

            // While waiting for the customer to reach the desk, poll their move state.
            case CustomerStateEnum.Inbound:
                if (!currentCustomer.GetComponent<SmoothMover2D>().IsMoving()){
                    // When they stop show the speech bubble and 
                    // pass it to the customer script so that it can
                    // (TODO) start displaying the intro text.
                    speechBubbleUiRoot.style.display = DisplayStyle.Flex;
                    Customer customer = currentCustomer.GetComponent<Customer>();
                    customer.StartIntroSpeech(speechBubbleUiRoot.Q<Label>("SpeechLabel"));
                    // Also have them give their phone to the desk.
                    // !! This is assumed to finish before the speech does !!
                    customer.PhoneToDesk();

                    customerState = CustomerStateEnum.Intro;
                }
                break;

            // In this state, poll for customer to finish talking
            case CustomerStateEnum.Intro:
                if (!currentCustomer.GetComponent<Customer>().IsTalking())
                {
                    // Turn off speech bubble and move camera down 
                    // so player can begin working
                    speechBubbleUiRoot.style.display = DisplayStyle.None;
                    cameraMover.SetCamera();
                    
                    customerState = CustomerStateEnum.Working;
                }
                break;

            // When Working, poll for SUBMITTED state on the screen protector
            // (Right now happens when all bubbles are removed. Add a submit
            // button to desk later)
            case CustomerStateEnum.Working:
                if(screenProtector.GetComponent<ScreenProtectorScript>().GetState() == ScreenProtectorStatus.Submitted)
                {
                    // Reset the camera, and have the customer start their evaluation speech.
                    cameraMover.ResetCamera();
                    speechBubbleUiRoot.style.display = DisplayStyle.Flex;
                    Customer customer = currentCustomer.GetComponent<Customer>(); 
                    customer.StartEvaluationSpeech(speechBubbleUiRoot.Q<Label>("SpeechLabel"));
                    // Also have the phone move to the customer.
                    // !! This is assumed to finish before the speech does !!
                    customer.PhoneToCustomer();

                    customerState = CustomerStateEnum.Evaluation;
                }
                break;

            // Wait for the the customer to finish their evaluation speech.
            case CustomerStateEnum.Evaluation:
                if (!currentCustomer.GetComponent<Customer>().IsTalking())
                {
                    // Start customer movement away from desk
                    speechBubbleUiRoot.style.display = DisplayStyle.None;
                    CallMethodOnObject(currentCustomer, "StartMovement");

                    // Reset screen protector
                    screenProtector.GetComponent<ScreenProtectorScript>().Reset();

                    customerState = CustomerStateEnum.Outbound;
                }
                break;

            // Wait for current customer to complete their exit (and deletion).
            case CustomerStateEnum.Outbound:
                if (currentCustomer == null)
                {
                    // TODO: Add a shift-over check to end the game.
                    if(false)
                    {
                        customerState = CustomerStateEnum.ShiftOver;
                    }
                    // Create (and move) the next customer.
                    NextCustomer();

                    customerState = CustomerStateEnum.Inbound;
                }
                break;

            case CustomerStateEnum.ShiftOver:
                // TODO: Implement end of shift/game.
                break;
        }
        

        // if (Input.GetKeyDown(KeyCode.X))
        // {
        //     DeleteCustomer();
        // }
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

    public GameObject GetCurrentCustomer(){
        return currentCustomer;
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
        // Don't use backgroundAudioSource, this object has its own component for audiosource.
        newObject.GetComponent<SmoothMover2D>().audioSource = gameObject.GetComponent<AudioSource>();

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
