using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// DeckManager
/// Description: Manages and handles a standard deck of 52 playing cards.
/// </summary>

[ExecuteInEditMode]
public class DeckManager : MonoBehaviour
{
    // public variables

    public List<Cards> deck = new List<Cards>();     // contains a list of cards
    public List<Cards> discardPile = new List<Cards>();

    #if UNITY_EDITOR
    [SerializeField]
    private bool initialized = false;

    // on awake
    void Awake() {
        // perform initialization
        Initialize();
    }

    // on initialization
    void Initialize()
    {
        // if initialization is true
        if (initialized)
            // return and break out of this function
            return;

        // create a new list of cards from the standard json template
        ListOfCards listOfCards = JsonUtility.FromJson<ListOfCards>(File.ReadAllText(Application.dataPath + "/Data/Standard52CardDesk.json"));

        // create a new deck based on that list
        deck = new List<Cards>(listOfCards.deck);

        // create a new discard pile
        discardPile = new List<Cards>();

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported ");

        initialized = true;
    }
    #endif
} 