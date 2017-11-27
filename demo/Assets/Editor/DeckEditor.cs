using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// DeckEditor
/// Description: Custom editor for the DeckManager.
/// </summary>

[CustomEditor(typeof(DeckManager))]
public class DeckEditor : Editor
{
    // reference to the DeckManager
    private DeckManager deckManager;   

    // json file that contains our standard deck
    private string jsonStandardDeck = File.ReadAllText(Application.dataPath + "/Data/Standard52CardDesk.json");     

    private void OnEnable()
    {
        // set the reference to the current inspected object
        deckManager = (DeckManager)target;

        // if the deck contains less than 52 cards
        if (deckManager.deck.Count < 52)
        {
            // import the standard deck template
            ImportStandardDeck();
        }
    }

    // overwrite inspector interface
    public override void OnInspectorGUI()
    {
        // record any changes done within the DeckManager
        Undo.RecordObjects(targets, "DeckManager");

        EditorGUILayout.Space();

        // if there are cards in the deck
        if (deckManager.deck.Count > 0)
        {
            // for each card in the deck
            foreach (Cards card in deckManager.deck)
            {
                EditorGUILayout.LabelField(card.rank + " of " + card.suit + " [" + card.color + "]");
            }
        } else
        {
            // import the standard deck template
            ImportStandardDeck();
        }

        EditorGUILayout.Space();
    }

    // import the standard deck template
    private void ImportStandardDeck()
    {
        // create a new list of cards from the standard json template
        ListOfCards listOfCards = JsonUtility.FromJson<ListOfCards>(jsonStandardDeck);

        // create a new deck based on that list
        deckManager.deck = new List<Cards>(listOfCards.deck);

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported ");
    }
}
