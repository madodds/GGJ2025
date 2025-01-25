using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public enum ItemType
    {
        Cleaner,
        Wipes,
        Squeegee,
        OldScreenProtector,
        NewScreenProtector
    }

    [Header("Object Settings")]
    public ItemType itemType; // Type of the item
    public bool isSelectable = true; // Whether this object can currently be selected

    [HideInInspector] public Color originalColor; // To store the original color of the object

    void Awake()
    {
        // Store the original color on initialization
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
}
