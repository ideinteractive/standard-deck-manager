using System.Collections.Generic;
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

    // outputs the default buttons every deck has
    public void DefaultButtons(List<Card> deck, int i)
    {
        // move the card up on the list
        if (GUILayout.Button("U", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
            deckManager.MoveCardUp(deck[i], deck);

        // move the card down on the list
        if (GUILayout.Button("D", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
            deckManager.MoveCardDown(deck[i], deck);

        // move the card to the top of the list
        if (GUILayout.Button("T", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
            deckManager.MoveCardToTop(deck[i], deck);

        // move the card to the bottom of the list
        if (GUILayout.Button("B", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
            deckManager.MoveCardToBottom(deck[i], deck);

        // remove the card from the list
        if (GUILayout.Button("R", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
            deckManager.RemoveCard(deck[i], deck);
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
        EditorGUILayout.LabelField("Deck / Discard Pile / In Use Pile", EditorStyles.boldLabel);
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

                    // render our default buttons
                    DefaultButtons(deckManager.deck, i);

                    // move the card to the discard pile
                    if (GUILayout.Button("-", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                        deckManager.DiscardCard(deckManager.deck[i], deckManager.deck);

                    // move the card to the in use pile
                    if (GUILayout.Button("IU", GUILayout.MinWidth(25.0f), GUILayout.MaxWidth(25.0f)))
                        deckManager.MoveToInUse(deckManager.deck[i], deckManager.deck);

                    EditorGUILayout.EndHorizontal();

                    // if the card's panel is expanded
                    if (deckManager.deck[i].blnExpandCardPnl)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(stylePaddingLeft);

                        // automatically assign the card name if it doesn't exist
                        deckManager.deck[i].strName = deckManager.deck[i].color.ToString() + " " + deckManager.deck[i].rank.ToString() + " of " + deckManager.deck[i].suit.ToString();

                        // show the card's color
                        deckManager.deck[i].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deckManager.deck[i].color);

                        // show the card's rank
                        deckManager.deck[i].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.deck[i].rank);

                        // show the card's suit
                        deckManager.deck[i].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.deck[i].suit);

                        // show the card's value
                        deckManager.deck[i].value = EditorGUILayout.IntField("Value", deckManager.deck[i].value);

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

                    // render our default buttons
                    DefaultButtons(deckManager.discardPile, i);

                    // move the card to the deck
                    if (GUILayout.Button("+", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                        deckManager.MoveCardToDeck(deckManager.discardPile[i], deckManager.discardPile, deckManager.deck);

                    // move the card to the in use pile
                    if (GUILayout.Button("IU", GUILayout.MinWidth(25.0f), GUILayout.MaxWidth(25.0f)))
                        deckManager.MoveToInUse(deckManager.discardPile[i], deckManager.discardPile);

                    EditorGUILayout.EndHorizontal();

                    // if the card's panel is expanded
                    if (deckManager.discardPile[i].blnExpandCardPnl)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(stylePaddingLeft);

                        // automatically assign the card name if it doesn't exist
                        deckManager.discardPile[i].strName = deckManager.discardPile[i].color.ToString() + " " + deckManager.discardPile[i].rank.ToString() + " of " + deckManager.discardPile[i].suit.ToString();

                        // show the card's color
                        deckManager.discardPile[i].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deckManager.discardPile[i].color);

                        // show the card's rank
                        deckManager.discardPile[i].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.discardPile[i].rank);

                        // show the card's suit
                        deckManager.discardPile[i].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.discardPile[i].suit);

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

        // in use pile

        EditorGUILayout.BeginHorizontal();
        deckManager.blnExpandInUsePnl = EditorGUILayout.Foldout(deckManager.blnExpandInUsePnl, "In Use Pile [" + deckManager.inUsePile.Count.ToString() + "]");
        EditorGUILayout.EndHorizontal();

        // if the in use panel is expanded
        if (deckManager.blnExpandInUsePnl)
        {
            // try and render the card info
            try
            {
                // for each card inside the in use pile starting from the lowest to highest (order)
                for (int i = 0; i < deckManager.inUsePile.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(stylePaddingLeft);

                    // expand or collapse the card's panel depending on the boolean status
                    // and name the panel based on the card
                    deckManager.inUsePile[i].blnExpandCardPnl = EditorGUILayout.Foldout(deckManager.inUsePile[i].blnExpandCardPnl, deckManager.inUsePile[i].strName);

                    // render our default buttons
                    DefaultButtons(deckManager.inUsePile, i);

                    // move the card to the deck
                    if (GUILayout.Button("+", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                        deckManager.MoveCardToDeck(deckManager.inUsePile[i], deckManager.inUsePile, deckManager.deck);

                    // move the card to the discard pile
                    if (GUILayout.Button("-", GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f)))
                        deckManager.DiscardCard(deckManager.inUsePile[i], deckManager.inUsePile);

                    EditorGUILayout.EndHorizontal();

                    // if the card's panel is expanded
                    if (deckManager.inUsePile[i].blnExpandCardPnl)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(stylePaddingLeft);

                        // automatically assign the card name if it doesn't exist
                        deckManager.inUsePile[i].strName = deckManager.inUsePile[i].color.ToString() + " " + deckManager.inUsePile[i].rank.ToString() + " of " + deckManager.inUsePile[i].suit.ToString();

                        // show the card's color
                        deckManager.inUsePile[i].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deckManager.inUsePile[i].color);

                        // show the card's rank
                        deckManager.inUsePile[i].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deckManager.inUsePile[i].rank);

                        // show the card's suit
                        deckManager.inUsePile[i].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deckManager.inUsePile[i].suit);

                        // show the card's object
                        deckManager.inUsePile[i].card = (GameObject)EditorGUILayout.ObjectField("Card", deckManager.inUsePile[i].card, typeof(GameObject), true);

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

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // show the card's back face object
        deckManager.cardBackFace = (GameObject)EditorGUILayout.ObjectField("Card Back Face", deckManager.cardBackFace, typeof(GameObject), true);

        EditorGUILayout.Space();
    }
}
