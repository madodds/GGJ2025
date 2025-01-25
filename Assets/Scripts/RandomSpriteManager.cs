using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RandomSpriteManager : MonoBehaviour
{
    [Header("Folder Path for Sprites")]
    public string spritesFolderPath = "Assets/UI/Customers"; // Path to the folder containing sprites

    [Header("Assigned Sprites")]
    public List<Sprite> spriteList = new List<Sprite>(); // List to store sprites

    private int currentSpriteIndex = 0; // Index to track the next sprite

    void Start()
    {
        LoadSpritesFromFolder();
        RandomizeSpriteList();
    }

    // Loads sprites from the specified folder
    private void LoadSpritesFromFolder()
    {
        spriteList.Clear();

        // Get all sprite files in the folder
        string[] files = Directory.GetFiles(spritesFolderPath, "*.png");
        foreach (string file in files)
        {
            // Load the sprite
            string assetPath = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                spriteList.Add(sprite);
            }
        }
    }

    // Randomize the order of the sprites in the list
    private void RandomizeSpriteList()
    {
        for (int i = 0; i < spriteList.Count; i++)
        {
            int randomIndex = Random.Range(0, spriteList.Count);
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }

    // Public method to duplicate an existing game object and assign the next sprite
    public GameObject DuplicateGameObject(GameObject original)
    {
        if (currentSpriteIndex >= spriteList.Count)
        {
            Debug.LogWarning("Resetting Sprite List");
            currentSpriteIndex = 0;
        }

        // Duplicate the game object
        GameObject newObject = Instantiate(original);

        // Set the parent first to ensure proper positioning
        newObject.transform.SetParent(original.transform.parent);

        // Explicitly reset the local transform to match the original
        newObject.transform.localPosition = original.transform.localPosition;
        newObject.transform.localRotation = original.transform.localRotation;
        newObject.transform.localScale = original.transform.localScale;

        // Assign the next sprite in the list
        SpriteRenderer spriteRenderer = newObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteList[currentSpriteIndex];
        }

        // Increment the index
        currentSpriteIndex++;

        // Log to check the world position for debugging
        Debug.Log($"Duplicated Object Local Position: {newObject.transform.localPosition}");
        Debug.Log($"Duplicated Object World Position: {newObject.transform.position}");

        return newObject;
    }

}
