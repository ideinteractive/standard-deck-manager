using UnityEngine;

/// <summary>
/// Card
/// Description: Properties for each individual card.
/// </summary>

[System.Serializable]
public class Card
{ 
    // define our card name
    public string strName;

#if UNITY_EDITOR
    // boolean to keep track of custom editor panel
    public bool blnExpandCardPnl;
# endif

    // define the card's suit
    public enum Suit
    {
        Clubs,
        Diamonds,
        Spades,
        Hearts
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

    // define the card's value
    public int value;

    // the card gameobject
    public GameObject card;
}