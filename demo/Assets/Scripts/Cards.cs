using System.Collections.Generic;

/// <summary>
/// Cards
/// Description: Properties for each individual card.
/// </summary>

[System.Serializable]
public class Cards
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
    public Suit suit;

    public enum Color
    {
        Black,
        Red
    }
    public Color color;

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

/// <summary>
/// ListOfCards
/// Description: Contains a list of cards.
/// </summary>

[System.Serializable]
public class ListOfCards
{
    public List<Cards> deck;
}