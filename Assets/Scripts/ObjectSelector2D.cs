using UnityEngine;

public class ObjectSelector2D : MonoBehaviour
{
    private GameObject currentlySelectedObject; // The currently selected object

    void Update()
    {
        // Detect mouse clicks
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            HandleSelection();
        }
    }

    void HandleSelection()
    {
        // Perform a raycast to detect the clicked object
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Check if the clicked object has a SelectableObject component
            SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();
            if (selectable != null && selectable.isSelectable)
            {
                GameObject clickedObject = hit.collider.gameObject;

                // If the clicked object is already selected
                if (clickedObject == currentlySelectedObject)
                {
                    DeselectCurrentObject();
                    return;
                }

                // Deselect the previously selected object
                DeselectCurrentObject();

                // Select the new object
                SelectObject(clickedObject);
                return;
            }
        }

        // No valid object clicked; deselect the current object
        //DeselectCurrentObject();
    }

    void SelectObject(GameObject obj)
    {
        currentlySelectedObject = obj;

        // Change object's appearance to transparent and grayscale
        SetObjectAppearance(obj, transparent: true, grayscale: true);

        // Use the object's sprite as the cursor
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Texture2D cursorTexture = SpriteToTexture(spriteRenderer.sprite);
            Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2); // Center the cursor
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }

        // Optionally: Log the item's type
        SelectableObject selectable = obj.GetComponent<SelectableObject>();
        if (selectable != null)
        {
            Debug.Log($"Selected Item Type: {selectable.itemType}");
        }
    }

    void DeselectCurrentObject()
    {
        if (currentlySelectedObject != null)
        {
            // Restore the object's appearance
            SetObjectAppearance(currentlySelectedObject, transparent: false, grayscale: false);

            currentlySelectedObject = null;
        }

        // Reset the cursor to the system default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void SetObjectAppearance(GameObject obj, bool transparent, bool grayscale)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        SelectableObject selectable = obj.GetComponent<SelectableObject>();

        if (spriteRenderer != null && selectable != null)
        {
            Color color = selectable.originalColor; // Start with the original color

            // Adjust transparency
            color.a = transparent ? 0.5f : selectable.originalColor.a;

            // Adjust grayscale (use the same value for all RGB channels)
            if (grayscale)
            {
                float grayValue = 0.5f * (color.r + color.g + color.b); // Average RGB for simplicity
                color.r = color.g = color.b = grayValue;
            }

            // Apply the modified color
            spriteRenderer.color = color;
        }
    }

    Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
        {
            Debug.LogWarning("Sprite or texture is null.");
            return null;
        }

        // Check if the texture is readable
        if (!sprite.texture.isReadable)
        {
            Debug.LogError($"Texture '{sprite.texture.name}' is not readable. Please enable Read/Write in the texture's import settings.");
            return null;
        }

        // Convert a Sprite to a Texture2D for use as a cursor
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height
        );
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
