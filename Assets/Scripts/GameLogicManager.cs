using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    public RandomSpriteManager spriteManager;
    public GameObject originalCustomer;
    public AudioSource backgroundAudioSource;

    private GameObject currentCustomer;

    private GameObject screenProtector; // Ref to the screen protector, to manage its state
    private GameObject phone; // Ref to the phone, to manage its state.

    void Start()
    {
        screenProtector = GameObject.Find("ScreenProtector");
        backgroundAudioSource.volume = 0.10f;
        backgroundAudioSource.playOnAwake = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextCustomer();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteCustomer();
        }
    }

    public void NextCustomer()
    {
        // Duplicate the object and assign the next sprite
        if (currentCustomer == null)
        {
            currentCustomer = spriteManager.DuplicateGameObject(originalCustomer);
        }

        // Call a method on the new object
        if (currentCustomer != null)
        {
            CallMethodOnObject(currentCustomer, "StartMovement");
        }
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
