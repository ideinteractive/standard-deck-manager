using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager
/// Description: Manages and handles all functionality of the demo
/// </summary>

public class GameManager : MonoBehaviour
{
    // public variables
    [Header("Game Objects")]
    public GameObject goDealerCardBorder;       // where the dealer card spawns
    public GameObject goPlayerCardBorder;       // where the player card spawns

    [Header("UI Objects")]
    public Text txtDeckCount;                   // text object to track deck count
    public Text txtPlayerHandCount;             // text object to track player's hand
    public Text txtDealerHandCount;             // text object to track dealer's hand
    public Text txtPlayerScore;                 // text object to track player's score
    public Text txtDealerScore;                 // text object to track dealer's score
    public Text txtWinMessage;                  // text object to inform who won
    public Button btnHit;                       // hit button
    public Button btnStand;                     // stand button
    public Button btnReplay;                    // replay button
    public Button btnRestart;                   // restart button

    [Header("Other Settings")]
    public Vector3 vecCardSpawnOffset;          // how much offset when placing the cards next to each other

    [Header("Dealer and Player Hand")]
    public List<Cards> col_dealerHand;          // the dealer's hand
    public List<Cards> col_playerHand;          // the player's hand

    // private evariables
    private Vector3 m_vecDealerCardOffset;      // the offset of where the card object is displayed for the dealer
    private Vector3 m_vecPlayerCardOffset;      // the offset of where the card object is displayed for the player
    private int m_intPlayerScore;               // the player's score
    private int m_intDealerScore;               // the dealer's score

    // on initialization
    void Start()
    {
        // shuffle the deck of cards
        DeckManager.instance.Shuffle();

        // reset the spawn offset
        ResetSpawnOffset();

        // deal a new hand 
        DealNewHand();
    }

    // deal a new hand to the player and dealer
    private void DealNewHand()
    {
        // if there is less than 4 cards in the deck
        if (DeckManager.instance.Count() < 4)
        {
            // shuffle the discard pile into the deck before continuing 
            DeckManager.instance.ShuffleWithDiscard();
        }

        // create a new list for the dealer and player
        col_dealerHand = new List<Cards>();
        col_playerHand = new List<Cards>();

        // deal cards to both the dealer and player
        DealCard(col_dealerHand, goDealerCardBorder, false);
        DealCard(col_playerHand, goPlayerCardBorder, true);
        DealCard(col_dealerHand, goDealerCardBorder, false);
        DealCard(col_playerHand, goPlayerCardBorder, true);

        // update the deck count
        txtDeckCount.text = DeckManager.instance.Count().ToString();

        // display the current score of each hand
        CalculatePlayerHand();
        CalculateDealerInitialHand();
    }

    // deal a card
    private void DealCard(List<Cards> hand, GameObject slot, bool isPlayer)
    {
        // add the card to the hand and set the sorting order
        hand.Add(DeckManager.instance.deck[0]);
        DeckManager.instance.deck[0].card.GetComponent<SpriteRenderer>().sortingOrder = hand.Count;

        // if we are dealing the player's card
        if (isPlayer)
        {
            // spawn the card and increment the offset
            SpawnCardToSlot(slot, DeckManager.instance.deck[0].card, m_vecPlayerCardOffset);
            m_vecPlayerCardOffset += vecCardSpawnOffset;
        }
        else
        {
            // if the dealer has no cards in their hand
            if (hand.Count == 1)
            {
                // spawn the card and increment the offset
                SpawnCardToSlot(slot, DeckManager.instance.deck[0].card, m_vecDealerCardOffset);
                m_vecDealerCardOffset += vecCardSpawnOffset;
            }
            else
            {
                // spawn a backface card
                SpawnBackFaceCardToSlot(slot, DeckManager.instance.cardBackFace, m_vecDealerCardOffset);
                DeckManager.instance.cardBackFace.GetComponent<SpriteRenderer>().sortingOrder = hand.Count;
            }
        }

        DeckManager.instance.DiscardCard(DeckManager.instance.deck[0]);
    }

    // spawn back face card onto slot
    private void SpawnBackFaceCardToSlot(GameObject slot, GameObject card, Vector3 offset)
    {
        card.transform.position = slot.transform.position + offset;
        card.SetActive(true);
    }

    // spawn the card onto the slots
    private void SpawnCardToSlot(GameObject slot, GameObject card, Vector3 offset)
    {
        card.transform.position = slot.transform.position + offset;
        card.SetActive(true);
    }

    // calculate the player's hand
    private void CalculatePlayerHand()
    {
        // create an empty variable to hold our player's hand
        int t_intScore = 0;
        bool t_blnContainsAce = false;

        // for each card in the player's hand
        foreach (Cards card in col_playerHand)
        {
            // get the card value and add it to the score
            t_intScore += CardValue(card);

            // if the card is an ace flag it
            if (card.rank == Cards.Rank.Ace)
            {
                t_blnContainsAce = true;
            }
        }

        // if the hand contains an ace
        if (t_blnContainsAce)
        {
            //if the hand is not greater than 21 set the ace value to 11
            if ((t_intScore - 1) + 11 <= 21)
            {
                t_intScore = (t_intScore - 1) + 11;
            }
        }

        // output our score onto the ui object
        txtPlayerHandCount.text = t_intScore.ToString();

        // check if the player's hand is greater than 21
        if (int.Parse(txtPlayerHandCount.text) > 21)
        {
            // calculate the score so that the same is over
            Stand();
        }
    }

