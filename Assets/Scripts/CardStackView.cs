using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CardStack))]

public class CardStackView : MonoBehaviour
{
    CardStack deck;
    Dictionary<int, CardView> fetchedCards;

    public Vector3 start;
    public float cardOffset;
    public bool faceUp = false;
    public bool reverseLayerOrder = false;
    public GameObject cardPreFab;

    /// <summary>
    /// IsFaceUp bool value is set by passing through this method. Used in dealing to dealer.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="isFaceUp"></param>
    public void Toggle(int card, bool isFaceUp)
    {
        fetchedCards[card].IsFaceUp = isFaceUp;
    }

    /// <summary>
    /// Clears a stack for a fresh start.
    /// </summary>
    public void Clear()
    {
        deck.Reset();

        foreach (CardView view in fetchedCards.Values)
        {
            Destroy(view.Card);
        }

        fetchedCards.Clear();
    }

    /// <summary>
    /// Ensures deck is visually populated on start up and that animations are called when adding and subtracting cards.
    /// </summary>
    void Awake()
    {
        fetchedCards = new Dictionary<int, CardView>();
        deck = GetComponent<CardStack>();
        ShowCards();

        deck.CardRemoved += deck_CardRemoved;
        deck.CardAdded += deck_CardAdded;
    }

    /// <summary>
    /// Card is visually added to a stack.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void deck_CardAdded(object sender, CardEventArgs e)
    {
        float co = cardOffset * deck.CardCount;
        Vector3 temp = start + new Vector3(co, 0f);
        AddCard(temp, e.CardIndex, deck.CardCount);
    }

    /// <summary>
    /// Card is visually removed from a stack.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void deck_CardRemoved(object sender, CardEventArgs e)
    {
        if (fetchedCards.ContainsKey(e.CardIndex))
        {
            Destroy(fetchedCards[e.CardIndex].Card);
            fetchedCards.Remove(e.CardIndex);
        }
    }

    /// <summary>
    /// Update is continously called, so this ensures that the deck is visually up to date.
    /// </summary>
    private void Update()
    {
        ShowCards();
    }

    /// <summary>
    /// Displays the deck of cards.
    /// </summary>
    public void ShowCards()
    {
        int cardCount = 0;

        if (deck.HasCards)
        {
            foreach (int i in deck.GetCards())
            {
                float co = cardOffset * cardCount;
                Vector3 temp = start + new Vector3(co, 0f);
                AddCard(temp, i, cardCount);
                cardCount++;
            }
        }
    }

    /// <summary>
    /// Determines positioning of cards contained in a stack.
    /// </summary>
    /// <param name="position">Startign position</param>
    /// <param name="cardIndex">Index of card</param>
    /// <param name="positionalIndex">How many cards in stack</param>
    void AddCard(Vector3 position, int cardIndex, int positionalIndex)
    {
        if (fetchedCards.ContainsKey(cardIndex))
        {
            if (!faceUp)
            {
                CardModel model = fetchedCards[cardIndex].Card.GetComponent<CardModel>();
                model.ToggleFace(fetchedCards[cardIndex].IsFaceUp);
            }
            return;
        }

        GameObject cardCopy = (GameObject)Instantiate(cardPreFab);
        cardCopy.transform.position = position;

        CardModel cardModel = cardCopy.GetComponent<CardModel>();
        cardModel.cardIndex = cardIndex;
        cardModel.ToggleFace(faceUp);

        SpriteRenderer spriteRenderer = cardCopy.GetComponent<SpriteRenderer>();
        if (reverseLayerOrder)
        {
            spriteRenderer.sortingOrder = 51 - positionalIndex;
        }
        else
        {
            spriteRenderer.sortingOrder = positionalIndex;
        }

        fetchedCards.Add(cardIndex, new CardView(cardCopy));
    }
}
