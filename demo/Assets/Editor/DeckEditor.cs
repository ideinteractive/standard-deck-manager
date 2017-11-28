using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/// <summary>
/// DeckEditor
/// Description: Custom editor for the DeckManager.
/// </summary>

[CustomEditor(typeof(DeckManager))]
public class DeckEditor : Editor
{
    // reference to the DeckManager
    private DeckManager deckManager;

    // create reorderable lists for each deck
    private ReorderableList deck;

    private void OnEnable()
    {
        // set the reference to the current inspected object
        deckManager = (DeckManager)target;
    }

    // overwrite inspector interface
    public override void OnInspectorGUI()
    {
        /// GUI STYLES

        // header styles
        GUIStyle styleRowHeader = new GUIStyle();
        styleRowHeader.padding = new RectOffset(0, 0, 3, 3);
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        GUIStyle stylePaddingLeft = new GUIStyle();
        stylePaddingLeft.padding = new RectOffset(10, 0, 3, 3);

        /// EDITOR

        // record any changes done within the DeckManager
        Undo.RecordObjects(targets, "DeckManager");

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Deck Manager", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // add a new card to the deck
        if (GUILayout.Button("Shuffle Deck"))
            deckManager.AddNewCard(deckManager.discardPile);

        // add a new card to the deck
        if (GUILayout.Button("Shuffle Deck with Discard Pile"))
            deckManager.AddNewCard(deckManager.discardPile);

        // add a new card to the deck
        if (GUILayout.Button("Shuffle Discard Pile to Bottom"))
            deckManager.AddNewCard(deckManager.discardPile);

        // add a new card to the deck
        if (GUILayout.Button("Add New Card to Deck"))
            deckManager.AddNewCard(deckManager.deck);

        // add a new card to the discard pile
        if (GUILayout.Button("Add New Card to Discard Pile"))
            deckManager.AddNewCard(deckManager.discardPile);
            
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Deck", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // try and render the card info
        try {
            // for each card inside the deck starting from the lowest to highest (order)
            for (int i = 0; i < deckManager.deck.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(stylePaddingLeft);

                // expand or collapse the card's panel depending on the boolean status
                // and name the panel based on the card
                deckManager.deck[i].blnExpandClipPnl = EditorGUILayout.Foldout(deckManager.deck[i].blnExpandClipPnl, deckManager.deck[i].strName);

                // move the card up on the list
                if (GUILayout.Button("U", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardUp(deckManager.deck[i], deckManager.deck);

                // move the card down on the list
                if (GUILayout.Button("D", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardDown(deckManager.deck[i], deckManager.deck);

                // move the card to the top of the list
                if (GUILayout.Button("T", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardToTop(deckManager.deck[i], deckManager.deck);

                // move the card to the bottom of the list
                if (GUILayout.Button("B", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardToBottom(deckManager.deck[i], deckManager.deck);

                // remove the card from the list
                if (GUILayout.Button("R", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.RemoveCard(deckManager.deck[i], deckManager.deck);

                // move the card to the discard pile
                if (GUILayout.Button("-", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.DiscardCard(deckManager.deck[i]);

                EditorGUILayout.EndHorizontal();

                // if the card's panel is expanded
                if (deckManager.deck[i].blnExpandClipPnl)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(stylePaddingLeft);

                    // show the card's name
                    deckManager.deck[i].strName = EditorGUILayout.TextField("Name", deckManager.deck[i].strName);

                    // show the card's color
                    deckManager.deck[i].color = (Cards.Color)EditorGUILayout.EnumPopup("Color", deckManager.deck[i].color);

                    // show the card's rank
                    deckManager.deck[i].rank = (Cards.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.deck[i].rank);

                    // show the card's suit
                    deckManager.deck[i].suit = (Cards.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.deck[i].suit);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }
        } catch
        {
            return;
        }

        EditorGUILayout.Space();

        // discard pile

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Discard Pile", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // try and render the card info
        try {
            // for each card inside the discard pile starting from the lowest to highest (order)
            for (int i = 0; i < deckManager.discardPile.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(stylePaddingLeft);

                // expand or collapse the card's panel depending on the boolean status
                // and name the panel based on the card
                deckManager.discardPile[i].blnExpandClipPnl = EditorGUILayout.Foldout(deckManager.discardPile[i].blnExpandClipPnl, deckManager.discardPile[i].strName);

                // move the card up on the list
                if (GUILayout.Button("U", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardUp(deckManager.discardPile[i], deckManager.discardPile);

                // move the card down on the list
                if (GUILayout.Button("D", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardDown(deckManager.discardPile[i], deckManager.discardPile);

                // move the card to the top of the list
                if (GUILayout.Button("T", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardToTop(deckManager.discardPile[i], deckManager.discardPile);

                // move the card to the bottom of the list
                if (GUILayout.Button("B", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.MoveCardToBottom(deckManager.discardPile[i], deckManager.discardPile);

                // remove the card from the list
                if (GUILayout.Button("R", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.RemoveCard(deckManager.discardPile[i], deckManager.discardPile);

                // move the card to the deck
                if (GUILayout.Button("+", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                    deckManager.AddCard(deckManager.discardPile[i]);

                EditorGUILayout.EndHorizontal();

                // if the card's panel is expanded
                if (deckManager.discardPile[i].blnExpandClipPnl)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(stylePaddingLeft);

                    // show the card's name
                    deckManager.discardPile[i].strName = EditorGUILayout.TextField("Name", deckManager.discardPile[i].strName);

                    // show the card's color
                    deckManager.discardPile[i].color = (Cards.Color)EditorGUILayout.EnumPopup("Color", deckManager.discardPile[i].color);

                    // show the card's rank
                    deckManager.discardPile[i].rank = (Cards.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.discardPile[i].rank);

                    // show the card's suit
                    deckManager.discardPile[i].suit = (Cards.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.discardPile[i].suit);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }
        } catch
        {
            return;
        }

        EditorGUILayout.Space();
    }
}
