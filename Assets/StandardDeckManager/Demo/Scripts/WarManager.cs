using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// WarManager
/// Description: Manages and handles all functionality of the war demo
/// </summary>

public class WarManager : MonoBehaviour
{
    #region Variables
    // public variables
    [Header("Game Objects")]
    public GameObject goOpponentCardBorder;             // where the opponent card spawns
    public GameObject goPlayerCardBorder;               // where the player card spawns
    public GameObject goOpponentBackFaceCard;           // the opponent's backface card
    public GameObject goPlayerBackFaceCard;             // the player's backface card

    [Header("UI Objects")]
    public Text txtOpponentDeckCount;                   // text object to track of opponent's deck count
    public Text txtPlayerDeckCount;                     // text object to track of players's deck count
    public Text txtPlayerScore;                         // text object to track player's score
    public Text txtOpponentScore;                       // text object to track opponent's score
    public Text txtWinMessage;                          // text object to inform who won
    public Button btnDeal;                              // deal button
    public Button btnPlayAgain;                         // replay button
    public Button btnMainMenu;                          // main menu button

    [Header("Other Settings")]
    public float fltWaitTimeAfterShuffle = 0.7f;        // the wait time after the deck is shuffled
    public float fltWaitTimeBeforeResults = 0.5f;       // the wait time before the winner is determined

    [Header("Sound Effects")]
    public AudioSource audSrc;                          // the audio source to play sounds from
    public AudioClip audClpCardSlide;                   // audio clip for dealing a card
    public AudioClip audClpCardShuffle;                 // audio clip for shuffling the deck
    public AudioClip audClpWin;                         // audio clip for win state
    public AudioClip audClpLose;                        // audio clip for lose state
    public AudioClip audClpDraw;                        // audio clip for draw state
    public AudioClip audClpFinalWin;                    // audio clip for final win state
    public AudioClip audClpFinalLose;                   // audio clip for final lose state
    public AudioClip audClpFinalDraw;                   // audio clip for final draw state

    [Header("Volume Levels")]
    public float fltCardSlideVolume = 0.5f;             // the volume for card slide  
    public float fltCardShuffleVolume = 0.5f;           // the volume for card shuffling   
    public float fltWinVolume = 0.5f;                   // the volume for our win sound
    public float fltLoseVolume = 0.5f;                  // the volume for our lose sound
    public float fltDrawVolume = 0.5f;                  // the volume for our draw sound
    public float fltFinalWinVolume = 0.5f;              // the volume for our final win sound
    public float fltFinalLoseVolume = 0.5f;             // the volume for our final lose sound
    public float fltFinalDrawVolume = 0.5f;             // the volume for our final draw sound

    // private evariables
    private List<Card> m_col_opponentDeck;              // the opponent's deck
    private List<Card> m_col_playerDeck;                // the player's deck
    private int m_intPlayerScore;                       // the player's score
    private int m_intOpponentScore;                     // the opponent's score
    private bool m_blnCardInPlay;                       // check if there are cards in play already
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

        // reset our button states
        btnDeal.gameObject.SetActive(false);
        btnPlayAgain.gameObject.SetActive(false);
        btnMainMenu.gameObject.SetActive(false);

        // reset our back face cards
        goOpponentBackFaceCard.SetActive(true);
        goPlayerBackFaceCard.SetActive(true);

        // reset the score
        m_intOpponentScore = 0;
        m_intPlayerScore = 0;

