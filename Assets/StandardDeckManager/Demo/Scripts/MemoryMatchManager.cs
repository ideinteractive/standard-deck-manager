using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MemoryMatchManager
/// Description: Manages and handles all functionality for the memory matching demo
/// </summary>

public class MemoryMatchManager : MonoBehaviour
{
    // public variables
    [Header("Game Objects")]
    public List<GameObject> slots = new List<GameObject>();     // our card slots to keep track of our card positions

    [Header("UI Objects")]
    public Text txtDeckCount;                                   // text object to track deck count
    public Text txtPlayerScore;                                 // text object to track player's score
    public Text txtWinMessage;                                  // text object to inform the game is over
    public Button btnMainMenu;                                  // main menu button
    public Button btnPlayAgain;                                 // play again button

    [Header("Other Settings")]
    public float fltWaitTimeAfterShuffle = 0.7f;                // the wait time after the deck is shuffled
    public float fltWaitTimeBeforeDeal = 0.5f;                  // the wait time between when a dealer hits  
    public float fltWaitTimeBeforeResults = 0.5f;               // the wait time before the winner is determined

    [Header("Sound Effects")]
    public AudioSource audSrc;                                  // the audio source to play sounds from
    public AudioClip audClpCardSlide;                           // audio clip for dealing a card
    public AudioClip audClpCardShuffle;                         // audio clip for shuffling the deck
    public AudioClip audClpWin;                                 // audio clip for win state
    public AudioClip audClpMatch;                               // audio clip for match state
    public AudioClip audClpNoMatch;                             // audio clip for no match state

    [Header("Volume Levels")]
    public float fltCardSlideVolume = 0.5f;                     // the volume for card slide  
    public float fltCardShuffleVolume = 0.5f;                   // the volume for card shuffling   
    public float fltWinVolume = 0.5f;                           // the volume for our win sound
    public float fltMatchVolume = 0.5f;                         // the volume for our match sound
    public float fltNoMatchVolume = 0.5f;                       // the volume for our no match sound

    // private variables
    private bool m_gameStarted = false;                         // check if the game has started
    private Card m_cardOne;                                     // holds our first selected card
    private Card m_cardTwo;                                     // holds our second selected card
    private int m_totalScore;                                   // keeps track of the overall score
    private int m_score;                                        // keeps track of the current round's score

    // on initialization
    void Start()
    {
        // if the audio source is null
        if (audSrc == null)
        {
            // set it from this component
            audSrc = this.GetComponent<AudioSource>();
        }

        // set up each deck's card value
        SetUpDeck(DeckManager.Instance.deck);
        SetUpDeck(DeckManager.Instance.discardPile);
        SetUpDeck(DeckManager.Instance.inUsePile);

        // find all gameobjects with the tag card
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Card"))
        {
            // add it to the slot list 
            // and hide them on startup
            slots.Add(go);
            go.SetActive(false);
        }

        // reverse the list
        slots.Reverse();

        // shuffle the deck of cards
        DeckManager.Instance.ShuffleDeck();

        // update the deck count
        txtDeckCount.text = DeckManager.Instance.CountDeck().ToString();

        // play the shuffle sfx
        AssignAudioClip(audClpCardShuffle);
        audSrc.Play();

        // reset our variables and references
        m_totalScore = 0;
        ResetGameVariables();
    }

