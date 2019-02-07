using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// BlackjackManager
/// Description: Manages and handles all functionality of the blackjack demo
/// </summary>

public class BlackjackManager : MonoBehaviour
{
    #region Variables
    // public variables
    [Header("Game Objects")]
    public GameObject goDealerCardBorder;               // where the dealer card spawns
    public GameObject goPlayerCardBorder;               // where the player card spawns
    public GameObject goCardBackFace;                   // the card back face object

    [Header("UI Objects")]
    public Text txtDeckCount;                           // text object to track deck count
    public Text txtPlayerHandCount;                     // text object to track player's hand
    public Text txtDealerHandCount;                     // text object to track dealer's hand
    public Text txtPlayerScore;                         // text object to track player's score
    public Text txtDealerScore;                         // text object to track dealer's score
    public Text txtWinMessage;                          // text object to inform who won
    public Button btnHit;                               // hit button
    public Button btnStand;                             // stand button
    public Button btnPlayAgain;                         // replay button
    public Button btnMainMenu;                          // main menu button

    [Header("Other Settings")]
    public float fltWaitTimeAfterShuffle = 0.7f;        // the wait time after the deck is shuffled
    public float fltWaitTimeBeforeDeal = 0.5f;          // the wait time between when a dealer hits  
    public float fltWaitTimeBeforeResults = 0.5f;       // the wait time before the winner is determined
    public float fltWaitTimeAfterHitButton = 0.6f;      // the wait time for the button to appear again after hitting hit
    public Vector3 vecCardSpawnOffset;                  // how much offset when placing the cards next to each other

    [Header("Sound Effects")]
    public AudioSource audSrc;                          // the audio source to play sounds from
    public AudioClip audClpCardSlide;                   // audio clip for dealing a card
    public AudioClip audClpCardShuffle;                 // audio clip for shuffling the deck
    public AudioClip audClpWin;                         // audio clip for win state
    public AudioClip audClpLose;                        // audio clip for lose state
    public AudioClip audClpBlackjack;                   // audio clip for blackjack state
    public AudioClip audClpDraw;                        // audio clip for draw state

    [Header("Volume Levels")]
    public float fltCardSlideVolume = 0.5f;             // the volume for card slide  
    public float fltCardShuffleVolume = 0.5f;           // the volume for card shuffling   
    public float fltWinVolume = 0.5f;                   // the volume for our win sound
    public float fltLoseVolume = 0.5f;                  // the volume for our lose sound
    public float fltBlackjackVolume = 0.5f;             // the volume for our blackjack soud
    public float fltDrawVolume = 0.5f;                  // the volume for our draw sound

    // private evariables
    private List<Card> m_col_dealerHand;                // the dealer's hand
    private List<Card> m_col_playerHand;                // the player's hand
    private Vector3 m_vecDealerCardOffset;              // the offset of where the card object is displayed for the dealer
    private Vector3 m_vecPlayerCardOffset;              // the offset of where the card object is displayed for the player
    private int m_intPlayerScore;                       // the player's score
    private int m_intDealerScore;                       // the dealer's score
    private bool m_blnActionInProgress;                 // check if the player performed an action already
    #endregion

    // on initialization
    private void Start()
    {
        // if the audio source is null
        if (audSrc == null)
        {
            // set it from this component
            audSrc = this.GetComponent<AudioSource>();
        }

        // spawn our back face object
        GameObject cardBackFace = Instantiate(goCardBackFace);
        cardBackFace.SetActive(false);
        cardBackFace.name = "Card Back Face";

        // set the card back face to the spawned object
        goCardBackFace = cardBackFace;

        // reset our button states
        btnHit.gameObject.SetActive(false);
        btnStand.gameObject.SetActive(false);
        btnPlayAgain.gameObject.SetActive(false);
        btnMainMenu.gameObject.SetActive(false);
         
        // update the deck count
        txtDeckCount.text = DeckManager.Instance.CountDeck().ToString();

        // initialize the game
        StartCoroutine(InitializeGame());
    }

    #region Game Functionality
    // set up the deck
    private void SetUpDeck(List<Card> deck)
    {
        // for each card in the deck
        int i = 0;
        while (i < deck.Count)
        {
            // set up its value
            SetCardValue(deck[i]);
            i++;
        }
    }

