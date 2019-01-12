using UnityEngine;
using UnityEditor;

/// <summary>
/// DeckOptionsEditor
/// Description: Custom editor for options in the DeckManager.
/// </summary>

[CustomEditor(typeof(DeckOptions))]
public class DeckOptionsEditor : Editor
{
    // reference to the DeckOptions
    private DeckOptions deckOptions;

    private void OnEnable()
    {
        // set the reference to the current inspected object
        deckOptions = (DeckOptions)target;
    }

    // overwrite inspector interface
    public override void OnInspectorGUI()
    {
        // header styles
        GUIStyle styleRowHeader = new GUIStyle();
        styleRowHeader.padding = new RectOffset(0, 0, 3, 3);
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        GUIStyle stylePaddingLeft = new GUIStyle();
        stylePaddingLeft.padding = new RectOffset(10, 0, 3, 3);

        /// EDITOR

        // record any changes done within the DeckOptions
        Undo.RecordObjects(targets, "DeckOptions");

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Deck Options", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("The Deck Options Editor is only meant to help you debug and test the Deck Manager.", MessageType.Info);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shuffle Individual Decks");

        // shuffle the deck
        if (GUILayout.Button("Deck"))
            deckOptions.deckManager.Shuffle();

        // shuffle the discard pile
        if (GUILayout.Button("Discard Pile"))
            deckOptions.deckManager.ShuffleDiscardPile();

        // shuffle the in use pile
        if (GUILayout.Button("In Use Pile"))
            deckOptions.deckManager.ShuffleInUsePile();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shuffle Decks Together");

        // shuffle the deck with the discard pile
        if (GUILayout.Button("Deck with Discard Pile"))
            deckOptions.deckManager.ShuffleDecks(deckOptions.deckManager.deck, deckOptions.deckManager.discardPile);

        // shuffle the deck with the in use pile
        if (GUILayout.Button("Deck with In Use Pile"))
            deckOptions.deckManager.ShuffleDecks(deckOptions.deckManager.deck, deckOptions.deckManager.inUsePile);

        // shuffle the discard and deck
        if (GUILayout.Button("Discard with In Use Pile"))
            deckOptions.deckManager.ShuffleDecks(deckOptions.deckManager.discardPile, deckOptions.deckManager.deck);

        // shuffle the discard and in use pile
        if (GUILayout.Button("Discard with In Use Pile"))
            deckOptions.deckManager.ShuffleDecks(deckOptions.deckManager.discardPile, deckOptions.deckManager.inUsePile);

        // shuffle the in use pile and deck
        if (GUILayout.Button("In Use Pile with Deck"))
            deckOptions.deckManager.ShuffleDecks(deckOptions.deckManager.inUsePile, deckOptions.deckManager.deck);

        // shuffle the in use pile and discard pile
        if (GUILayout.Button("In Use Pile with Discard Pile"))
            deckOptions.deckManager.ShuffleDecks(deckOptions.deckManager.inUsePile, deckOptions.deckManager.discardPile);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shuffle To Top");

        // shuffle the discard pile and add it to the top of the deck
        if (GUILayout.Button("Discard Pile to Top of Deck"))
            deckOptions.deckManager.ShuffleToTop(deckOptions.deckManager.deck, deckOptions.deckManager.discardPile);

        // shuffle the in use pile and add it to the top of the deck
        if (GUILayout.Button("In Use Pile to Top of Deck"))
            deckOptions.deckManager.ShuffleToTop(deckOptions.deckManager.deck, deckOptions.deckManager.inUsePile);

        // shuffle the deck and add it to the top of the discard pile
        if (GUILayout.Button("Deck to Top of Discard Pile"))
            deckOptions.deckManager.ShuffleToTop(deckOptions.deckManager.discardPile, deckOptions.deckManager.deck);

        // shuffle the in use pile and add it to the top of the discard pile
        if (GUILayout.Button("In Use Pile to Top of Discard Pile"))
            deckOptions.deckManager.ShuffleToTop(deckOptions.deckManager.discardPile, deckOptions.deckManager.inUsePile);

