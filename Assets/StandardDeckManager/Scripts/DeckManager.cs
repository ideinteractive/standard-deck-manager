using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardDeckManager.Scripts
{
    /// <summary>
    /// DeckManager
    /// Description: Manages and handles all the functionality of our decks.
    /// </summary>

    public class DeckManager : MonoBehaviour
    {
        // global variables
        public static DeckManager instance; // set this object as a global object

        // public variables
        public List<Card> deck = new List<Card>(); // main deck that contains a list of cards
        public List<Card> discardPile = new List<Card>(); // contains a list of discarded cards
        public List<Card> inUsePile = new List<Card>(); // contains a list of cards in use

        [HideInInspector] public GameObject goPool; // game object to nest our pooled objects

#if UNITY_EDITOR
        // render these variables only in the unity editor
        public bool blnExpandDeckPnl; // checks if we should expand the deck panel
        public bool blnExpandDiscardPnl; // checks if we should expand the discard panel
        public bool blnExpandInUsePnl; // checks if we should expand the in use panel
#endif

        // on awake
        private void Awake()
        {
            // set this object's reference
            instance = this;

            // spawn a new empty game object to contain our pooled objects
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
            for (var i = 0; i < list.Count; i++)
            {
                // randomize the order
                var temp = list[i];
                var randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        // spawn all the cards inside a deck
        private void SpawnCardPool(IReadOnlyCollection<Card> fromDeck)
        {
            // if there are cards in the deck
            if (fromDeck.Count <= 0) return;
            // for each object inside the deck 
            foreach (var card in fromDeck)
            {
                var currentTransform = this.transform;
                var goCard = Instantiate(card.card, currentTransform.position, currentTransform.rotation);
                goCard.SetActive(false);
                goCard.transform.parent = goPool.transform;
                card.card = goCard;
                goCard.name = card.color + " " + card.rank + " of " + card.suit;
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
            otherDeck.AddRange(currentDeck);
            
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
        public void MoveAllCardToDeck(List<Card> fromDeck)
        {
            MoveAllCardsToADeck(fromDeck, deck);
        }

        // move all cards to  the in use pile
        public void MoveAllCardToInUse(List<Card> fromDeck)
        {
            MoveAllCardsToADeck(fromDeck, inUsePile);
        }

        // add a new card to the deck
        public void AddCard(Card card, List<Card> toDeck)
        {
            // if the application is playing
            if (Application.isPlaying)
            {
                // instantiate the object
                var currentTransform = this.transform;
                var goCard = Instantiate(card.card, currentTransform.position, currentTransform.rotation);
                goCard.SetActive(false);
                goCard.transform.parent = goPool.transform;
                goCard.name = card.color + " " + card.rank + " of " + card.suit;

                // assign the new card to the card object
                card.card = goCard;
            }

            // add the card to the deck
            toDeck.Add(card);
        }

        // remove a card from the deck
        public void RemoveCard(Card card, List<Card> fromDeck)
        {
            // if the application is playing
            if (Application.isPlaying)
                // delete the instantiated object
                Destroy(card.card);

            // remove the card from the deck
            fromDeck.Remove(card);
        }

        // move a card up its current slot in the deck
        public void MoveCardUpOneSlot(Card card, List<Card> inDeck)
        {
            // get the current index of the card
            var index = inDeck.IndexOf(card);

            // if that index is not zeo
            if (index == 0) return;
            // remove the current card from the deck
            inDeck.Remove(card);

            // add the card into the position
            inDeck.Insert(index - 1, card);
        }

        // move a card down its current slot in the deck
        public void MoveCardDownOneSlot(Card card, List<Card> inDeck)
        {
            // get the current index of the card
            var index = inDeck.IndexOf(card);

            // if that index is not the max count of the deck
            if (index == (inDeck.Count - 1)) return;
            // remove the current card from the deck
            inDeck.Remove(card);

            // add the card into the position
            inDeck.Insert(index + 1, card);
        }

        // move a card to the top of the deck
        public void MoveCardToTopDeck(Card card, List<Card> inDeck)
        {
            // get the current index of the card
            var index = inDeck.IndexOf(card);

            // if that index is not zero
            if (index == 0) return;
            // remove the current card from the deck
            inDeck.Remove(card);

            // add the card to the top of the deck
            inDeck.Insert(0, card);
        }

        // move a card to the bottom of the deck
        public void MoveCardToBottomDeck(Card card, List<Card> inDeck)
        {
            // get the current index of the card
            var index = inDeck.IndexOf(card);

            // if that index is not the max count of the deck
            if (index == (inDeck.Count - 1)) return;
            // remove the current card from the deck
            inDeck.Remove(card);

            // add the card to the bottom of the deck
            inDeck.Insert(inDeck.Count, card);
        }

        // return the count of a deck
        private static int Count(ICollection thisDeck)
        {
            return thisDeck.Count;
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

        // return count of all decks combined
        public int CountAllDecks()
        {
            return Count(deck) + Count(discardPile) + Count(inUsePile);
        }

        // delete all cards in a deck
        private static void DeleteCardsInDeck(ICollection<Card> inDeck)
        {
            // if we are in play mode
            if (Application.isPlaying)
            {
                // destroy the card objects
                foreach (var t in inDeck)
                {
                    Destroy(t.card);
                }
            }

            inDeck.Clear();
        }

        // remove all playing cards and generate a new deck
        public void RemoveAllAndCreateNew()
        {
            // remove all cards
            RemoveAll();

            // while the deck count is under 52
            while (deck.Count < 52)
            {
                // for each suit 
                for (var i = 0; i <= (int) Card.Suit.Hearts; i++)
                {
                    // if i is an odd number
                    if (i % 2 == 1)
                    {
                        // for each card
                        for (var c = 0; c <= (int) Card.Rank.King; c++)
                        {
                            // create a new card and add it to the deck
                            // add the card to the deck
                            var card = new Card((Card.Suit) i, (Card.Color) 1, (Card.Rank) c, c + 1);
                            deck.Add(card);
                        }
                    }
                    else
                    {
                        // for each card
                        for (var c = 0; c <= (int) Card.Rank.King; c++)
                        {
                            // create a new card and add it to the deck
                            // add the card to the deck
                            var card = new Card((Card.Suit) i, 0, (Card.Rank) c, c + 1);
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
}
