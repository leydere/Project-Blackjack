using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coinage : MonoBehaviour
{
    public int purse;
    public int pot;
    public int tempBet;

    /// <summary>
    /// Sets temporary value used before bet is set to a starting point of 10.
    /// </summary>
    public void BetIntitialize()
    {
        tempBet = 10;
    } 

    /// <summary>
    /// Once bet is set amount determined is removed from the purse and added to the pot.
    /// </summary>
    /// <returns>Returns a string to be displayed.</returns>
    public string ConfirmBet()
    {
        purse -= tempBet;
        pot = tempBet;
        return purse.ToString();
    }

    /// <summary>
    /// Raises bet.  Bet cannot exceed 100 or the amount in the purse.  
    /// </summary>
    /// <param name="amount">Amount that bet is attempted to be raised.</param>
    /// <returns>Returns a string to be displayed.</returns>
    public string DetermineBet(int amount)
    {
        tempBet += amount;
        if (tempBet > purse) { tempBet = purse; }
        if (tempBet > 100) { tempBet = 100; }
        return tempBet.ToString();
    }

    /// <summary>
    /// Pays bet to player.
    /// </summary>
    /// <param name="amount">Amount bet is to be multiplied by before returned to the player.</param>
    public void Payout(int amount)
    {
        purse += (pot * amount);
    }
}
