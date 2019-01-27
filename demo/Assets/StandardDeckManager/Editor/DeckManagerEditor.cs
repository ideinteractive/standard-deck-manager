using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/// <summary>
/// DeckManagerEditor
/// Description: Custom editor to display all of the cards inside the Deck Manager.
/// </summary>

[CustomEditor(typeof(DeckManager))]
public class DeckManagerEditor : Editor
{
    // reference this editor
    public static DeckManagerEditor Instance { get; private set; }

    // reference to the DeckManager
    public DeckManager deckManager;

    // reference to our reorderable lists
    private ReorderableList deck;
    private ReorderableList discardPile;
    private ReorderableList inUsePile;

    // when this is enabled and active
    private void OnEnable()
    {
        Instance = this;    // set up this instance
        deckManager = (DeckManager)target;  // set up the reference to the current inspected object

        // create our reorderable list
        deck = new ReorderableList(serializedObject, serializedObject.FindProperty("deck"));
        discardPile = new ReorderableList(serializedObject, serializedObject.FindProperty("discardPile"));
        inUsePile = new ReorderableList(serializedObject, serializedObject.FindProperty("inUsePile"));

        // draw out our column headers
        // and reorderable lists
        deck.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        };
        deck.drawElementCallback += DrawDeckElement;
        deck.onSelectCallback += SelectCard;

