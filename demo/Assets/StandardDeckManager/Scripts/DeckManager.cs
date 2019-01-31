using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DeckManager
/// Description: Manages and handles all the functionality of our decks.
/// </summary>

public class DeckManager : MonoBehaviour
{
    // global variables
    public static DeckManager Instance;     // set this object as a global object

    // public variables
    public List<Card> deck = new List<Card>();         // contains a list of cards
    public List<Card> discardPile = new List<Card>();         // contains a list of discarded cards
    public List<Card> inUsePile = new List<Card>();         // contains a list of cards in use

    [HideInInspector]
    public GameObject goPool;                                       // object to nest our pooled objects

#if UNITY_EDITOR
    // boolean to keep track of custom editor panel
    public bool blnExpandDeckPnl;
    public bool blnExpandDiscardPnl;
    public bool blnExpandInUsePnl;
#endif

    // on awake
    private void Awake()
    {
        // set this object's reference
        Instance = this;

        // spawn a new empty gameobject to contain our pooled objects
        goPool = new GameObject
        {
            name = "Pool"
        };
        goPool.transform.parent = this.transform;

        // spawn our cards
        SpawnCardPool(deck);
        SpawnCardPool(discardPile);
        SpawnCardPool(inUsePile);
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

    // spawn all the cards inside a deck
    private void SpawnCardPool(List<Card> deck)
    {
        // if there are cards in the deck
        if (deck.Count > 0)
        {
            // for each object inside the deck 
            foreach (Card card in deck)
            {
                GameObject goCard = Instantiate(card.card, this.transform.position, this.transform.rotation);
                goCard.SetActive(false);
                goCard.transform.parent = goPool.transform;
                card.card = goCard;
                goCard.name = card.color + " " + card.rank + " of " + card.suit;
            }
        }
    }

    // shuffle the deck
    public void ShuffleDeck()
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

    // shuffle all the decks together 
    public void ShuffleAllDecksTogether()
    {
        ShuffleDecksTogether(deck, discardPile);
        ShuffleDecksTogether(deck, inUsePile);
    }

    // shuffle all the decks
    public void ShuffleAllDecks()
    {
        ShuffleDeck();
        ShuffleDiscardPile();
        ShuffleInUsePile();
    }

    // shuffle two different decks together
    public void ShuffleDecksTogether(List<Card> currentDeck, List<Card> withDeck)
    {
        // add the deck we are shuffling with into the current deck
        currentDeck.AddRange(withDeck);

        // shuffle the deck
        RandomizeList(currentDeck);

        // clear the deck we were shuffling with
        withDeck.Clear();
    }

    // shuffle a deck to the top of another deck
    public void ShuffleToTopOfDeck(List<Card> currentDeck, List<Card> otherDeck)
    {
        // shuffle the current deck
        RandomizeList(currentDeck);

        // add the other deck to the bottom of the current deck
        currentDeck.AddRange(otherDeck);

        // set the other deck to the current one
        if (otherDeck == deck)
            deck = new List<Card>(currentDeck);
        else if (otherDeck == discardPile)
            discardPile = new List<Card>(currentDeck);
        else if (otherDeck == inUsePile)
            inUsePile = new List<Card>(currentDeck);
        
        // clear the current deck
        currentDeck.Clear();
    }

    // shuffle a deck to the bottom of another deck
    public void ShuffleToBottomOfDeck(List<Card> currentDeck, List<Card> otherDeck)
    {
        // shuffle the current deck
        RandomizeList(currentDeck);

        // add the current deck to the bottom of the other deck
        otherDeck.AddRange(currentDeck);

        // clear the current deck
        currentDeck.Clear();
    }

    // move a card to another deck
    private void MoveCardToADeck(Card card, List<Card> fromDeck, List<Card> toDeck)
    {
        // move a card from the from deck to the to deck
        toDeck.Add(card);
        fromDeck.Remove(card);
    }

    // move a card to the discard pile
    public void MoveCardToDiscard(Card card, List<Card> fromDeck)
    {
        MoveCardToADeck(card, fromDeck, discardPile);
    }

    // move a card to the deck
    public void MoveCardToDeck(Card card, List<Card> fromDeck)
    {
        MoveCardToADeck(card, fromDeck, deck);
    }

    // move a card to the in use pile
    public void MoveCardToInUse(Card card, List<Card> fromDeck)
    {
        MoveCardToADeck(card, fromDeck, inUsePile);
    }

    // move all cards to another deck
    private void MoveAllCardsToADeck(List<Card> fromDeck, List<Card> toDeck)
    {
        // for each card in the from deck
        foreach (Card card in fromDeck)
        {
            // add it to the to deck
            toDeck.Add(card);
        }

        // remove all cards from the from deck
        fromDeck.Clear();
    }

    // move all cards to  the discard pile
    public void MoveAllCardToDiscard(List<Card> fromDeck)
    {
        MoveAllCardsToADeck(fromDeck, discardPile);
    }

    // move all cards to  the deck
    public void MoveAllCardToDeck(Card card, List<Card> fromDeck)
    {
        MoveAllCardsToADeck(fromDeck, deck);
    }

    // move all cards to  the in use pile
    public void MoveAllCardToInUse(Card card, List<Card> fromDeck)
    {
        MoveAllCardsToADeck(fromDeck, inUsePile);
    }

    // add a new card to the deck
    public void AddCard(Card card, List<Card> deck)
    {
        // if the application is playing
        if (Application.isPlaying)
        {
            // instantiate the object
            GameObject goCard = Instantiate(card.card, this.transform.position, this.transform.rotation);
            goCard.SetActive(false);
            goCard.transform.parent = goPool.transform;
            goCard.name = card.color + " " + card.rank + " of " + card.suit;

            // assign the new card to the card object
            card.card = goCard;
        }

        // add the card to the deck
        deck.Add(card);
    }

    // remove a card from the deck
    public void RemoveCard(Card card, List<Card> deck)
    {
        // if the application is playing
        if (Application.isPlaying)
            // delete the instantiated object
            Destroy(card.card);

        // remove the card from the deck
        deck.Remove(card);
    }

    // move a card up its current slot in the deck
    public void MoveCardUpOneSlot(Card card, List<Card> deck)
    {
        // get the current index of the card
        int index = deck.IndexOf(card);

        // if that index is not zeo
        if (index != 0)
        {
            // remove the current card from the deck
            deck.Remove(card);

            // add the card into the position
            deck.Insert(index - 1, card);
        }
    }

    // move a card down its current slot in the deck
    public void MoveCardDownOneSlot(Card card, List<Card> deck)
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
    public void MoveCardToTopDeck(Card card, List<Card> deck)
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
    public void MoveCardToBottomDeck(Card card, List<Card> deck)
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

    // return the count of a deck
    private int Count(List<Card> deck)
    {
        return deck.Count;
    }

    // return count of deck
    public int CountDeck()
    {
        return Count(deck);
    }

    // return count of discard pile
    public int CountDiscardPile()
    {
        return Count(discardPile);
    }

    // return count of in use pile
    public int CountInUsePile()
    {
        return Count(inUsePile);
    }

    // return coutn of all decks combined
    public int CountAllDecks()
    {
        return Count(deck) + Count(discardPile) + Count(inUsePile);
    }

    // delete all cards in a deck
    private void DeleteCardsInDeck(List<Card> deck)
    {
        // if we are in play mode
        if (Application.isPlaying)
        {
            // destroy the card objects
            for (int i = 0; i < deck.Count; i++)
            {
                Destroy(deck[i].card);
            }
        }

        deck.Clear();
    }

    // remove all playing cards and generate a new deck
    public void RemoveAllAndCreateNew()
    {
        // create a new deck 
        DeleteCardsInDeck(deck);

        // while the deck count is under 52
        while (deck.Count < 52)
        {
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
                        Card card = new Card
                        {
                            suit = (Card.Suit)i,
                            color = (Card.Color)1,
                            rank = (Card.Rank)c,
                            value = c + 1
                        };
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
                        Card card = new Card
                        {
                            suit = (Card.Suit)i,
                            color = (Card.Color)0,
                            rank = (Card.Rank)c,
                            value = c + 1
                        };
                        card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
                        card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                        deck.Add(card);
                    }
                }
            }
        }

        // if we are in play mode
        if (Application.isPlaying)
        {
            // spawn our cards
            SpawnCardPool(deck);
        }

        // create a new discard pile
        DeleteCardsInDeck(discardPile);

        // create a new in use pile
        DeleteCardsInDeck(inUsePile);

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported");
    }

    // remove all playing cards
    public void RemoveAll()
    {
        // remove all cards 
        DeleteCardsInDeck(deck);
        DeleteCardsInDeck(discardPile);
        DeleteCardsInDeck(inUsePile);

        // inform the user the deck has been updated
        Debug.Log("All Card Removed");
    }
}

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