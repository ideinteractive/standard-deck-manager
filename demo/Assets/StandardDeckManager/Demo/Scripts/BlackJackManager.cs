﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// BlackJackManager
/// Description: Manages and handles all functionality of the black jack demo
/// </summary>

public class BlackJackManager : MonoBehaviour
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
    public List<Card> col_dealerHand;           // the dealer's hand
    public List<Card> col_playerHand;           // the player's hand

    [Header("Sound Effects")]
    public AudioSource audSrc;                  // the audio source to play sounds from
    public AudioClip audClpCardSlide;           // audio clip for dealing a card

    // private evariables
    private Vector3 m_vecDealerCardOffset;      // the offset of where the card object is displayed for the dealer
    private Vector3 m_vecPlayerCardOffset;      // the offset of where the card object is displayed for the player
    private int m_intPlayerScore;               // the player's score
    private int m_intDealerScore;               // the dealer's score

    // on initialization
    void Start()
    {
        // set up our card values below
        // for each card in the deck
        for (int i = 0; i < DeckManager.instance.deck.Count; i++)
        {
            SetCardValue(DeckManager.instance.deck[i]);
        }

        // for each card in the discard
        for (int i = 0; i < DeckManager.instance.discardPile.Count; i++)
        {
            SetCardValue(DeckManager.instance.discardPile[i]);
        }

        // for each card in the in use pile
        for (int i = 0; i < DeckManager.instance.inUsePile.Count; i++)
        {
            SetCardValue(DeckManager.instance.inUsePile[i]);
        }

        // shuffle the deck of cards
        DeckManager.instance.Shuffle();

        // reset the spawn offset
        ResetSpawnOffset();

        // deal a new hand 
        StartCoroutine(DealNewHand());

        // reset our buttons
        btnHit.gameObject.SetActive(false);
        btnStand.gameObject.SetActive(false);
        btnReplay.gameObject.SetActive(false);
        btnRestart.gameObject.SetActive(false);
    }

    // set the card values
    private void SetCardValue(Card card)
    {
        // create a switch statement for each 
        // rank type and return the value
        switch (card.rank)
        {
            case Card.Rank.Ace:
                card.value = 1;
                break;
            case Card.Rank.Two:
                card.value = 2;
                break;
            case Card.Rank.Three:
                card.value = 3;
                break;
            case Card.Rank.Four:
                card.value = 4;
                break;
            case Card.Rank.Five:
                card.value = 5;
                break;
            case Card.Rank.Six:
                card.value = 6;
                break;
            case Card.Rank.Seven:
                card.value = 7;
                break;
            case Card.Rank.Eight:
                card.value = 8;
                break;
            case Card.Rank.Nine:
                card.value = 9;
                break;
            case Card.Rank.Ten:
                card.value = 10;
                break;
            case Card.Rank.Jack:
                card.value = 10;
                break;
            case Card.Rank.Queen:
                card.value = 10;
                break;
            case Card.Rank.King:
                card.value = 10;
                break;
            default:
                card.value = 0;
                break;
        }
    }

    // deal a new hand to the player and dealer
    private IEnumerator DealNewHand()
    {
        // if there are cards in the in use pile
        while (DeckManager.instance.inUsePile.Count > 0)
        {
            // put them in the discard pile
            DeckManager.instance.DiscardCard(DeckManager.instance.inUsePile[0], DeckManager.instance.inUsePile);
        }

        // if there is less than 4 cards in the deck
        if (DeckManager.instance.Count() < 4)
        {
            // shuffle the discard pile into the deck before continuing 
            DeckManager.instance.ShuffleDecks(DeckManager.instance.deck, DeckManager.instance.discardPile);
        }

        // create a new list for the dealer and player
        col_dealerHand = new List<Card>();
        col_playerHand = new List<Card>();

        // assign the correct sfx
        if (audSrc.clip != audClpCardSlide)
            audSrc.clip = audClpCardSlide;
        
        // deal cards to both the dealer and player
        DealCard(col_dealerHand, goDealerCardBorder, false);
        audSrc.Play();
        yield return new WaitForSeconds(0.5f);
        DealCard(col_playerHand, goPlayerCardBorder, true);
        audSrc.Play();
        yield return new WaitForSeconds(0.5f);
        DealCard(col_dealerHand, goDealerCardBorder, false);
        audSrc.Play();
        yield return new WaitForSeconds(0.5f);
        DealCard(col_playerHand, goPlayerCardBorder, true);
        audSrc.Play();

        // turn on our buttons
        btnHit.gameObject.SetActive(true);
        btnStand.gameObject.SetActive(true);

        // update the deck count
        txtDeckCount.text = DeckManager.instance.Count().ToString();

        // display the current score of each hand
        CalculateHand(col_playerHand, txtPlayerHandCount);
        CalculateDealerInitialHand();
    }

    // deal a card
    private void DealCard(List<Card> hand, GameObject slot, bool isPlayer)
    {
        // if there is less than 4 cards in the deck
        if (DeckManager.instance.Count() < 4)
        {
            // shuffle the discard pile into the deck before continuing 
            DeckManager.instance.ShuffleDecks(DeckManager.instance.deck, DeckManager.instance.discardPile);
        }

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

        // move the current card in the deck manager to the in use pile
        DeckManager.instance.MoveToInUse(DeckManager.instance.deck[0], DeckManager.instance.deck);
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

    // calculate the current hand
    private void CalculateHand(List<Card> hand, Text textOutput)
    {
        // create variables to reference the hand's cards
        int t_intScore = 0;
        bool t_blnContainsAce = false;

        // for each card in the hand
        foreach (Card card in hand)
        {
            // get the card value and add it to the score
            t_intScore += card.value;

            // if the card is an ace flag it
            if (card.rank == Card.Rank.Ace)
            {
                t_blnContainsAce = true;
            }
        }

        // if the hand contains an ace
        if (t_blnContainsAce)
        {
            // if the hand is not greater than 21 set the ace value to 11
            if ((t_intScore - 1) + 11 <= 21)
            {
                t_intScore = (t_intScore - 1) + 11;
            }
        }

        // output our score onto the ui object
        textOutput.text = t_intScore.ToString();  
    }

    // display only the face up value of the dealer's hand
    private void CalculateDealerInitialHand()
    {
        // output our score onto the ui object
        txtDealerHandCount.text = col_dealerHand[0].value.ToString();
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
        CalculateHand(col_dealerHand, txtDealerHandCount);

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
        // assign the correct sfx
        if (audSrc.clip != audClpCardSlide)
            audSrc.clip = audClpCardSlide;

        audSrc.Play();

        // add a new card to the player hand 
        DealCard(col_playerHand, goPlayerCardBorder, true);

        // calculate the player's hand 
        CalculateHand(col_playerHand, txtPlayerHandCount);

        // check if the player's hand is greater than 21
        if (int.Parse(txtPlayerHandCount.text) > 21)
        {
            // calculate the score so that the game is over
            Stand();
        }
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
        foreach (Card card in col_playerHand)
        {
            // hide the card
            card.card.SetActive(false);
        }

        // for each card in the dealer's hand
        foreach (Card card in col_dealerHand)
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
        StartCoroutine(DealNewHand());
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