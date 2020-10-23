using System;

public class CardEventArgs : EventArgs
{
    public int CardIndex { get; private set; }

    public CardEventArgs(int cardIndex)
    {
        CardIndex = cardIndex;
    }
}

public delegate void CardEventHandler(object sender, CardEventArgs e);