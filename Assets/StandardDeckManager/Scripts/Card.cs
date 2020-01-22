using UnityEngine;

/// <summary>
/// Card
/// Description: Manages properties for an individual card.
/// </summary>

[System.Serializable]
public class Card
{
    #if UNITY_EDITOR
    // render these variables only in the unity editor
    [HideInInspector]
    public bool blnAutoAssign;      // auto assign gameobject based on selection

    public enum RedSuit
    {
        Diamonds,
        Hearts
    }
    [HideInInspector]
    public RedSuit redSuit;         // helps us limit the selection to only red suits

    public enum BlackSuit
    {
        Clubs,
        Spades
    }
    [HideInInspector]
    public BlackSuit blackSuit;     // helps us limit the selection to only black suits
    #endif

    // enum containing our different suits
    public enum Suit
    {
        Clubs,
        Diamonds,
        Spades,
        Hearts
    }

    // enum containing our different suit colors
    public enum Color
    {
        Black,
        Red
    }

    // enum containing our different card ranks
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
    public string name;          // define our card's name
    [SerializeField]
    public Suit suit;               // define the card's suit
    [SerializeField]
    public Color color;             // define the card's color
    [SerializeField]
    public Rank rank;               // define the card's rank
    [SerializeField]
    public int value;               // define the card's value
    [SerializeField]
    public GameObject card;         // define the card's gameobject
}