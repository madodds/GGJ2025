using UnityEngine;

public class GameLogicManager : MonoBehaviour
{
    public RandomSpriteManager spriteManager;
    public GameObject originalCustomer;

    private GameObject currentCustomer;

    private GameObject screenProtector;

    void Start()
    {
        screenProtector = GameObject.Find("ScreenProtector");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
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
                if (screenProtector != null)
                {
                    screenProtector.GetComponent<ScreenProtectorScript>().SpawnBubbles();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // Destroy the object (example)
            DestroyGameObject(currentCustomer);
            currentCustomer = null;
        }
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
