using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// DeckManager
/// Description: Manages and handles a standard deck of 52 playing cards.
/// </summary>

[ExecuteInEditMode]
public class DeckManager : MonoBehaviour
{
    // public variables

    public List<Cards> deck = new List<Cards>();     // contains a list of cards
    public List<Cards> discardPile = new List<Cards>();

    #if UNITY_EDITOR
    [SerializeField]
    private bool initialized = false;

    // on awake
    void Awake() {
        // perform initialization
        Initialize();
    }

    // on initialization
    void Initialize()
    {
        // if initialization is true
        if (initialized)
            // return and break out of this function
            return;

        // create a new list of cards from the standard json template
        ListOfCards listOfCards = JsonUtility.FromJson<ListOfCards>(File.ReadAllText(Application.dataPath + "/Data/Standard52CardDesk.json"));

        // create a new deck based on that list
        deck = new List<Cards>(listOfCards.deck);

        // for each card in the deck assign a name
        foreach (Cards card in deck)
        {
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
        }

        // create a new discard pile
        discardPile = new List<Cards>();

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported ");

        initialized = true;
    }
    #endif

    // move a card up the deck
    public void MoveCardUp(Cards card)
    {

    }

    // move a card down the deck
    public void MoveCardDown(Cards card)
    {

    }

    // move a card to the top of the deck
    public void MoveCardToTop(Cards card)
    {

    }

    // move a card to the bottom of the deck
    public void MoveCardToBottom(Cards card)
    {

    }

    // discard a card to the discard pile
    public void DiscardCard(Cards card)
    {
        // add the card to the discard pille
        // and remove it from the deck
        discardPile.Add(card);
        deck.Remove(card);
    }

    // shuffle the deck
    public void Shuffle()
    {

    }

    // shuffle the deck with the discard pile
    public void ShuffleWithDiscard()
    {

    }

    // shuffle the discard pile and add it to the bottom of the deck
    public void ShuffleDiscard()
    {

    }
} 