    // once per frame
    void Update()
    {
        // if the cards are all dealt 
        if (m_gameStarted)
            // allow the player to start flipping the cards
            FlipCard();
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

    // deal a new set of cards
    private IEnumerator DealNewSet()
    {
        // if there are cards in the in use pile
        if (DeckManager.Instance.CountInUsePile() > 0)
            // put them in the discard pile
            DeckManager.Instance.MoveAllCardToDiscard(DeckManager.Instance.inUsePile);

        // check if the discard pile should 
        // be shuffled back into the main deck
        if (CheckForShuffle())
            yield return new WaitForSeconds(fltWaitTimeAfterShuffle);

        // create a new temporary list of cards and select only distinct ranks
        IEnumerable<Card> ienumerableCardList = DeckManager.Instance.deck.GroupBy(x => x.rank).Select(g => g.First()).Distinct().ToList();
        List<Card> tempListOfCards = ienumerableCardList.ToList();

        // for each card slot
        int i = 0;
        while (i < slots.Count)
        {

            // for the first 4 cards
            if (i < 4)
            {
                // if there is less than 9 cards in the deck
                while (DeckManager.Instance.CountDeck() <= 8)
                {
                    if (CheckForShuffle())
                    {
                        // pull a new set of unqiue cards
                        ienumerableCardList = DeckManager.Instance.deck.GroupBy(x => x.rank).Select(g => g.First()).Distinct().ToList();
                        tempListOfCards = ienumerableCardList.ToList();

                        // for each card in our in use pile
                        int tc = 0;
                        while (tc < DeckManager.Instance.CountInUsePile())
                        {
                            // check the list for duplicate ranks matching our in use pile and remove them
                            tempListOfCards.RemoveAll(x => x.rank == DeckManager.Instance.inUsePile[tc].rank);
                            tc++;
                        }

                        yield return new WaitForSeconds(fltWaitTimeAfterShuffle);
                    }

                    // play the shuffle sfx
                    AssignAudioClip(audClpCardShuffle);
                    audSrc.Play();
                }

                // select a card from the top of the temp list of cards
                Card card = DeckManager.Instance.deck.Single(s => s == tempListOfCards[i]);

                // move that card in the deck manager to the in use pile
                DeckManager.Instance.MoveCardToInUse(card, DeckManager.Instance.deck);
            }
            else
            {
                // check if the discard pile should 
                // be shuffled back into the main deck
                if (CheckForShuffle())
                    yield return new WaitForSeconds(fltWaitTimeAfterShuffle);

                // find a pair for each of the first four cards dealt
                Card card = DeckManager.Instance.deck.First(s => s.rank == DeckManager.Instance.inUsePile[i - 4].rank);

                // move that card in the deck manager to the in use pile
                DeckManager.Instance.MoveCardToInUse(card, DeckManager.Instance.deck);
            }

            // display the card slot and play the sfx
            slots[i].SetActive(true);
            AssignAudioClip(audClpCardSlide);
            audSrc.Play();

            // update the deck count
            txtDeckCount.text = DeckManager.Instance.CountDeck().ToString();

            yield return new WaitForSeconds(0.5f);
            i++;
        }

        // shuffle the cards again so the ordering is different
        DeckManager.Instance.ShuffleInUsePile();

        // start the game
        m_gameStarted = true;
    }

    // detects which card we are raycasting and flip it over
    private void FlipCard()
    {
        // if we are left mouse clicking
        // and we still have room to hold one or two cards
        if (Input.GetMouseButtonDown(0) && (m_cardOne.card == null || m_cardTwo.card == null))
        {
            // draw a ray from the camera to our mouse position
            // and check if we've hit anything
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // if we're hitting a collider
            if (hit.collider != null)
            {
                // check if the collider is tagged "Card"
                if (hit.collider.tag == "Card")
                {
                    // get the index of the current card
                    int index = slots.IndexOf(hit.collider.gameObject);

                    // check if the card being hit is the same card
                    if (m_cardOne == DeckManager.Instance.inUsePile[index] || m_cardTwo == DeckManager.Instance.inUsePile[index])
                        return;

                    // assign the card slide sfx
                    AssignAudioClip(audClpCardSlide);
                    audSrc.Play();

                    // turn the card over in the correct slot
                    DeckManager.Instance.inUsePile[index].card.transform.position = slots[index].transform.position;
                    DeckManager.Instance.inUsePile[index].card.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    DeckManager.Instance.inUsePile[index].card.SetActive(true);

                    // assign our card to the card slot
                    if (m_cardOne.card == null)
                    {
                        m_cardOne = DeckManager.Instance.inUsePile[index];
                    }
                    else
                    {
                        m_cardTwo = DeckManager.Instance.inUsePile[index];

                        // check the pair of cards for a match
                        // only if we have a second card
                        StartCoroutine(CheckForMatch());
                    }
                }
            }
        }
    }

    // check the pair of cards for a match
    private IEnumerator CheckForMatch()
    {
        // wait for the player to see the card before we proceed
        yield return new WaitForSeconds(fltWaitTimeBeforeResults);

        // if the cards match
        if (m_cardOne.rank == m_cardTwo.rank)
        {
            // hide the card slots they are on
            slots[DeckManager.Instance.inUsePile.IndexOf(m_cardOne)].SetActive(false);
            slots[DeckManager.Instance.inUsePile.IndexOf(m_cardTwo)].SetActive(false);
            
            // increase the score by 1
            m_score++;
            m_totalScore++;
            txtPlayerScore.text = "Pairs Matched: " + m_totalScore.ToString();

            // play the match sfx
            AssignAudioClip(audClpMatch);
            audSrc.Play();
        } else
        {
            // play the no match sfx
            AssignAudioClip(audClpNoMatch);
            audSrc.Play();
        }

        // reset the card one and two and hide our turned over cards
        m_cardOne.card.SetActive(false);
        m_cardOne = new Card();
        m_cardTwo.card.SetActive(false);
        m_cardTwo = new Card();

        // if the current round's score equals to half the value of our slot
        if (m_score == slots.Count / 2)
        {
            yield return new WaitForSeconds(fltWaitTimeBeforeResults);

            // the game is now over
            // show our win screen
            ShowWinScreen();
        }
    }

    // show our win screen
    private void ShowWinScreen()
    {
        // play the win sfx
        AssignAudioClip(audClpWin);
        audSrc.Play();

        // inform the player they won
        txtWinMessage.text = "You've matched all the pairs. Nice job!";
        AssignAudioClip(audClpWin);
        audSrc.Play();

        // display our buttons
        btnMainMenu.gameObject.SetActive(true);
        btnPlayAgain.gameObject.SetActive(true);
    }

    // reset our variables and references
    private void ResetGameVariables()
    {
        // reset our variables and references
        m_gameStarted = false;
        m_cardOne = new Card();
        m_cardTwo = new Card();
        m_score = 0;
        btnMainMenu.gameObject.SetActive(false);
        btnPlayAgain.gameObject.SetActive(false);
        txtWinMessage.text = "";

        // deal a new set
        StartCoroutine(DealNewSet());
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
        else if (audClp == audClpMatch)
            audSrc.volume = fltMatchVolume;
        else if (audClp == audClpNoMatch)
            audSrc.volume = fltNoMatchVolume;
    }

    // check if the discard pile should 
    // be shuffled back into the main deck
    private bool CheckForShuffle()
    {
        // if there is less than 9 cards in the deck
        if (DeckManager.Instance.CountDeck() == 0)
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
    // go to main menu
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // deal a new set of cards
    public void PlayAgainButton()
    {
        // reset our variables and references
        ResetGameVariables();
    }
    #endregion
}
