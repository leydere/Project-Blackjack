using UnityEngine;

public class CardView : MonoBehaviour
{
    public GameObject Card { get; private set; }
    public bool IsFaceUp { get; set; }

    /// <summary>
    /// Card is what card it is and is either face up or not.
    /// </summary>
    /// <param name="card">Card from original card collection.</param>
    public CardView(GameObject card)
    {
        Card = card;
        IsFaceUp = false;
    }
}
