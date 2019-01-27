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
    public List<Card> deck = new List<Card>();            // contains a list of cards
    public List<Card> discardPile = new List<Card>();     // contains a list of discarded cards
    public List<Card> inUsePile = new List<Card>();       // contains a list of cards in use

    public GameObject cardBackFace;                         // the back face of a card

    // boolean to keep track of custom editor panel
    public bool blnExpandDeckPnl;
    public bool blnExpandDiscardPnl;
    public bool blnExpandInUsePnl;

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
        foreach (Card card in deck)
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
    public void MoveCardUp(Card card, List<Card> deck)
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
    public void MoveCardDown(Card card, List<Card> deck)
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
    public void MoveCardToTop(Card card, List<Card> deck)
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
    public void MoveCardToBottom(Card card, List<Card> deck)
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



    // add a new card to the deck
    public void AddCard(Card card, List<Card> deck)
    {
        // add the card to the deck
        deck.Add(card);
    }

    // remove a card from the deck
    public void RemoveCard(Card card, List<Card> deck)
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

    // shuffle the in use pile
    public void ShuffleInUsePile()
    {
        // shuffle the in use pile
        RandomizeList(inUsePile);
    }

    // shuffle two different decks together
    public void ShuffleDecks(List<Card> mainDeck, List<Card> deckToShuffle)
    {
        // add the deck to shuffle to the main deck
        mainDeck.AddRange(deckToShuffle);

        // shuffle the deck
        RandomizeList(mainDeck);

        // clear the deck to shuffle pile
        deckToShuffle.Clear();
    }

    // shuffle a choosen deck and add it to the top of a deck
    public void ShuffleToTop(List<Card> mainDeck, List<Card> deckToShuffle)
    {
        // shuffle the deck to shuffle
        RandomizeList(deckToShuffle);

        // add the main deck to the bottom of the deck to shuffle
        deckToShuffle.AddRange(mainDeck);

        // clear the main deck
        mainDeck = deckToShuffle;

        // set the main deck to the deck to shuffle
        mainDeck = deckToShuffle;

        // clear the deck to shuffle
        deckToShuffle.Clear();
    }

    // shuffle a choosen deck and add it to the bottom of a deck
    public void ShuffleToBottom(List<Card> mainDeck, List<Card> deckToShuffle)
    {
        // shuffle the deck to shuffle
        RandomizeList(deckToShuffle);

        // add it to the bottom of the main deck
        mainDeck.AddRange(deckToShuffle);

        // clear the deck to shuffle
        deckToShuffle.Clear();
    }

    // randomize a list that has been passed through
    private static void RandomizeList(List<Card> list)
    {
        // for each card in the list
        for (int i = 0; i < list.Count; i++)
        {
            // randomize the order
            Card temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    

    // return the deck count
    public int Count()
    {
        return deck.Count;
    }

    // return the deck count of a given deck
    public int Count(List<Card> deck)
    {
        return deck.Count;
    }
    // remove cards from in use pile to discard pile
    public void DiscardInUse()
    {
        // for each card in the in use pile
        foreach (Card card in inUsePile)
        {
            discardPile.Add(card);
            inUsePile.Remove(card);
        }
    }





    // move a card to another deck
    private void MoveCardToADeck(Card card, List<Card> fromDeck, List<Card> toDeck)
    {
        // move a card from the from deck to the to deck
        toDeck.Add(card);
        fromDeck.Remove(card);
    }

    // move a card to the discard pile
    public void MoveToDiscard(Card card, List<Card> fromDeck)
    {
        MoveCardToADeck(card, fromDeck, discardPile);
    }

    // move a card to the deck
    public void MoveToDeck(Card card, List<Card> fromDeck)
    {
        MoveCardToADeck(card, fromDeck, deck);
    }

    // move a card to the in use pile
    public void MoveToInUse(Card card, List<Card> fromDeck)
    {
        MoveCardToADeck(card, fromDeck, inUsePile);
    }

    // remove all playing cards and generate a new deck
    public void RemoveAllAndCreateNew()
    {
        // create a new deck 
        deck = new List<Card>();

        // for each suit 
        for (int i = 0; i <= (int)Card.Suit.Hearts; i++)
        {
            // if i is an odd number
            if (i % 2 == 1)
            {
                // for each card
                for (int c = 0; c <= (int)Card.Rank.King; c++)
                {
                    // create a new card and add it to the deck
                    // add the card to the deck
                    Card card = new Card();
                    card.suit = (Card.Suit)i;
                    card.color = (Card.Color)1;
                    card.rank = (Card.Rank)c;
                    card.value = c + 1;
                    card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
                    card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                    deck.Add(card);
                }
            }
            else
            {
                // for each card
                for (int c = 0; c <= (int)Card.Rank.King; c++)
                {
                    // create a new card and add it to the deck
                    // add the card to the deck
                    Card card = new Card();
                    card.suit = (Card.Suit)i;
                    card.color = (Card.Color)0;
                    card.rank = (Card.Rank)c;
                    card.value = c + 1;
                    card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
                    card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                    deck.Add(card);
                }
            }
        }

        // create a new discard pile
        discardPile = new List<Card>();

        // create a new in use pile
        inUsePile = new List<Card>();

        // assign the back face card
        cardBackFace = Resources.Load("Prefabs/Back Face") as GameObject;

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported");
    }

    // remove all playing cards
    public void RemoveAll()
    {
        // remove all cards 
        deck = new List<Card>();
        discardPile = new List<Card>();
        inUsePile = new List<Card>();

        // assign the back face card
        cardBackFace = Resources.Load("Prefabs/Back Face") as GameObject;

        // inform the user the deck has been updated
        Debug.Log("All Card Removed");
    }




}