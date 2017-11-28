using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

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
    public void MoveCardUp(Cards card, List<Cards> deck)
    {
        // get the current index of the card
        int index = deck.IndexOf(card);

        // if that index is not zeo
        if(index != 0)
        {
            // remove the current card from the deck
            deck.Remove(card);

            // add the card into the position
            deck.Insert(index - 1, card);
        }
    }

    // move a card down the deck
    public void MoveCardDown(Cards card, List<Cards> deck)
    {
        // get the current index of the card
        int index = deck.IndexOf(card);

        // if that index is not the max count of the deck
        if (index != (deck.Count - 1))
        {
            // remove the current card from the deck
            deck.Remove(card);

            // add the card into the position
            deck.Insert(index + 1, card);
        }
    }

    // move a card to the top of the deck
    public void MoveCardToTop(Cards card, List<Cards> deck)
    {
        // get the current index of the card
        int index = deck.IndexOf(card);

        // if that index is not zero
        if (index != 0)
        {
            // remove the current card from the deck
            deck.Remove(card);

            // add the card to the top of the deck
            deck.Insert(0, card);
        }
    }

    // move a card to the bottom of the deck
    public void MoveCardToBottom(Cards card, List<Cards> deck)
    {
        // get the current index of the card
        int index = deck.IndexOf(card);

        // if that index is not the max count of the deck
        if (index != (deck.Count - 1))
        {
            // remove the current card from the deck
            deck.Remove(card);

            // add the card to the bottom of the deck
            deck.Insert(deck.Count, card);
        }
    }

    // add card to the deck
    public void AddCard(Cards card)
    {
        // add the card to the deck
        // and remove it from the discard pile
        deck.Add(card);
        discardPile.Remove(card);
    }

    // discard a card to the discard pile
    public void DiscardCard(Cards card)
    {
        // add the card to the discard pille
        // and remove it from the deck
        discardPile.Add(card);
        deck.Remove(card);
    }

    // add a new card to the deck
    public void AddNewCard(List<Cards> deck)
    {
        // create a new card with default values
        // and add it to the deck
        Cards card = new Cards();
        card.strName = "Black Ace of Clubs";
        card.color = Cards.Color.Black;
        card.rank = Cards.Rank.Ace;
        card.suit = Cards.Suit.Clubs;
        deck.Add(card);
    }

    // remove a card from the deck
    public void RemoveCard(Cards card, List<Cards> deck)
    {
        // remove the card from the deck
        deck.Remove(card);
    }

    // shuffle the deck
    public void Shuffle()
    {
        // shuffle the deck
        RandomizeList(deck);
    }

    // shuffle the discard pile
    public void ShuffleDiscardPile()
    {
        // shuffle the discard pile
        RandomizeList(discardPile);
    }

    // shuffle the deck with the discard pile
    public void ShuffleWithDiscard()
    {
        // add the discard pile to the deck
        deck.AddRange(discardPile);

        // shuffle the deck
        RandomizeList(deck);

        // clear the discard pile
        discardPile = new List<Cards>();
    }

    // shuffle the discard pile and add it to the top of the deck
    public void ShuffleDiscardToTop()
    {
        // shuffle the discard pile
        RandomizeList(discardPile);

        // add the deck to the bottom of the discard pile
        discardPile.AddRange(deck);

        // clear the deck
        deck = new List<Cards>();

        // set the deck to the discard pile
        deck = discardPile;

        // clear the discard pile
        discardPile = new List<Cards>();
    }

    // shuffle the discard pile and add it to the bottom of the deck
    public void ShuffleDiscardToBottom()
    {
        // shuffle the discard pile
        RandomizeList(discardPile);

        // add it to the bottom of the deck
        deck.AddRange(discardPile);

        // clear the discard pile
        discardPile = new List<Cards>();
    }

    // randomize a list that has been passed through
    private static void RandomizeList(List<Cards> list)
    {
        // for each card in the list
        for (int i = 0; i < list.Count; i++)
        {
            // randomize the order
            Cards temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
} 