using System.Collections.Generic;

/// <summary>
/// Cards
/// Description: Properties for each individual card.
/// </summary>

[System.Serializable]
public class Cards
{
    // define our card name
    public string strName;

    // boolean to keep track of custom editor panel
    public bool blnExpandClipPnl;       

    // define the card's suit
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
    public Suit suit;

    // define the card's color
    public enum Color
    {
        Black,
        Red
    }
    public Color color;

    // define the card's rank
    public enum Rank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
    public Rank rank;
}