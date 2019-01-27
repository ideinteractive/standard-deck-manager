using UnityEngine;

/// <summary>
/// Card
/// Description: Properties for each individual card.
/// </summary>

[System.Serializable]
public class Card
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Spades,
        Hearts
    }

    public enum Color
    {
        Black,
        Red
    }
    
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

    public string strName;          // define our card name
    public Suit suit;               // define the card's suit
    [SerializeField]
    public Color color;             // define the card's color
    public Rank rank;               // define the card's rank
    public int value;               // define the card's value
    public GameObject card;         // the card gameobject
}