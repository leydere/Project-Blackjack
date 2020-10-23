using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    List<int> cards;

    public bool isGameDeck;

    public bool HasCards
    {
        get { return cards != null && cards.Count > 0; } // sets bool HasCards to true only if the cardStack has cards in it
    }

    public event CardEventHandler CardRemoved;
    public event CardEventHandler CardAdded;

    /// <summary>
    /// Runs the CreateDeck method when the application starts up.
    /// </summary>
    void Awake()
    {
        cards = new List<int>();
        if (isGameDeck)
        {
            CreateDeck();
        }
    }

    /// <summary>
    /// Allows cards.Count to not break anything by returning null on an empty hand.
    /// </summary>
    public int CardCount
    {
        get
        {
            if (cards == null)
            {
                return 0;
            }
            else
            {
                return cards.Count;
            }
        }
    }

    /// <summary>
    /// Not 100% on the IEnumerable yet, but I believe it is enumerating a list from what ever you run through it.  
    /// So in this case it seems to pull an int off of each card object. The int being related to the orignal position
    /// of the card in the card deck collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetCards()
    {
        foreach (int i in cards)
        {
            yield return i;
        }
    }

    /// <summary>
    /// Removes a card to be dealt to a hand.
    /// </summary>
    /// <returns>Int that represents card index position from the original card deck ceollection.</returns>
    public int RemoveCard()
    {
        int temp = cards[0];
        cards.RemoveAt(0);

        if (CardRemoved != null)
        {
            CardRemoved(this, new CardEventArgs(temp));
        }

        return temp;
    }

    /// <summary>
    /// Removes the second card in a stack istead of the first.
    /// </summary>
    /// <returns>Int that represents card index position from the original card deck ceollection.</returns>
    public int AltRemoveCard()
    {
        int temp = cards[1];
        cards.RemoveAt(1);

        if (CardRemoved != null)
        {
            CardRemoved(this, new CardEventArgs(temp));
        }

        return temp;
    }

    /// <summary>
    /// Add a card to a card stack.
    /// </summary>
    /// <param name="card">Int that represents card index position from the original card deck ceollection. Collected from RemoveCard() or AltRemoveCard() methods.</param>
    public void AddCard(int card)
    {
        cards.Add(card);

        if (CardAdded != null)
        {
            CardAdded(this, new CardEventArgs(card));
        }
    }

    /// <summary>
    /// Determines the total value of a hand.
    /// </summary>
    /// <returns>Returns the handle value total as an int.</returns>
    public int HandValue()
    {
        int total = 0;
        int aces = 0;

        foreach (int card in GetCards())
        {
            int cardRank = card % 13;

            if (cardRank == 0) { cardRank = 11; aces++; } // Handles aces
            else if (cardRank > 0 && cardRank < 10) { cardRank += 1; } // Handles numbered cards
            else if (cardRank > 9) { cardRank = 10; } // Handles face cards

            total = total + cardRank;
        }

        // Runs once for each ace in the hand and if you bust it shaves 10 points off the total.
        for (int i = 0; i < aces; i++)
        {
            if (total > 21)
            {
                total -= 10;
            }
        }

        return total;
    }

    /// <summary>
    /// Checks hand to see if it contains any aces and if those aces have had their values 
    /// reduced.  Exact copy of HandValue() method but returns a bool instead of an int.
    /// </summary>
    /// <returns>Returns true if reduction found, false if not.</returns>
    public bool AcesReduced()
    {
        int total = 0;
        int aces = 0;

        foreach (int card in GetCards())
        {
            int cardRank = card % 13;

            if (cardRank == 0) { cardRank = 11; aces++; }
            else if (cardRank > 0 && cardRank < 10) { cardRank += 1; }
            else if (cardRank > 9) { cardRank = 10; }

            total = total + cardRank;
        }

        for (int i = 0; i < aces; i++)
        {
            if (total > 21)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks cards in hand to see if they are a pair.
    /// </summary>
    /// <returns>Returns true if pair found.</returns>
    public bool CanSplit()
    {
        List<int> cardValues = new List<int>();

        foreach (int card in GetCards())
        {
            int cardRank = card % 13;
            cardValues.Add(cardRank);
        }

        int A = cardValues[0];
        int B = cardValues[1];

        if (A == B)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks to see if hand value is 9, 10, or 11.
    /// </summary>
    /// <returns>Returns true value is found to be in range.</returns>
    public bool CanDouble()
    {
        int total = HandValue();

        if (total >= 9 && total <= 11)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates and shuffles a new deck of cards.  I forget the name of the shuffling method.  It is 
    /// probably not as efficient as simply drawing a card at random, but I like that it simulates a 
    /// shuffled deck of cards.
    /// </summary>
    public void CreateDeck()
    {
        cards.Clear();

        for (int i = 0; i < 52; i++) // populates the deck
        {
            cards.Add(i);
        }

        int n = cards.Count;
        while (n > 1) // handles the shuffling
        {
            n--;
            int k = Random.Range(0, n + 1);
            int temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
    }

    /// <summary>
    /// Clears a list of cards. ie. a card stack
    /// </summary>
    public void Reset()
    {
        cards.Clear();
    }
}
