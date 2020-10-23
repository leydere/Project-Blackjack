using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    #region Properties (cardstacks, buttons, text, etc.)
    int dealersFirstCard = -1;

    public CardStack deck; 
    public CardStack dealer;
    public CardStack player;
    public CardStack sideHand;

    public Coinage chips;

    public Button HitButton; 
    public Button StandButton;
    public Button PlayAgainButton;
    public Button SplitButton;
    public Button DoubleButton;
    public Button TenButton;
    public Button TwentyButton;
    public Button FiftyButton;
    public Button SetBetButton;
    public Button ResetBetButton;
    public Button MaxBetButton;

    public Text WinnerText; 
    public Text DealerValueText;
    public Text PlayerValueText;
    public Text SideValueText;
    public Text BetAmountText;
    public Text PurseAmountText;
    public Text PurseIdentifierText;
    public Text PlayerHandIndicator;
    public Text SideHandIndicator;
    public Text PlayerAceReducedText;
    public Text SideAceReducedText;

    private bool split = false;
    private bool toSide = false;
    private bool loser = false;
    private bool natural21Found = false;

    #endregion

    #region Initialize Game
    /// <summary>
    /// Calls whatever method(s) desired when the application starts.
    /// Sets the resolution, purse starting value and goes to the betting phase.
    /// </summary>
    private void Start()
    {
        Screen.SetResolution(1225, 578, false);
        chips.purse = 500;
        StartCoroutine(StartBetPhase());

    }

    /// <summary>
    /// Handles the betting phase.
    /// </summary>
    /// <returns></returns>
    IEnumerator StartBetPhase()
    {
        ActivateBetButtons();
        NewHandStatusReset();
        WinnerText.text = "Place your bet and press 'Set' to proceed.";
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Beginning of a hand.  Cards are dealt out. Checks for natural 21, then split, and then double down.
    /// </summary>
    /// <returns></returns>
    IEnumerator StartGame()
    {
        WinnerText.text = "";
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            player.AddCard(deck.RemoveCard());
            yield return new WaitForSeconds(0.5f);
            HitDealer();
        }
        yield return new WaitForSeconds(0.5f);
        PlayerValueText.text = HandValueDisplay(player);
        PlayerAceReducedText.text = AceReductionText(player.AcesReduced());

        Natural21Handler(FindNatural21());
        
        if (natural21Found == false)
        {
            if (player.CanSplit() == true && chips.purse >= chips.pot) { SplitButton.interactable = true; };
            if (player.CanDouble() == true) { DoubleButton.interactable = true; };
            HitButton.interactable = true;
            StandButton.interactable = true;
        }
    }

    /// <summary>
    /// Toggles various settings to desirable status for new hands.
    /// </summary>
    private void NewHandStatusReset()
    {
        split = false;
        toSide = false;
        natural21Found = false;
        dealersFirstCard = -1;

        HitButton.interactable = false;
        StandButton.interactable = false;
        SplitButton.interactable = false;
        PlayAgainButton.interactable = false;

        player.GetComponent<CardStackView>().Clear();
        dealer.GetComponent<CardStackView>().Clear();
        sideHand.GetComponent<CardStackView>().Clear();
        if (deck.CardCount < 26)
        {
            deck.GetComponent<CardStackView>().Clear();
            deck.CreateDeck();
        }

        WinnerText.text = "";
        PlayerValueText.text = "";
        DealerValueText.text = "";
        SideValueText.text = "";
        PlayerAceReducedText.text = "";
        SideAceReducedText.text = "";
        PurseAmountText.text = "$" + chips.purse.ToString();
    }
    #endregion

    #region Main Game Buttons
    /// <summary>
    /// Player is dealt a card.  Activated by the 'Hit' button. Functionality changes if split button had been activated.
    /// </summary>
    public void Hit()
    {
        SplitButton.interactable = false;
        DoubleButton.interactable = false;
        if (split == false && toSide == false)
        {
            player.AddCard(deck.RemoveCard());
            PlayerValueText.text = HandValueDisplay(player);
            PlayerAceReducedText.text = AceReductionText(player.AcesReduced());
            if (player.HandValue() > 21)
            {
                HitButton.interactable = false;
                StandButton.interactable = false;
                StartCoroutine(DealersTurn());
            }
        }
        else if (split == true && toSide == false)
        {
            player.AddCard(deck.RemoveCard());
            PlayerValueText.text = HandValueDisplay(player);
            PlayerAceReducedText.text = AceReductionText(player.AcesReduced());
            if (player.HandValue() > 21)
            {
                toSide = true;
                PlayerHandIndicator.text = "";
                SideHandIndicator.text = "*";
            }
        }
        else if (toSide == true)
        {
            sideHand.AddCard(deck.RemoveCard());
            SideValueText.text = HandValueDisplay(sideHand);
            SideAceReducedText.text = AceReductionText(sideHand.AcesReduced());
            if (sideHand.HandValue() > 21)
            {
                SideHandIndicator.text = "";
                HitButton.interactable = false;
                StandButton.interactable = false;
                StartCoroutine(DealersTurn());
            }
        }
    }

    /// <summary>
    /// Ends players turn. Activated by the 'Stand' button. Functionality changes if split button had been activated.
    /// </summary>
    public void Stand()
    {
        SplitButton.interactable = false;
        DoubleButton.interactable = false;
        if (split == false)
        {
            HitButton.interactable = false;
            StandButton.interactable = false;
            StartCoroutine(DealersTurn());
        }
        else if (split == true && toSide == false)
        {
            toSide = true;
            PlayerHandIndicator.text = "";
            SideHandIndicator.text = "*";
        }
        else if (split == true && toSide == true)
        {
            SideHandIndicator.text = "";
            HitButton.interactable = false;
            StandButton.interactable = false;
            StartCoroutine(DealersTurn());
        }

    }

    /// <summary>
    /// Resets numerous components and starts new hand. Activated by the 'PlayAgain' button. Functionality changes if minimum bet as not had.
    /// </summary>
    public void PlayAgain()
    {
        if (loser == true)
        {
            chips.purse = 500;
            loser = false;
            StartCoroutine(StartBetPhase());
        }
        else if (chips.purse < 10)
        {
            WinnerText.text = "You do not have the minimum bet to proceed.  Press 'Play Again' to start over.";
            loser = true;
            PlayAgainButton.interactable = true;
        }
        else
        {
            StartCoroutine(StartBetPhase());
        }
     }

    /// <summary>
    /// Moves card from hand to side hand and doubles bet.  Activated by 'Split' button.
    /// </summary>
    public void Split()
    {
        SplitButton.interactable = false;
        DoubleButton.interactable = false;
        split = true;
        sideHand.AddCard(player.AltRemoveCard());
        SideValueText.text = HandValueDisplay(sideHand);
        SideAceReducedText.text = AceReductionText(sideHand.AcesReduced());
        PlayerValueText.text = HandValueDisplay(player);
        PlayerAceReducedText.text = AceReductionText(player.AcesReduced());
        BetAmountText.text = "$" + chips.pot + " x2";
        chips.purse -= chips.pot;
        PurseAmountText.text = "$" + chips.purse;
        PlayerHandIndicator.text = "*";
    }

    /// <summary>
    /// Doubles the bet, draws a single card, and moves to dealer's turn. Activated by 'Double' button.
    /// </summary>
    public void Double()
    {
        SplitButton.interactable = false;
        DoubleButton.interactable = false;
        chips.pot = (chips.pot * 2);
        BetAmountText.text = "$" + chips.pot;
        player.AddCard(deck.RemoveCard());
        PlayerValueText.text = HandValueDisplay(player);
        PlayerAceReducedText.text = AceReductionText(player.AcesReduced());
        StartCoroutine(DealersTurn());
    }
    #endregion

    #region Betting Buttons

    /// <summary>
    /// Increments bet by $10.  Activated by pressing the '10Button' button.
    /// </summary>
    public void Bet10()
    {
        BetAmountText.text = "$" + chips.DetermineBet(10);
    }

    /// <summary>
    /// Increments bet by $20.  Activated by pressing the '20Button' button.
    /// </summary>
    public void Bet20()
    {
        BetAmountText.text = "$" + chips.DetermineBet(20);
    }

    /// <summary>
    /// Increments bet by $50.  Activated by pressing the '50Button' button.
    /// </summary>
    public void Bet50()
    {
        BetAmountText.text = "$" + chips.DetermineBet(50);
    }

    /// <summary>
    /// Sets the bet to whatever amount the player has determined and goes to the main game phase. Activated by the 'SetBet' button.
    /// </summary>
    public void SetBet()
    {
        DeactivateBetButtons();
        PurseAmountText.text = "$" + chips.ConfirmBet();
        StartCoroutine(StartGame());
    }

    /// <summary>
    /// Sets the bet to the max bet. Activated by the 'MaxBet' button.
    /// </summary>
    public void BetMax()
    {
        BetAmountText.text = "$" + chips.DetermineBet(100);
    }

    /// <summary>
    /// Sets the bet to the minimum bet. Activated by the 'Reset Bet' button.
    /// </summary>
    public void BetReset()
    {
        chips.BetIntitialize();
        BetAmountText.text = "$10";
    }

    /// <summary>
    /// Starts the bet at $10 and turns on all the betting buttons.
    /// </summary>
    private void ActivateBetButtons()
    {
        chips.BetIntitialize();
        BetAmountText.text = "$10";
        TenButton.interactable = true;
        TwentyButton.interactable = true;
        FiftyButton.interactable = true;
        SetBetButton.interactable = true;
        ResetBetButton.interactable = true;
        MaxBetButton.interactable = true;
    }

    /// <summary>
    /// Turns off all the betting buttons.
    /// </summary>
    private void DeactivateBetButtons()
    {
        TenButton.interactable = false;
        TwentyButton.interactable = false;
        FiftyButton.interactable = false;
        SetBetButton.interactable = false;
        ResetBetButton.interactable = false;
        MaxBetButton.interactable = false;
    }

    #endregion

    #region Dealer Handler
    /// <summary>
    /// Handles events after player turn ends. Includes both dealer drawing and winner declaration.
    /// </summary>
    /// <returns></returns>
    IEnumerator DealersTurn()
    {
        // Flips the dealers pocket card face up and displays hand value.
        CardStackView view = dealer.GetComponent<CardStackView>();
        view.Toggle(dealersFirstCard, true);
        view.ShowCards();
        DealerValueText.text = HandValueDisplay(dealer);
        yield return new WaitForSeconds(1f);

        // Dealer draws while hand value < 17.
        while (dealer.HandValue() < 17 && player.HandValue() <= 21)
        {
            HitDealer();
            DealerValueText.text = HandValueDisplay(dealer);
            yield return new WaitForSeconds(1f);
        }

        // Winner declaration for regular hand.
        if (split == false)
        {
            int winType = DetermineWinner(player);
            WinnerText.text = WinTextHandler(winType);
            chips.Payout(winType);
            PurseAmountText.text = "$" + chips.purse.ToString();
            BetAmountText.text = "";
        }

        //Winner determination for split hand.
        else if (split == true)
        {
            int winA = DetermineWinner(player);
            int winB = DetermineWinner(sideHand);
            WinnerText.text = WinTextHandler(winA, winB);
            chips.Payout(winA);
            chips.Payout(winB);
            PurseAmountText.text = "$" + chips.purse.ToString();
            BetAmountText.text = "";
         }

        yield return new WaitForSeconds(1f);
        PlayAgainButton.interactable = true;
    }

    /// <summary>
    /// Deal a card to the dealer
    /// </summary>
    void HitDealer()
    {
        int card = deck.RemoveCard();
        // Purpose of dealersFirstCard variable is to track and be flipped face up later.
        if (dealersFirstCard < 0)
        {
            dealersFirstCard = card;
        }
        dealer.AddCard(card);
        if (dealer.CardCount >= 2)
        {
            CardStackView view = dealer.GetComponent<CardStackView>();
            view.Toggle(card, true);
        }
    }
    #endregion

    #region Hand Value Methods
    /// <summary>
    /// Checks dealer hand vs players hand(s).
    /// </summary>
    /// <param name="whom">Hand that will be checked against dealer hand.  Either player or sideHand.</param>
    /// <returns>Returns an integer equal to the factor which the bet should be multiplied and returned.</returns>
    private int DetermineWinner(CardStack whom)
    {
        if (dealer.HandValue() > 21 && whom.HandValue() > 21) { return 1; }
        else if (dealer.HandValue() > 21 && whom.HandValue() <= 21) { return 2; }
        else if (dealer.HandValue() <= 21 && whom.HandValue() > 21) { return 0; }
        else if (dealer.HandValue() == whom.HandValue()) { return 1; }
        else if (dealer.HandValue() < whom.HandValue()) { return 2; }
        else if (dealer.HandValue() > whom.HandValue()) { return 0; }

        return -1;
    }

    /// <summary>
    /// Checks dealer and player hands for a value of 21.
    /// </summary>
    /// <returns>Returns an integer equal to the factor which the bet should be multiplied and returned.</returns>
    private int FindNatural21()
    {
        if (dealer.HandValue() == 21 && player.HandValue() != 21) { return 0; }
        else if (dealer.HandValue() == 21 && player.HandValue() == 21) { return 1; }
        else if (dealer.HandValue() != 21 && player.HandValue() == 21) { return 3; }

        return -1;
    }

    /// <summary>
    /// If natural 21 has been found, flips dealer pocket card face up, pays out chips accordingly, and turns on PlayAgain button.
    /// </summary>
    /// <param name="input">Input comes from FindNatural21() method.</param>
    private void Natural21Handler(int input)
    {
        if (input != -1)
        {
            CardStackView view = dealer.GetComponent<CardStackView>();
            view.Toggle(dealersFirstCard, true);
            view.ShowCards();
            DealerValueText.text = HandValueDisplay(dealer);
            WinnerText.text = WinTextHandler(input);
            chips.Payout(input);
            PurseAmountText.text = "$" + chips.purse.ToString();
            BetAmountText.text = "";
            natural21Found = true;
            PlayAgainButton.interactable = true;
        }
    }
    #endregion

    #region Text Handlers
    /// <summary>
    /// Generates text for result of hand resolution on single hand.
    /// </summary>
    /// <param name="input">Input comes from DetermineWinnrer() method.</param>
    /// <returns>String of text for display.</returns>
    private string WinTextHandler(int input)
    {
        switch (input)
        {
            case 0: return "You lost your bet.";
            default: return "$" + (chips.pot * input).ToString() + " paid out";
        }
    }

    /// <summary>
    /// Generates the text for result of hand resolution when had has been split.
    /// </summary>
    /// <param name="A">Input comes from DetermineWinnrer() method.</param>
    /// <param name="B">Input comes from DetermineWinnrer() method.</param>
    /// <returns>String of text for display.</returns>
    private string WinTextHandler(int A, int B)
    {
        int value = A + B;
        switch (value)
        {
            case 0: return "You lost your bet.";
            default: return "$" + (chips.pot * value).ToString() + " paid out";
        }
    }

    /// <summary>
    /// Converts hand value to a string.
    /// </summary>
    /// <param name="whom">Pick which card stack this method applies to.</param>
    /// <returns>String that represents hand value.</returns>
    private string HandValueDisplay(CardStack whom)
    {
        string output = "";

        if (whom.HandValue() > 21) { output = "BUST"; }
        else { output = whom.HandValue().ToString(); }

        return output;
    }

    /// <summary>
    /// Generates text if ace is in hand and the value of that ace has been reduced.
    /// </summary>
    /// <param name="input">Input comes from AcesReduced() method.</param>
    /// <returns>String for display.</returns>
    private string AceReductionText(bool input)
    {
        if (input == true) { return "value of ace reduced"; };

        return "";
    }
    #endregion
}