        // shuffle the deck and add it to the top of the in use pile
        if (GUILayout.Button("Deck to Top of In Use Pile"))
            deckOptions.deckManager.ShuffleToTop(deckOptions.deckManager.inUsePile, deckOptions.deckManager.deck);

        // shuffle the discard pile and add it to the top of the in use pile
        if (GUILayout.Button("Discard Pile to Top of In Use Pile"))
            deckOptions.deckManager.ShuffleToTop(deckOptions.deckManager.inUsePile, deckOptions.deckManager.discardPile);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shuffle To Bottom");

        // shuffle the discard pile and add it to the bottom of the deck
        if (GUILayout.Button("Discard Pile to Bottom of Deck"))
            deckOptions.deckManager.ShuffleToBottom(deckOptions.deckManager.deck, deckOptions.deckManager.discardPile);

        // shuffle the in use pile and add it to the bottom of the deck
        if (GUILayout.Button("In Use Pile to Bottom of Deck"))
            deckOptions.deckManager.ShuffleToBottom(deckOptions.deckManager.deck, deckOptions.deckManager.inUsePile);

        // shuffle the deck and add it to the bottom of the discard pile
        if (GUILayout.Button("Deck to Bottom of Discard Pile"))
            deckOptions.deckManager.ShuffleToBottom(deckOptions.deckManager.discardPile, deckOptions.deckManager.deck);

        // shuffle the in use pile and add it to the bottom of the discard pile
        if (GUILayout.Button("In Use Pile to Bottom of Discard Pile"))
            deckOptions.deckManager.ShuffleToBottom(deckOptions.deckManager.discardPile, deckOptions.deckManager.inUsePile);

        // shuffle the deck and add it to the bottom of the in use pile
        if (GUILayout.Button("Deck to Bottom of In Use Pile"))
            deckOptions.deckManager.ShuffleToBottom(deckOptions.deckManager.inUsePile, deckOptions.deckManager.deck);

        // shuffle the discard pile and add it to the bottom of the in use pile
        if (GUILayout.Button("Discard Pile to Bottom of In Use Pile"))
            deckOptions.deckManager.ShuffleToBottom(deckOptions.deckManager.inUsePile, deckOptions.deckManager.discardPile);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add Card");

        // add a new card to the deck
        if (GUILayout.Button("Deck"))
        {
            Card card = new Card();
            card.color = (Card.Color)Random.Range(0, (int)Card.Color.Red + 1);
            card.rank = (Card.Rank)Random.Range(0, (int)Card.Rank.King + 1);
            card.suit = (Card.Suit)Random.Range(0, (int)Card.Suit.Spades + 1);
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckOptions.deckManager.AddCard(card, deckOptions.deckManager.deck);
        }

        // add a new card to the discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Card card = new Card();
            card.color = (Card.Color)Random.Range(0, (int)Card.Color.Red + 1);
            card.rank = (Card.Rank)Random.Range(0, (int)Card.Rank.King + 1);
            card.suit = (Card.Suit)Random.Range(0, (int)Card.Suit.Spades + 1);
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckOptions.deckManager.AddCard(card, deckOptions.deckManager.discardPile);
        }

        // add a new card to the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Card card = new Card();
            card.color = (Card.Color)Random.Range(0, (int)Card.Color.Red + 1);
            card.rank = (Card.Rank)Random.Range(0, (int)Card.Rank.King + 1);
            card.suit = (Card.Suit)Random.Range(0, (int)Card.Suit.Spades + 1);
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckOptions.deckManager.AddCard(card, deckOptions.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Misc");

        // remove everything
        if (GUILayout.Button("Remove All Card"))
        {
            deckOptions.deckManager.RemoveAll();
        }

        // remove everything and create a new standard deck
        if (GUILayout.Button("Generate New Deck"))
        {
            deckOptions.deckManager.RemoveAllAndCreateNew();
        }

        EditorGUILayout.Space();
    }
}
