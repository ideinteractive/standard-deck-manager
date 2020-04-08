using System;
using UnityEngine;

namespace StandardDeckManager.Scripts
{
    /// <summary>
    /// Card
    /// Description: Manages properties for an individual card.
    /// </summary>

    [System.Serializable]
    public class Card
    {
#if UNITY_EDITOR
        // render these variables only in the unity editor
        [HideInInspector] [SerializeField] public bool blnAutoAssign; // auto assign game object based on selection

        public enum RedSuit
        {
            Diamonds,
            Hearts
        }

        [HideInInspector] public RedSuit redSuit; // helps us limit the selection to only red suits

        public enum BlackSuit
        {
            Clubs,
            Spades
        }

        [HideInInspector] public BlackSuit blackSuit; // helps us limit the selection to only black suits
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

        [SerializeField] public string name; // define our card's name
        [SerializeField] public Suit suit; // define the card's suit
        [SerializeField] public Color color; // define the card's color
        [SerializeField] public Rank rank; // define the card's rank
        [SerializeField] public int value; // define the card's value
        [SerializeField] public GameObject card; // define the card's game object

        public Card()
        {
            // assign our values
            this.suit = Suit.Clubs;
            this.color = Color.Black;
            this.rank = Rank.Ace;
            this.value = 0;
        }
        
        public Card(Suit suit, Color color, Rank rank, int value)
        {
            // assign our values
            this.suit = suit;
            this.color = color;
            this.rank = rank;
            this.value = value;

            // based on our values assign the name
            this.name = color.ToString() + " " + rank.ToString() + " of " + suit.ToString();
            
            // based on our values assign the correct prefab
            this.card = Resources.Load("Prefabs/" + this.color + " " + this.rank + " " + this.suit) as GameObject;
        }
    }
}