        discardPile.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        };
        discardPile.drawElementCallback += DrawDiscardPileElement;
        discardPile.onSelectCallback += SelectCard;

        inUsePile.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        };
        inUsePile.drawElementCallback += DrawInUsePileElement;
        inUsePile.onSelectCallback += SelectCard;
    }

    // if this script is destroyed or removed
    public void OnDestroy()
    {
        // if we are using the editor
        if (Application.isEditor)
        {
            // if there is no deck manager found
            if (deckManager == null)
            {
                // get existing open windows and close it
                DeckOptionsEditor doeWindow = (DeckOptionsEditor)EditorWindow.GetWindow(typeof(DeckOptionsEditor), false, "Deck Options");
                doeWindow.Close();
                CardEditor ceWindow = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
                ceWindow.Close();
            }

            // remove a reference to this editor
            Instance = null;

            // clear all windows of references
            CardEditor.Instance.blnEditingCardFromDeck = false;
            CardEditor.Instance.blnEditingCardFromDiscard = false;
            CardEditor.Instance.blnEditingCardFromInUse = false;
        }
    }

    // callback for when we select a card in the deck
    private void SelectCard(ReorderableList _deck)
    {
        if (0 <= _deck.index && _deck.index < _deck.count)
        {
            if (CardEditor.IsOpen)
            {
                // pass our information to the card editor
                CardEditor.Instance.intCardIndex = _deck.index;

                if (_deck == deck)
                    CardEditor.Instance.blnEditingCardFromDeck = true;
                else
                    CardEditor.Instance.blnEditingCardFromDeck = false;

                if (_deck == discardPile)
                    CardEditor.Instance.blnEditingCardFromDiscard = true;
                else
                    CardEditor.Instance.blnEditingCardFromDiscard = false;

                if (_deck == inUsePile)
                    CardEditor.Instance.blnEditingCardFromInUse = true;
                else
                    CardEditor.Instance.blnEditingCardFromInUse = false;
            }
        }
    }

    // draw our our elements
    private void DrawElements(Rect rect, Card card, List<Card> _deck)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 2, rect.width, rect.height), card.color + " " + card.rank + " of " + card.suit);

        // if it is the main deck
        if (_deck == deckManager.deck)
        {
            // move the card to the discard pile
            if (GUI.Button(new Rect(rect.width - 95, rect.y, 60, 15), "Discard"))
            {
                Undo.RecordObjects(targets, "Card moved to discard pile.");
                deckManager.MoveToDiscard(card, deckManager.deck);
            }
            // move the card to the in use pile
            if (GUI.Button(new Rect(rect.width - 30, rect.y, 60, 15), "Use"))
            {
                Undo.RecordObjects(targets, "Card moved to in use pile.");
                deckManager.MoveToInUse(card, deckManager.deck);
            }
        }

        // if it is the discard pile
        if (_deck == deckManager.discardPile)
        {
            // move the card to the deck
            if (GUI.Button(new Rect(rect.width - 95, rect.y, 60, 15), "Deck"))
            {
                Undo.RecordObjects(targets, "Card moved to deck.");
                deckManager.MoveToDeck(card, deckManager.discardPile);
            }
            // move the card to the in use pile
            if (GUI.Button(new Rect(rect.width - 30, rect.y, 60, 15), "Use"))
            {
                Undo.RecordObjects(targets, "Card moved to in use pile.");
                deckManager.MoveToInUse(card, deckManager.discardPile);
            }
        }

        // if it is the in use pile
        if (_deck == deckManager.inUsePile)
        {
            // move the card to the deck
            if (GUI.Button(new Rect(rect.width - 95, rect.y, 60, 15), "Deck"))
            {
                Undo.RecordObjects(targets, "Card moved to deck.");
                deckManager.MoveToDeck(card, deckManager.inUsePile);
            }
            // move the card to the discard pile
            if (GUI.Button(new Rect(rect.width - 30, rect.y, 60, 15), "Discard"))
            {
                Undo.RecordObjects(targets, "Card moved to discard pile.");
                deckManager.MoveToDiscard(card, deckManager.inUsePile);
            }
        }
    }

    // draw our our elements for the deck
    private void DrawDeckElement(Rect rect, int index, bool active, bool focused)
    {
        Card card = deckManager.deck[index];

        // draw our our elements
        DrawElements(rect, card, deckManager.deck);
    }

    // draw our our elements for the discard pile
    private void DrawDiscardPileElement(Rect rect, int index, bool active, bool focused)
    {
        Card card = deckManager.discardPile[index];

        // draw our our elements
        DrawElements(rect, card, deckManager.discardPile);
    }

    // draw our our elements for the in use pile
    private void DrawInUsePileElement(Rect rect, int index, bool active, bool focused)
    {
        Card card = deckManager.inUsePile[index];

        // draw our our elements
        DrawElements(rect, card, deckManager.inUsePile);
    }

    // override our inspector interface
    public override void OnInspectorGUI()
    {
        // update serialized object representation
        serializedObject.Update();

        // if the instance is empty
        if (Instance == null)
            // set this instance
            Instance = this;

        /// GUI STYLES

        // header styles
        GUIStyle styleRowHeader = new GUIStyle();
        styleRowHeader.padding = new RectOffset(0, 0, 3, 3);
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        GUIStyle stylePaddingLeft = new GUIStyle();
        stylePaddingLeft.padding = new RectOffset(10, 0, 3, 3);

        /// EDITOR

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Deck Manager Tools", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // get existing open window or if none, make a new one
        if (GUILayout.Button("Deck Options"))
        {
            DeckOptionsEditor window = (DeckOptionsEditor)EditorWindow.GetWindow(typeof(DeckOptionsEditor), false, "Deck Options");
            window.Show();
        }

        // optn the card editor window
        if (GUILayout.Button("Card Editor"))
        {
            // get existing open window or if none, make a new one
            CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
            window.minSize = new Vector2(325, 140);
            window.Show();
        }

        // remove everything and create a new standard deck
        if (GUILayout.Button("Generate a New Deck"))
        {
            Undo.RecordObjects(targets, "New deck generated.");
            deckManager.RemoveAllAndCreateNew();
        }

        // remove everything
        if (GUILayout.Button("Remove All Cards"))
        {
            Undo.RecordObjects(targets, "All card removed.");
            deckManager.RemoveAll();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("In Use Pile / Deck / Discard Pile Cards", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        deckManager.blnExpandDeckPnl = EditorGUILayout.Foldout(deckManager.blnExpandDeckPnl, "Deck [" + deckManager.deck.Count + "]");
        if (deckManager.blnExpandDeckPnl)
            try
            {
                deck.DoLayoutList();
            }
            catch
            {
                return;
            }

        deckManager.blnExpandDiscardPnl = EditorGUILayout.Foldout(deckManager.blnExpandDiscardPnl, "Discard Pile [" + deckManager.discardPile.Count + "]");
        if (deckManager.blnExpandDiscardPnl)
            try
            {
                discardPile.DoLayoutList();
            }
            catch
            {
                return;
            }

        deckManager.blnExpandInUsePnl = EditorGUILayout.Foldout(deckManager.blnExpandInUsePnl, "In Use Pile [" + deckManager.inUsePile.Count + "]");
        if (deckManager.blnExpandInUsePnl)
            try
            {
                inUsePile.DoLayoutList();
            }
            catch
            {
                return;
            }

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);

        // apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}