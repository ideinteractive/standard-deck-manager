using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MatchManager
/// Description: Manages and handles all functionality for the card match making demo
/// </summary>

public class MatchManager : MonoBehaviour
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

    [Header("Sound Effects")]
    public AudioSource audSrc;                                  // the audio source to play sounds from
    public AudioClip audClpCardSlide;                           // audio clip for dealing a card

    // private variables
    private bool m_gameStarted = false;                         // check if the game has started
    private Card m_cardOne;                                     // holds our first selected card
    private Card m_cardTwo;                                     // holds our second selected card
    private int m_totalScore;                                   // keeps track of the overall score
    private int m_score;                                        // keeps track of the current round's score

    // on initialization
    void Start ()
    {
        // if the audio source is null
        if (audSrc == null)
        {
            // set it from this component
            audSrc = this.GetComponent<AudioSource>();
        }

        // set up our card values below
        // for each card in the deck
        SetUpDeck(DeckManager.instance.deck);
        SetUpDeck(DeckManager.instance.discardPile);
        SetUpDeck(DeckManager.instance.inUsePile);

        // find all gameobjects with the tag card
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Card"))
        {
            // add it to the slot list 
            // and hide them on startup
            slots.Add(go);
            go.SetActive(false);
        }

        // shuffle the deck of cards
        DeckManager.instance.Shuffle();

        // set up our variables and references
        m_gameStarted = false;
        m_cardOne = new Card();
        m_cardTwo = new Card();
        m_score = 0;
        m_totalScore = 0;
        btnMainMenu.gameObject.SetActive(false);
        btnPlayAgain.gameObject.SetActive(false);
        txtWinMessage.text = "";

        // deal a new set
        StartCoroutine(DealNewSet());
	}
	
	// once per frame
	void Update ()
    {
		// if the cards are all dealt 
        if(m_gameStarted)
        {
            // allow the player to start flipping the cards
            FlipCard();
        }
	}

    // deal a new set of cards
    private IEnumerator DealNewSet()
    {
        // if there are cards in the in use pile
        while (DeckManager.instance.inUsePile.Count > 0)
        {
            // put them in the discard pile
            DeckManager.instance.DiscardCard(DeckManager.instance.inUsePile[0], DeckManager.instance.inUsePile);
        }

        // if there is less than 8 cards in the deck
        if (DeckManager.instance.Count() < 8)
        {
            // shuffle the discard pile into the deck before continuing 
            DeckManager.instance.ShuffleDecks(DeckManager.instance.deck, DeckManager.instance.discardPile);
        }

        // assign the correct sfx
        if (audSrc.clip != audClpCardSlide)
            audSrc.clip = audClpCardSlide;

        // for each card slot
        int i = 0;
        while (i<slots.Count)
        {
            // deal 4 cards straight from the top of the deck
            if (i < 4)
            {
                // if any any card after the first card is dealt
                if (i != 0)
                {
                    // for each card in the in use pile
                    int c = 0;
                    Debug.Log("CARDS - " + DeckManager.instance.inUsePile.Count);
                    while (c<DeckManager.instance.inUsePile.Count)
                    {
                        Debug.Log(c);
                        // check to see if the card about to be dealt
                        // is the same rank as one of the cards in the pile
                       while (DeckManager.instance.deck[0].rank == DeckManager.instance.inUsePile[c].rank)
                        {
                            Debug.Log("SHUFFLE");
                            // if it is shuffle the deck of cards and restart the loop
                            DeckManager.instance.Shuffle();
                            
                            //c = 0;
                            //continue;
                        }
                        c++;
                        Debug.Log("INCREASE");
                    }
                }

                // move the current card in the deck manager to the in use pile
                DeckManager.instance.MoveToInUse(DeckManager.instance.deck[0], DeckManager.instance.deck);
            } else
            {
                // find a pair that matches one of the first four cards
                var card = DeckManager.instance.deck.Find(x => x.rank == DeckManager.instance.inUsePile[i-4].rank);

                // move the current card in the deck manager to the in use pile
                DeckManager.instance.MoveToInUse(DeckManager.instance.deck[DeckManager.instance.deck.IndexOf(card)], DeckManager.instance.deck);
            }

            // display the card slot and play the sfx
            slots[i].SetActive(true);
            audSrc.Play();

            // update the deck count
            txtDeckCount.text = DeckManager.instance.Count(DeckManager.instance.deck).ToString();

            yield return new WaitForSeconds(0.5f);
            i++;
        }

        // shuffle the cards again so the ordering is different
        DeckManager.instance.ShuffleInUsePile();

        // start the game
        m_gameStarted = true;
    }

    // detects which card we are raycasting
    // and flip it over
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
                    if (m_cardOne == DeckManager.instance.inUsePile[index] || m_cardTwo == DeckManager.instance.inUsePile[index])
                    {
                        return;
                    }

                    // assign the correct sfx
                    if (audSrc.clip != audClpCardSlide)
                        audSrc.clip = audClpCardSlide;

                    // play the sfx
                    audSrc.Play();

                    // turn the card over in the correct slot
                    
                    DeckManager.instance.inUsePile[index].card.transform.position = slots[index].transform.position;
                    DeckManager.instance.inUsePile[index].card.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    DeckManager.instance.inUsePile[index].card.SetActive(true);

                    // assign our card to the card slot
                    if(m_cardOne.card == null)
                    {
                        m_cardOne = DeckManager.instance.inUsePile[index];
                    } else
                    {
                        m_cardTwo = DeckManager.instance.inUsePile[index];

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
        yield return new WaitForSeconds(0.5f);

        // if the cards match
        if (m_cardOne.rank == m_cardTwo.rank)
        {
            // hide the card slots they are on
            slots[DeckManager.instance.inUsePile.IndexOf(m_cardOne)].SetActive(false);
            slots[DeckManager.instance.inUsePile.IndexOf(m_cardTwo)].SetActive(false);

            // increase the score by 1
            m_score++;
            m_totalScore++;
            txtPlayerScore.text = "Pairs Matched: " + m_totalScore.ToString();
        }

        // reset the card one and two and hide our turned over cards
        m_cardOne.card.SetActive(false);
        m_cardOne = new Card();
        m_cardTwo.card.SetActive(false);
        m_cardTwo = new Card();

        // if the current round's score equals to half the value of our slot
        if(m_score == slots.Count/2)
        {
            // the game is now over
            // show our win screen
            ShowWinScreen();
        }
    }

    // show our win screen
    private void ShowWinScreen()
    {
        // inform the player they won
        txtWinMessage.text = "You've matched all the pairs. Nice job!";

        // display our buttons
        btnMainMenu.gameObject.SetActive(true);
        btnPlayAgain.gameObject.SetActive(true);
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

    // set up the deck
    private void SetUpDeck(List<Card> deck)
    {
        // for each card in the deck
        int i = 0;
        while (i < deck.Count)
        {
            SetCardValue(deck[i]);
            i++;
        }
    }

    // play again
    public void PlayAgain()
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

    // go to main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
