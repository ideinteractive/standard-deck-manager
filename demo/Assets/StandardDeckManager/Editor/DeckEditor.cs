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

        // if the deck and discard pile is empty
        if (deckManager.deck.Count == 0 && deckManager.discardPile.Count == 0)
            // prefill the deck
            deckManager.RemoveAllAndCreateNew();
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
        EditorGUILayout.LabelField("Deck Options", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // shuffle the deck
        if (GUILayout.Button("Shuffle Deck"))
            deckManager.Shuffle();

        // shuffle the discard pile
        if (GUILayout.Button("Shuffle Discard Pile"))
            deckManager.ShuffleDiscardPile();

        // shuffle the deck with the discard pile
        if (GUILayout.Button("Shuffle Deck with Discard Pile"))
            deckManager.ShuffleWithDiscard();

        // shuffle the discard pile and add it to the top of the deck
        if (GUILayout.Button("Shuffle Discard Pile to Top"))
            deckManager.ShuffleDiscardToTop();

        // shuffle the discard pile and add it to the bottom of the deck
        if (GUILayout.Button("Shuffle Discard Pile to Bottom"))
            deckManager.ShuffleDiscardToBottom();

        // add a new card to the deck
        if (GUILayout.Button("Add New Card to Deck"))
        {
            Cards card = new Cards();
            card.color = (Cards.Color)Random.Range(0, (int)Cards.Color.Red + 1);
            card.rank = (Cards.Rank)Random.Range(0, (int)Cards.Rank.King + 1);
            card.suit = (Cards.Suit)Random.Range(0, (int)Cards.Suit.Spades + 1);
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckManager.AddNewCard(card, deckManager.deck);
        }

        // add a new card to the discard pile
        if (GUILayout.Button("Add New Card to Discard Pile"))
        {
            Cards card = new Cards();
            card.color = (Cards.Color)Random.Range(0, (int)Cards.Color.Red + 1);
            card.rank = (Cards.Rank)Random.Range(0, (int)Cards.Rank.King + 1);
            card.suit = (Cards.Suit)Random.Range(0, (int)Cards.Suit.Spades + 1);
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckManager.AddNewCard(card, deckManager.discardPile);
        }

        // add a new card to the discard pile
        if (GUILayout.Button("Remove Everything and Add Standard Deck"))
        {
            deckManager.RemoveAllAndCreateNew();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // show the card's back face object
        deckManager.cardBackFace = (GameObject)EditorGUILayout.ObjectField("Card Back Face", deckManager.cardBackFace, typeof(GameObject), true);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Deck and Discard Pile", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        deckManager.blnExpandDeckPnl = EditorGUILayout.Foldout(deckManager.blnExpandDeckPnl, "Deck [" + deckManager.deck.Count.ToString() + "]");
        EditorGUILayout.EndHorizontal();

        // if the deck's panel is expanded
        if (deckManager.blnExpandDeckPnl)
        {
            // try and render the card info
            try
            {
                // for each card inside the deck starting from the lowest to highest (order)
                for (int i = 0; i < deckManager.deck.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(stylePaddingLeft);

                    // expand or collapse the card's panel depending on the boolean status
                    // and name the panel based on the card
                    deckManager.deck[i].blnExpandCardPnl = EditorGUILayout.Foldout(deckManager.deck[i].blnExpandCardPnl, deckManager.deck[i].strName);

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
                    if (deckManager.deck[i].blnExpandCardPnl)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(stylePaddingLeft);

                        // automatically assign the card name if it doesn't exist
                        deckManager.deck[i].strName = deckManager.deck[i].color.ToString() + " " + deckManager.deck[i].rank.ToString() + " of " + deckManager.deck[i].suit.ToString();

                        // show the card's color
                        deckManager.deck[i].color = (Cards.Color)EditorGUILayout.EnumPopup("Color", deckManager.deck[i].color);

                        // show the card's rank
                        deckManager.deck[i].rank = (Cards.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.deck[i].rank);

                        // show the card's suit
                        deckManager.deck[i].suit = (Cards.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.deck[i].suit);

                        // show the card's object
                        deckManager.deck[i].card = (GameObject)EditorGUILayout.ObjectField("Card", deckManager.deck[i].card, typeof(GameObject), true);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }
                }
            }
            catch
            {
                return;
            }
        }

        EditorGUILayout.Space();

        // discard pile

        EditorGUILayout.BeginHorizontal();
        deckManager.blnExpandDiscardPnl = EditorGUILayout.Foldout(deckManager.blnExpandDiscardPnl, "Discard Pile [" + deckManager.discardPile.Count.ToString() + "]");
        EditorGUILayout.EndHorizontal();

        // if the discard panel is expanded
        if (deckManager.blnExpandDiscardPnl)
        {
            // try and render the card info
            try
            {
                // for each card inside the discard pile starting from the lowest to highest (order)
                for (int i = 0; i < deckManager.discardPile.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(stylePaddingLeft);

                    // expand or collapse the card's panel depending on the boolean status
                    // and name the panel based on the card
                    deckManager.discardPile[i].blnExpandCardPnl = EditorGUILayout.Foldout(deckManager.discardPile[i].blnExpandCardPnl, deckManager.discardPile[i].strName);

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
                    if (deckManager.discardPile[i].blnExpandCardPnl)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(stylePaddingLeft);

                        // automatically assign the card name if it doesn't exist
                        deckManager.discardPile[i].strName = deckManager.discardPile[i].color.ToString() + " " + deckManager.discardPile[i].rank.ToString() + " of " + deckManager.discardPile[i].suit.ToString();

                        // show the card's color
                        deckManager.discardPile[i].color = (Cards.Color)EditorGUILayout.EnumPopup("Color", deckManager.discardPile[i].color);

                        // show the card's rank
                        deckManager.discardPile[i].rank = (Cards.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.discardPile[i].rank);

                        // show the card's suit
                        deckManager.discardPile[i].suit = (Cards.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.discardPile[i].suit);

                        // show the card's object
                        deckManager.discardPile[i].card = (GameObject)EditorGUILayout.ObjectField("Card", deckManager.discardPile[i].card, typeof(GameObject), true);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }
                }
            }
            catch
            {
                return;
            }
        }

        EditorGUILayout.Space();
    }
}
