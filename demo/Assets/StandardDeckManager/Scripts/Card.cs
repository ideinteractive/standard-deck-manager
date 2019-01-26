using UnityEngine;

/// <summary>
/// Card
/// Description: Properties for each individual card.
/// </summary>

[System.Serializable]
public class Card
{
    // define our card name
    [SerializeField]
    public string strName;

#if UNITY_EDITOR
    // boolean to keep track of custom editor panel
    [SerializeField]
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
    [SerializeField]
    public Suit suit;

    // define the card's color
    public enum Color
    {
        Black,
        Red
    }
    [SerializeField]
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
    [SerializeField]
    public Rank rank;

    // define the card's value
    [SerializeField]
    public int value;

    // the card gameobject
    [SerializeField]
    public GameObject card;
}