    // sets the value for specific cards
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

    // initializes the game and handles the setup
    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(0.2f);

        // set up each deck's card value
        SetUpDeck(DeckManager.Instance.deck);
        SetUpDeck(DeckManager.Instance.discardPile);
        SetUpDeck(DeckManager.Instance.inUsePile);

        // play our shuffle sfx
        AssignAudioClip(audClpCardShuffle);
        audSrc.Play();

        // shuffle the deck of cards
        DeckManager.Instance.ShuffleDeck();

        yield return new WaitForSeconds(fltWaitTimeAfterShuffle);

        // reset the spawn offset
        ResetSpawnOffset();

        // deal a new hand 
        StartCoroutine(DealNewHand());
    }

    // deal a new hand to the player and dealer
    private IEnumerator DealNewHand()
    {
        // reset our variables
        m_blnActionInProgress = false;

        // if there are cards in the in use pile
        if (DeckManager.Instance.CountInUsePile() > 0)
            // put them in the discard pile
            DeckManager.Instance.MoveAllCardToDiscard(DeckManager.Instance.inUsePile);

        // check if the discard pile should 
        // be shuffled back into the main deck
        if (CheckForShuffle())
            yield return new WaitForSeconds(fltWaitTimeAfterShuffle);
      
        // create a new list for the dealer and player
        m_col_dealerHand = new List<Card>();
        m_col_playerHand = new List<Card>();
       
        // for 4 loops
        for (int i = 0; i < 4; i++)
        {
            // infom the manager an action is in progress
            m_blnActionInProgress = true;

            // deal cards to both the dealer and player
            if (i % 2 == 0)
            {
                StartCoroutine(DealCard(m_col_dealerHand, goDealerCardBorder, false));

                // while an action is in progress wait until it is complete
                while (m_blnActionInProgress)
                {
                    yield return null;
                }

                // if this is the first deal
                if (i == 0)
                {
                    // display the current score of the dealer
                    CalculateDealerInitialHand();
                }

                yield return new WaitForSeconds(fltWaitTimeBeforeDeal);
            }
            else
            {
                StartCoroutine(DealCard(m_col_playerHand, goPlayerCardBorder, true));

                // while an action is in progress wait until it is complete
                while (m_blnActionInProgress)
                {
                    yield return null;
                }

                // display the current score of the player
                CalculateHand(m_col_playerHand, txtPlayerHandCount);

                yield return new WaitForSeconds(fltWaitTimeBeforeDeal);
            }
        }

        // turn on our buttons
        btnHit.gameObject.SetActive(true);
        btnStand.gameObject.SetActive(true);
    }

    // deal a card
    private IEnumerator DealCard(List<Card> hand, GameObject slot, bool isPlayer)
    {
        // check if the discard pile should 
        // be shuffled back into the main deck
        if (CheckForShuffle())
        {
            yield return new WaitForSeconds(fltWaitTimeAfterShuffle);
        }

        // add the card to the hand and set the sorting order
        hand.Add(DeckManager.Instance.deck[0]);
        DeckManager.Instance.deck[0].card.GetComponent<SpriteRenderer>().sortingOrder = hand.Count;

        // if we are dealing the player's card
        if (isPlayer)
        {
            // spawn the card and increment the offset
            SpawnCardToSlot(slot, DeckManager.Instance.deck[0].card, m_vecPlayerCardOffset);
            m_vecPlayerCardOffset += vecCardSpawnOffset;
        }
        else
        {
            // if the dealer has no cards in their hand
            if (hand.Count == 1)
            {
                // spawn the card and increment the offset
                SpawnCardToSlot(slot, DeckManager.Instance.deck[0].card, m_vecDealerCardOffset);
                m_vecDealerCardOffset += vecCardSpawnOffset;
            }
            else if (hand.Count == 2)
            {
                // spawn a backface card
                SpawnBackFaceCardToSlot(slot, goCardBackFace, m_vecDealerCardOffset);
                goCardBackFace.GetComponent<SpriteRenderer>().sortingOrder = hand.Count;
                m_vecDealerCardOffset += vecCardSpawnOffset;
            }
            else
            {
                SpawnCardToSlot(slot, DeckManager.Instance.deck[0].card, m_vecDealerCardOffset);
                m_vecDealerCardOffset += vecCardSpawnOffset;
            }
        }

        // assign the card sliding sfx
        AssignAudioClip(audClpCardSlide);
        audSrc.Play();

        // move the current card in the deck manager to the in use pile
        DeckManager.Instance.MoveCardToInUse(DeckManager.Instance.deck[0], DeckManager.Instance.deck);

        // update the deck count
        txtDeckCount.text = DeckManager.Instance.CountDeck().ToString();

        // set action in progress to false
        // to allow the game to continue
        m_blnActionInProgress = false;
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
        int i = 0;
        while (i < hand.Count)
        {
            // get the card value and add it to the score
            t_intScore += hand[i].value;

            // if the card is an ace flag it
            if (hand[i].rank == Card.Rank.Ace)
            {
                t_blnContainsAce = true;
            }
            i++;
        }

        // if the hand contains an ace
        if (t_blnContainsAce)
        {

            // if the hand is not greater than 21 set the ace value to 11
            if ((t_intScore - 1) + 11 <= 21)
            {
                t_intScore = (t_intScore - 1) + 11;

                Debug.Log("Run");
                if ((t_intScore - 1) + 11 == 21)
                {
                    StartCoroutine(Stand());
                    Debug.Log("21");
                }
            }
        }

        // output our score onto the ui object
        textOutput.text = t_intScore.ToString();
    }

    // display only the face up value of the dealer's hand
    private void CalculateDealerInitialHand()
    {
        // output our score onto the ui object
        txtDealerHandCount.text = m_col_dealerHand[0].value.ToString();
    }

    // stand and reveal the hands to determine who wins
    private IEnumerator Stand()
    {
        // hide the buttons
        btnHit.gameObject.SetActive(false);
        btnStand.gameObject.SetActive(false);

        // assign the card sliding sfx
        AssignAudioClip(audClpCardSlide);

        // spawn the dealer's second card onto the back face position
        m_col_dealerHand[1].card.transform.position = goCardBackFace.transform.position;
        m_col_dealerHand[1].card.GetComponent<SpriteRenderer>().sortingOrder = 2;
        m_col_dealerHand[1].card.SetActive(true);
        goCardBackFace.SetActive(false);

        // play the sfx
        audSrc.Play();

        // calculate the dealer's hand
        CalculateHand(m_col_dealerHand, txtDealerHandCount);

        yield return new WaitForSeconds(fltWaitTimeBeforeResults);

        // if the player's hand is less than or equal to 21
        if (int.Parse(txtPlayerHandCount.text) <= 21)
        {
            // if the dealer's hand is less than 17
            while (int.Parse(txtDealerHandCount.text) < 17)
            {
                // infom the manager an action is in progress
                m_blnActionInProgress = true;

                // add a new card to the dealer's hand 
                StartCoroutine(DealCard(m_col_dealerHand, goDealerCardBorder, false));

                // while an action is in progress wait until it is complete
                while (m_blnActionInProgress)
                {
                    yield return null;
                }

                // calculate the dealer's hand 
                CalculateHand(m_col_dealerHand, txtDealerHandCount);

                // play the sfx
                audSrc.Play();
                yield return new WaitForSeconds(fltWaitTimeBeforeDeal);
            }
        }

        // reveal the score and who won
        SelectWinner();

        // show option buttons
        btnMainMenu.gameObject.SetActive(true);
        btnPlayAgain.gameObject.SetActive(true);
    }

    // add a card to the player hand
    public IEnumerator Hit()
    {
        // add a new card to the player hand 
        StartCoroutine(DealCard(m_col_playerHand, goPlayerCardBorder, true));

        // while an action is in progress wait until it is complete
        while(m_blnActionInProgress)
        {
            yield return null;
        }

        // calculate the player's hand 
        CalculateHand(m_col_playerHand, txtPlayerHandCount);

        // check if the player's hand is greater than 21
        if (int.Parse(txtPlayerHandCount.text) > 21)
        {
            // hide the buttons
            btnHit.gameObject.SetActive(false);
            btnStand.gameObject.SetActive(false);

            // calculate the score so that the game is over
            yield return new WaitForSeconds(fltWaitTimeBeforeResults);
            StartCoroutine(Stand());
        }

        yield return new WaitForSeconds(fltWaitTimeAfterHitButton);
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
                if (int.Parse(txtPlayerHandCount.text) < 21)
                {
                    txtWinMessage.text = "You have won!";
                    AssignAudioClip(audClpWin);
                }
                else
                {
                    txtWinMessage.text = "Blackjack!";
                    AssignAudioClip(audClpBlackjack);
                }
                m_intPlayerScore++;
                txtPlayerScore.text = "You: " + m_intPlayerScore;
            }
            else if (int.Parse(txtPlayerHandCount.text) == int.Parse(txtDealerHandCount.text))
            {
                // show the it is a draw
                txtWinMessage.text = "Draw!";
                AssignAudioClip(audClpDraw);
            }
            else
            {
                // if the dealer's hand is greater than 21
                if (int.Parse(txtDealerHandCount.text) > 21)
                {
                    txtWinMessage.text = "The dealer busts. You have won!";
                    m_intPlayerScore++;
                    txtPlayerScore.text = "You: " + m_intPlayerScore;
                    AssignAudioClip(audClpWin);
                }
                else
                {
                    // show that the dealer won and increment the score
                    txtWinMessage.text = "The Dealer has won!";
                    m_intDealerScore++;
                    txtDealerScore.text = "Dealer: " + m_intDealerScore;
                    AssignAudioClip(audClpLose);
                }
            }
        }
        else
        {
            // show that the dealer won and increment the score
            txtWinMessage.text = "Bust! The Dealer has won.";
            m_intDealerScore++;
            txtDealerScore.text = "Dealer: " + m_intDealerScore;
            AssignAudioClip(audClpLose);
        }

        audSrc.Play();
    }

    // assign an audio clip
    private void AssignAudioClip(AudioClip audClp)
    {
        // if the audio clip is not the clip we want
        if (audSrc.clip != audClp)
            // assign it
            audSrc.clip = audClp;

        // adjust the volume based on the clip
        if (audClp == audClpCardShuffle)
            audSrc.volume = fltCardShuffleVolume;
        else if (audClp == audClpCardSlide)
            audSrc.volume = fltCardSlideVolume;
        else if (audClp == audClpWin)
            audSrc.volume = fltWinVolume;
        else if (audClp == audClpLose)
            audSrc.volume = fltLoseVolume;
        else if (audClp == audClpDraw)
            audSrc.volume = fltDrawVolume;
        else if (audClp == audClpBlackjack)
            audSrc.volume = fltBlackjackVolume;
    }

    // reset the spawn offset
    private void ResetSpawnOffset()
    {
        m_vecDealerCardOffset = Vector3.zero;
        m_vecPlayerCardOffset = Vector3.zero;
    }

    // if there are no cards in the deck
    private bool CheckForShuffle()
    {
        // if there is less than the min amount of cards in the deck
        if (DeckManager.Instance.CountDeck() <= 0)
        {
            // shuffle the discard pile into the deck
            DeckManager.Instance.ShuffleDecksTogether(DeckManager.Instance.deck, DeckManager.Instance.discardPile);

            // play the shuffle sfx
            AssignAudioClip(audClpCardShuffle);
            audSrc.Play();

            return true;
        }

        return false;
    }
    #endregion

    #region UI Button Actions
    // stand function for button click
    public void StandButton()
    {
        // if an action is already in progress
        if (m_blnActionInProgress)
            // ignore everything else
            return;

        // infom the manager an action is in progress
        m_blnActionInProgress = true;

        // stand and reveal the hands to determine who wins
        StartCoroutine(Stand());
    }

    // hit button
    public void HitButton()
    {
        // if an action is already in progress
        if (m_blnActionInProgress)
            // ignore everything else
            return;

        // infom the manager an action is in progress
        m_blnActionInProgress = true;

        // add a card to the player hand
        StartCoroutine(Hit());
    }

    // deal a new hand 
    public void PlayAgainButton()
    {
        // hide and show the appropriate buttons
        btnMainMenu.gameObject.SetActive(false);
        btnPlayAgain.gameObject.SetActive(false);

        // for each card in the player's hand
        foreach (Card card in m_col_playerHand)
        {
            // hide the card
            card.card.SetActive(false);
        }

        // for each card in the dealer's hand
        foreach (Card card in m_col_dealerHand)
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

    // go to main menu
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}