    // calculate the dealer's hand
    private void CalculateDealerHand()
    {
        // create an empty variable to hold our dealer's hand
        int t_intScore = 0;
        bool t_blnContainsAce = false;

        // for each card in the dealer's hand
        foreach (Cards card in col_dealerHand)
        {
            // get the card value and add it to the score
            t_intScore += CardValue(card);

            // if the card is an ace flag it
            if (card.rank == Cards.Rank.Ace)
            {
                t_blnContainsAce = true;
            }
        }

        // if the hand contains an ace
        if (t_blnContainsAce)
        {
            // check if the current hand is greater than 21 if the ace is an 11
            if ((t_intScore - 1) + 11 > 21)
            {
                return;
            }
            else
            {
                // else if the hand is not greater than 21 set the ace value to 11
                t_intScore = t_intScore - 1 + 11;
            }
        }

        // output our score onto the ui object
        txtDealerHandCount.text = t_intScore.ToString();
    }

    // display only the face up value of the dealer's hand
    private void CalculateDealerInitialHand()
    {
        // output our score onto the ui object
        txtDealerHandCount.text = CardValue(col_dealerHand[0]).ToString();
    }

    // return the card's value
    private int CardValue(Cards card)
    {
        // create a switch statement for each 
        // rank type and return the value
        switch (card.rank)
        {
            case Cards.Rank.Ace:
                return 1;
            case Cards.Rank.Two:
                return 2;
            case Cards.Rank.Three:
                return 3;
            case Cards.Rank.Four:
                return 4;
            case Cards.Rank.Five:
                return 5;
            case Cards.Rank.Six:
                return 6;
            case Cards.Rank.Seven:
                return 7;
            case Cards.Rank.Eight:
                return 8;
            case Cards.Rank.Nine:
                return 9;
            case Cards.Rank.Ten:
                return 10;
            case Cards.Rank.Jack:
                return 10;
            case Cards.Rank.Queen:
                return 10;
            case Cards.Rank.King:
                return 10;
            default:
                return 0;
        }
    }

    // reset the spawn offset
    private void ResetSpawnOffset()
    {
        m_vecDealerCardOffset = Vector3.zero;
        m_vecPlayerCardOffset = Vector3.zero;
    }

    // stand and reveal the hands to determine who wins
    public void Stand()
    {
        // spawn the dealer's second card onto the back face position
        col_dealerHand[1].card.transform.position = DeckManager.instance.cardBackFace.transform.position;
        col_dealerHand[1].card.GetComponent<SpriteRenderer>().sortingOrder = 2;
        col_dealerHand[1].card.SetActive(true);
        DeckManager.instance.cardBackFace.SetActive(false);

        // calculate the dealer's hand
        CalculateDealerHand();

        // reveal the score and who won
        SelectWinner();

        // hide the buttons and show replay button
        btnHit.gameObject.SetActive(false);
        btnStand.gameObject.SetActive(false);
        btnReplay.gameObject.SetActive(true);
        btnRestart.gameObject.SetActive(true);
    }

    // add a card to the player hand
    public void Hit()
    {
        // add a new card to the player hand 
        DealCard(col_playerHand, goPlayerCardBorder, true);

        // calculate the player's hand 
        CalculatePlayerHand();
    }

    // deal a new hand and start a new game
    public void Replay()
    {
        // hide the replay button and show the appropriate buttons
        btnHit.gameObject.SetActive(true);
        btnStand.gameObject.SetActive(true);
        btnReplay.gameObject.SetActive(false);
        btnRestart.gameObject.SetActive(false);

        // for each card in the player's hand
        foreach (Cards card in col_playerHand)
        {
            // hide the card
            card.card.SetActive(false);
        }

        // for each card in the dealer's hand
        foreach (Cards card in col_dealerHand)
        {
            // hide the card
            card.card.SetActive(false);
        }

        // reset the score
        txtDealerHandCount.text = "0";
        txtPlayerHandCount.text = "0";

        // remove the win message
        txtWinMessage.text = "";

        // reset the offsets
        ResetSpawnOffset();

        // deal a new hand to the player and dealer
        DealNewHand();
    }

    // reveal the score and who won
    private void SelectWinner()
    {
        // if the player score is 21 or less
        if (int.Parse(txtPlayerHandCount.text) <= 21)
        {
            // if the player score is higher than the dealer's
            if (int.Parse(txtPlayerHandCount.text) > int.Parse(txtDealerHandCount.text))
            {
                // show that the player won and increment the score
                txtWinMessage.text = "You have won!";
                m_intPlayerScore++;
                txtPlayerScore.text = "You: " + m_intPlayerScore;
            }
            else if (int.Parse(txtPlayerHandCount.text) == int.Parse(txtDealerHandCount.text))
            {
                // show the it is a draw
                txtWinMessage.text = "Draw!";
            }
            else
            {
                // show that the dealer won and increment the score
                txtWinMessage.text = "The Dealer has won!";
                m_intDealerScore++;
                txtDealerScore.text = "Dealer: " + m_intDealerScore;
            }
        }
        else
        {
            // show that the dealer won and increment the score
            txtWinMessage.text = "The Dealer has won!";
            m_intDealerScore++;
            txtDealerScore.text = "Dealer: " + m_intDealerScore;
        }
    }

    // restart the scene
    public void RestartGame()
    {
        // reload the current scene
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }
}