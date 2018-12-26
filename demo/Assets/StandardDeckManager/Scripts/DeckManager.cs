using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DeckManager
/// Description: Manages and handles a standard deck of 52 playing cards.
/// </summary>

public class DeckManager : MonoBehaviour
{
    // global variables
    public static DeckManager instance;     // set this object as a global object

    // public variables
    public List<Cards> deck = new List<Cards>();            // contains a list of cards
    public List<Cards> discardPile = new List<Cards>();     // contains a list of discarded cards
    public GameObject cardBackFace;                         // the back face of a card

    // boolean to keep track of custom editor panel
    public bool blnExpandDeckPnl;
    public bool blnExpandDiscardPnl;

    // private variables
    private GameObject m_goPool;   // object to nest our pooled objects

    // on awake
    private void Awake()
    {
        // set this object
        instance = this;

        // spawn a new empty gameobject to contain our pooled objects
        m_goPool = new GameObject();
        m_goPool.name = "Pool";
        m_goPool.transform.parent = this.transform;

        // for each object inside the deck 
        foreach (Cards card in deck)
        {
            GameObject goCard = Instantiate(card.card, this.transform.position, this.transform.rotation);
            goCard.SetActive(false);
            goCard.transform.parent = m_goPool.transform;
            card.card = goCard;
        }

        // spawn the back face card
        GameObject goBackFaceCard = Instantiate(cardBackFace, this.transform.position, this.transform.rotation);
        goBackFaceCard.SetActive(false);
        goBackFaceCard.transform.parent = m_goPool.transform;
        cardBackFace = goBackFaceCard;
    }

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
    public void AddNewCard(Cards card, List<Cards> deck)
    {
        // add the card to the deck
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

    // remove all playing cards and generate a new deck
    public void RemoveAllAndCreateNew()
    {
        // create a new deck 
        deck = new List<Cards>();

        // for each suit 
        for (int i = 0; i <= (int)Cards.Suit.Hearts; i++)
        {
            // if i is an odd number
            if (i % 2 == 1)
            {
                // for each card
                for (int c = 0; c <= (int)Cards.Rank.King; c++)
                {
                    // create a new card and add it to the deck
                    // add the card to the deck
                    Cards card = new Cards();
                    card.suit = (Cards.Suit)i;
                    card.color = (Cards.Color)1;
                    card.rank = (Cards.Rank)c;
                    card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
                    card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                    deck.Add(card);
                }
            }
            else
            {
                // for each card
                for (int c = 0; c <= (int)Cards.Rank.King; c++)
                {
                    // create a new card and add it to the deck
                    // add the card to the deck
                    Cards card = new Cards();
                    card.suit = (Cards.Suit)i;
                    card.color = (Cards.Color)0;
                    card.rank = (Cards.Rank)c;
                    card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
                    card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                    deck.Add(card);
                }
            }
        }

        // create a new discard pile
        discardPile = new List<Cards>();

        // assign the back face card
        cardBackFace = Resources.Load("Prefabs/Back Face") as GameObject;

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported ");
    }

    // return the deck count
    public int Count()
    {
        return deck.Count;
    }
} 