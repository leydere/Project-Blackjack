using UnityEngine;

public class CardModel : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public Sprite[] faces;
    public Sprite cardBack;

    public int cardIndex;

    /// <summary>
    /// Renders either the front or back of the card.
    /// </summary>
    /// <param name="showFace">Bool value determines whther card is rendered face up or down.</param>
    public void ToggleFace(bool showFace)
    {
        if (showFace)
        {
            spriteRenderer.sprite = faces[cardIndex];
        }

        else if (!showFace)
        {
            spriteRenderer.sprite = cardBack;
        }
    }

    /// <summary>
    /// I may be off on this, but my understanding of 'Awake' is that it initializes variables prior to the game beginning, but after game objects have been initialized.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
