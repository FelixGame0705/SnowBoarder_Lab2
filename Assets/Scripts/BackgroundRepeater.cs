using UnityEngine;

public class BackgroundRepeater : MonoBehaviour
{
    [Tooltip("The child object containing the sprite to repeat")]
    [SerializeField]
    private Transform spriteObject;

    [Tooltip("How many copies to create on each side")]
    [SerializeField]
    private int repeats = 2;

    [Tooltip("Only repeat horizontally (X-axis)")]
    [SerializeField]
    private bool horizontalOnly = true;

    private float spriteWidth;
    private float spriteHeight;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteObject == null)
        {
            spriteObject = transform.GetChild(0);
            if (spriteObject == null)
            {
                Debug.LogError("No sprite object found to repeat. Please assign one.");
                return;
            }
        }

        // Get sprite dimensions
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the sprite object.");
            return;
        }

        spriteWidth = spriteRenderer.bounds.size.x;
        spriteHeight = spriteRenderer.bounds.size.y;

        // Create repeating sprites
        CreateRepeatingSprites();
    }

    private void CreateRepeatingSprites()
    {
        // Create horizontal repeats
        for (int i = 1; i <= repeats; i++)
        {
            // Right side
            CreateRepeatSprite(new Vector3(spriteWidth * i, 0, 0));

            // Left side
            CreateRepeatSprite(new Vector3(-spriteWidth * i, 0, 0));
        }

        // Create vertical repeats if needed
        if (!horizontalOnly)
        {
            for (int i = 1; i <= repeats; i++)
            {
                // Top side
                CreateRepeatSprite(new Vector3(0, spriteHeight * i, 0));

                // Bottom side
                CreateRepeatSprite(new Vector3(0, -spriteHeight * i, 0));

                // Diagonals
                for (int j = 1; j <= repeats; j++)
                {
                    // Top-right
                    CreateRepeatSprite(new Vector3(spriteWidth * j, spriteHeight * i, 0));

                    // Top-left
                    CreateRepeatSprite(new Vector3(-spriteWidth * j, spriteHeight * i, 0));

                    // Bottom-right
                    CreateRepeatSprite(new Vector3(spriteWidth * j, -spriteHeight * i, 0));

                    // Bottom-left
                    CreateRepeatSprite(new Vector3(-spriteWidth * j, -spriteHeight * i, 0));
                }
            }
        }
    }

    private void CreateRepeatSprite(Vector3 position)
    {
        GameObject clone = Instantiate(spriteObject.gameObject, transform);
        clone.transform.localPosition = position;

        // Make sure the clone has the same sorting properties
        SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
        if (cloneRenderer != null && spriteRenderer != null)
        {
            cloneRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            cloneRenderer.sortingOrder = spriteRenderer.sortingOrder;
        }
    }
}