        while (DeckManager.Instance.CountDeck() != 52)
        {
            // check if all the decks together 
            // equal 52 cards and if they do not
            if (DeckManager.Instance.CountAllDecks() != 52)
            {
                // remoev everything and generate a new deck so we have 52 cards exactly
                DeckManager.Instance.RemoveAllAndCreateNew();
            }
            else
            {
                // shuffle all the decks together
                DeckManager.Instance.ShuffleAllDecksTogether();
            }
        }

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
                card.value = 11;
                break;
            case Card.Rank.Queen:
                card.value = 12;
                break;
            case Card.Rank.King:
                card.value = 13;
                break;
            default:
                card.value = 0;
                break;
        }
    }

    // initializes the game and handles the setup
    private IEnumerator InitializeGame()
    {
        // inform the manager we have no cards in play
        m_blnCardInPlay = false;

        // setup our new lists
        m_col_playerDeck = new List<Card>();
        m_col_opponentDeck = new List<Card>();

        // set up each deck's card value
        SetUpDeck(DeckManager.Instance.deck);

        // play our shuffle sfx
        AssignAudioClip(audClpCardShuffle);
        audSrc.Play();

        // shuffle the deck of cards
        DeckManager.Instance.ShuffleDeck();

        // split the deck between the player and the opponent
        while (DeckManager.Instance.CountDeck() != 0)
        {
            if (DeckManager.Instance.CountDeck() % 2 == 0)
                m_col_playerDeck.Add(DeckManager.Instance.deck[0]);
            else
                m_col_opponentDeck.Add(DeckManager.Instance.deck[0]);
            DeckManager.Instance.MoveCardToInUse(DeckManager.Instance.deck[0], DeckManager.Instance.deck);
        }
        yield return new WaitForSeconds(fltWaitTimeAfterShuffle);

        // show the deck count
        txtPlayerDeckCount.text = m_col_playerDeck.Count.ToString();
        txtOpponentDeckCount.text = m_col_opponentDeck.Count.ToString();

        // show the deal button
        btnDeal.gameObject.SetActive(true);
    }

    // deal cards
    private IEnumerator DealCards()
    {
        // turn on our buttons
        btnDeal.gameObject.SetActive(false);

        // if we have cards in play
        if (m_blnCardInPlay)
        {
            // remove the card from each player's deck and also put it in the
            // discard pile and reset our variables for the current round
            Card card = DeckManager.Instance.inUsePile.First(s => s == m_col_playerDeck[0]);
            DeckManager.Instance.MoveCardToDiscard(card, DeckManager.Instance.inUsePile);
            m_col_playerDeck[0].card.SetActive(false);
            m_col_playerDeck.Remove(m_col_playerDeck[0]);
            card = DeckManager.Instance.inUsePile.First(s => s == m_col_opponentDeck[0]);
            DeckManager.Instance.MoveCardToDiscard(card, DeckManager.Instance.inUsePile);
            m_col_opponentDeck[0].card.gameObject.SetActive(false);
            m_col_opponentDeck.Remove(m_col_opponentDeck[0]);
            m_blnCardInPlay = false;
            txtWinMessage.text = "";
            yield return new WaitForSeconds(fltWaitTimeBeforeResults);
        }

        // if we have no more cards in the deck hide the back face object
        if (m_col_playerDeck.Count == 1)
        {
            goPlayerBackFaceCard.gameObject.SetActive(false);
            txtPlayerDeckCount.gameObject.SetActive(false);
        }

        if (m_col_opponentDeck.Count == 1)
        {
            goOpponentBackFaceCard.gameObject.SetActive(false);
            txtOpponentDeckCount.gameObject.SetActive(false);
        }

        // flip the top card of each deck onto the slot
        FlipCard(m_col_playerDeck[0].card, goPlayerCardBorder);
        FlipCard(m_col_opponentDeck[0].card, goOpponentCardBorder);

        // update the deck count
        txtPlayerDeckCount.text = (m_col_playerDeck.Count - 1).ToString();
        txtOpponentDeckCount.text = (m_col_opponentDeck.Count - 1).ToString();

        // play the card slide sfx
        AssignAudioClip(audClpCardSlide);
        audSrc.Play();

        yield return new WaitForSeconds(fltWaitTimeBeforeResults);

        m_blnCardInPlay = true;

        // select a winner
        SelectWinner();
    }

    // reveal the score and who won
    private void SelectWinner()
    {
        // if the deck count is greater than 1
        if ((m_col_playerDeck.Count - 1) > 0)
        {
            // if the player score is higher than the opponent's
            if (m_col_playerDeck[0].value > m_col_opponentDeck[0].value)
            {
                txtWinMessage.text = "You have won!";
                AssignAudioClip(audClpWin);
                m_intPlayerScore++;
                txtPlayerScore.text = "You: " + m_intPlayerScore;
            }
            else if (m_col_playerDeck[0].value == m_col_opponentDeck[0].value)
            {
                // show the it is a draw
                txtWinMessage.text = "Draw!";
                AssignAudioClip(audClpDraw);
            }
            else
            {
                // show that the opponent has won and increment the score
                txtWinMessage.text = "Your Opponent has won!";
                m_intOpponentScore++;
                txtOpponentScore.text = "Opponent: " + m_intOpponentScore;
                AssignAudioClip(audClpLose);
            }

            audSrc.Play();

            // enable our buttons
            btnDeal.gameObject.SetActive(true);
        }
        else
        {
            // if the player has a higher total score
            if (m_intPlayerScore > m_intOpponentScore)
            {
                txtWinMessage.text = "You have won the game!";
                AssignAudioClip(audClpFinalWin);
                txtPlayerScore.text = "You: " + m_intPlayerScore;
            }
            else if (m_intPlayerScore == m_intOpponentScore)
            {
                // show the it is a draw
                txtWinMessage.text = "You and Your Opponent had a Draw!";
                AssignAudioClip(audClpFinalDraw);
            }
            else
            {
                // show that the opponent has won and increment the score
                txtWinMessage.text = "Your Opponent has won the game!";
                txtOpponentScore.text = "Opponent: " + m_intOpponentScore;
                AssignAudioClip(audClpFinalLose);
            }

            audSrc.Play();

            // enable our buttons
            btnMainMenu.gameObject.SetActive(true);
            btnPlayAgain.gameObject.SetActive(true);
        }
    }

    // flip the card over
    private void FlipCard(GameObject card, GameObject slot)
    {
        card.transform.position = slot.transform.position;
        card.SetActive(true);
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
        else if (audClp == audClpFinalWin)
            audSrc.volume = fltFinalWinVolume;
        else if (audClp == audClpFinalLose)
            audSrc.volume = fltFinalLoseVolume;
        else if (audClp == audClpFinalDraw)
            audSrc.volume = fltFinalDrawVolume;
    }
    #endregion

    #region UI Button Actions
    // deal button
    public void DealButton()
    {
        // add a card to the player hand
        StartCoroutine(DealCards());
    }

    // deal a new hand 
    public void PlayAgainButton()
    {
        // reset the ui
        btnMainMenu.gameObject.SetActive(false);
        btnPlayAgain.gameObject.SetActive(false);
        txtWinMessage.text = "";

        // reset our deck
        m_col_playerDeck[0].card.SetActive(false);
        m_col_playerDeck.Remove(m_col_playerDeck[0]);
        m_col_opponentDeck[0].card.SetActive(false);
        m_col_opponentDeck.Remove(m_col_opponentDeck[0]);

        // shuffle all the decks together
        DeckManager.Instance.ShuffleAllDecksTogether();

        // turn back on some of our objects
        goPlayerBackFaceCard.gameObject.SetActive(true);
        txtPlayerDeckCount.gameObject.SetActive(true);
        goOpponentBackFaceCard.gameObject.SetActive(true);
        txtOpponentDeckCount.gameObject.SetActive(true);

        // reinitialize the game
        StartCoroutine(InitializeGame());
    }

    // go to main menu
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}