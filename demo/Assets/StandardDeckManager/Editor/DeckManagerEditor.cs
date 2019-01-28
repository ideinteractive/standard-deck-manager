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
    private ReorderableList reorderableDeck;
    private ReorderableList reorderableDiscardPile;
    private ReorderableList reorderableInUsePile;

    // when this is enabled and active
    private void OnEnable()
    {
        Instance = this;    // set up this instance
        deckManager = (DeckManager)target;  // set up the reference to the current inspected object

        // create our reorderable list
        reorderableDeck = new ReorderableList(serializedObject, serializedObject.FindProperty("deck"));
        reorderableDiscardPile = new ReorderableList(serializedObject, serializedObject.FindProperty("discardPile"));
        reorderableInUsePile = new ReorderableList(serializedObject, serializedObject.FindProperty("inUsePile"));

        // draw out our column headers
        // and reorderable lists
        reorderableDeck.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        };
        reorderableDeck.drawElementCallback += DrawDeckElement;
        reorderableDeck.onSelectCallback += SelectCard;
        reorderableDeck.onAddDropdownCallback = DrawGenericMenu;
        reorderableDeck.onRemoveCallback = RemoveCard;

        reorderableDiscardPile.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        };
        reorderableDiscardPile.drawElementCallback += DrawDiscardPileElement;
        reorderableDiscardPile.onSelectCallback += SelectCard;
        reorderableDiscardPile.onAddDropdownCallback = DrawGenericMenu;
        reorderableDiscardPile.onRemoveCallback = RemoveCard;

        reorderableInUsePile.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        };
        reorderableInUsePile.drawElementCallback += DrawInUsePileElement;
        reorderableInUsePile.onSelectCallback += SelectCard;
        reorderableInUsePile.onAddDropdownCallback = DrawGenericMenu;
        reorderableInUsePile.onRemoveCallback = RemoveCard;
    }

    // if this script is destroyed or removed
    private void OnDestroy()
    {
        // remove a reference to this editor
        Instance = null;
    }

    // callback for when we select a card in the deck
    private void SelectCard(ReorderableList deck)
    {
        if (0 <= deck.index && deck.index < deck.count)
        {
            if (CardEditor.IsOpen)
            {
                // pass our information to the card editor
                CardEditor.Instance.intCardIndex = deck.index;

                if (deck == reorderableDeck)
                    CardEditor.Instance.blnEditingCardFromDeck = true;
                else
                    CardEditor.Instance.blnEditingCardFromDeck = false;

                if (deck == reorderableDiscardPile)
                    CardEditor.Instance.blnEditingCardFromDiscard = true;
                else
                    CardEditor.Instance.blnEditingCardFromDiscard = false;

                if (deck == reorderableInUsePile)
                    CardEditor.Instance.blnEditingCardFromInUse = true;
                else
                    CardEditor.Instance.blnEditingCardFromInUse = false;
            }
        }
    }

    // callback for when we remove a card from the deck
    private void RemoveCard(ReorderableList deck)
    {
        if (0 <= deck.index && deck.index < deck.count)
        {
            // determine which deck we are removing the card from
            if (deck == reorderableDeck)
            {
                Undo.RecordObjects(targets, "Card removed from deck.");
                DestroyAndRemoveCard(deckManager.deck, deck.index);
                serializedObject.ApplyModifiedProperties();

            }
            else if (deck == reorderableDiscardPile)
            {
                Undo.RecordObjects(targets, "Card removed from discard pile.");
                DestroyAndRemoveCard(deckManager.discardPile, deck.index);
                serializedObject.ApplyModifiedProperties();
            }
            else if (deck == reorderableInUsePile)
            {
                Undo.RecordObjects(targets, "Card removed from in use pile.");
                DestroyAndRemoveCard(deckManager.inUsePile, deck.index);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    // destroy and remove our card object
    private void DestroyAndRemoveCard(List<Card> deck, int index)
    {
        // if the application is playing
        if (Application.isPlaying)
            // delete the instantiated object
            Destroy(deck[index].card);

        // remove the card from the deck
        deck.Remove(deck[index]);
    }

    // draw our our elements
    private void DrawElements(Rect rect, Card card, List<Card> deck)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 2, rect.width, rect.height), card.color + " " + card.rank + " of " + card.suit);

        // if it is the main deck
        if (deck == deckManager.deck)
        {
            // move the card to the discard pile
            if (GUI.Button(new Rect(rect.width - 40, rect.y, 20, 15), "D"))
            {
                Undo.RecordObjects(targets, "Card moved to discard pile.");
                deckManager.MoveToDiscard(card, deckManager.deck);
            }
            // move the card to the in use pile
            if (GUI.Button(new Rect(rect.width - 15, rect.y, 20, 15), "U"))
            {
                Undo.RecordObjects(targets, "Card moved to in use pile.");
                deckManager.MoveToInUse(card, deckManager.deck);
            }
        }

        // if it is the discard pile
        if (deck == deckManager.discardPile)
        {
            // move the card to the deck
            if (GUI.Button(new Rect(rect.width - 40, rect.y, 20, 15), "A"))
            {
                Undo.RecordObjects(targets, "Card moved to deck.");
                deckManager.MoveToDeck(card, deckManager.discardPile);
            }
            // move the card to the in use pile
            if (GUI.Button(new Rect(rect.width - 15, rect.y, 20, 15), "U"))
            {
                Undo.RecordObjects(targets, "Card moved to in use pile.");
                deckManager.MoveToInUse(card, deckManager.discardPile);
            }
        }

        // if it is the in use pile
        if (deck == deckManager.inUsePile)
        {
            // move the card to the deck
            if (GUI.Button(new Rect(rect.width - 40, rect.y, 20, 15), "A"))
            {
                Undo.RecordObjects(targets, "Card moved to deck.");
                deckManager.MoveToDeck(card, deckManager.inUsePile);
            }
            // move the card to the discard pile
            if (GUI.Button(new Rect(rect.width - 15, rect.y, 20, 15), "D"))
            {
                Undo.RecordObjects(targets, "Card moved to discard pile.");
                deckManager.MoveToDiscard(card, deckManager.inUsePile);
            }
        }

        // edit the card with the card editor
        if (GUI.Button(new Rect(rect.width + 10, rect.y, 20, 15), "E"))
        {
            // get existing open window or if none, make a new one
            CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
            window.minSize = new Vector2(325, 140);
            window.Show();

            // pass our information to the card editor
            CardEditor.Instance.intCardIndex = deck.IndexOf(card);

            if (deck == deckManager.deck)
                CardEditor.Instance.blnEditingCardFromDeck = true;
            else
                CardEditor.Instance.blnEditingCardFromDeck = false;

            if (deck == deckManager.discardPile)
                CardEditor.Instance.blnEditingCardFromDiscard = true;
            else
                CardEditor.Instance.blnEditingCardFromDiscard = false;

            if (deck == deckManager.inUsePile)
                CardEditor.Instance.blnEditingCardFromInUse = true;
            else
                CardEditor.Instance.blnEditingCardFromInUse = false;
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

    // draw our generic menu for adding in new cards
    private void DrawGenericMenu(Rect rect, ReorderableList deck)
    {
        var menu = new GenericMenu();
        for (int c = 0; c < 2; c++)
        {
            // if c is an odd number
            // return red
            if (c % 2 == 1)
            {
                for (int s = 0; s < 2; s++)
                {
                    // if s is an odd number
                    // return red
                    if (s % 2 == 1)
                        for (int r = 0; r < 13; r++)
                        {
                            Card card = new Card
                            {
                                color = Card.Color.Red,
                                suit = Card.Suit.Hearts,
                                rank = (Card.Rank)r
                            };
                            card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                            AddItem(new GUIContent("Red/Hearts/" + (Card.Rank)r), false, card, deck, menu);
                        }
                    else
                        for (int r = 0; r < 13; r++)
                        {
                            Card card = new Card
                            {
                                color = Card.Color.Red,
                                suit = Card.Suit.Diamonds,
                                rank = (Card.Rank)r
                            };
                            card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                            AddItem(new GUIContent("Red/Diamonds/" + (Card.Rank)r), false, card, deck, menu);
                        }
                }
            }
            else
            {
                for (int s = 0; s < 2; s++)
                {
                    // if s is an odd number
                    // return red
                    if (s % 2 == 1)
                        for (int r = 0; r < 13; r++)
                        {
                            Card card = new Card
                            {
                                color = Card.Color.Black,
                                suit = Card.Suit.Spades,
                                rank = (Card.Rank)r
                            };
                            card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                            AddItem(new GUIContent("Black/Spades/" + (Card.Rank)r), false, card, deck, menu);
                        }
                    else
                        for (int r = 0; r < 13; r++)
                        {
                            Card card = new Card
                            {
                                color = Card.Color.Black,
                                suit = Card.Suit.Clubs,
                                rank = (Card.Rank)r
                            };
                            card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
                            AddItem(new GUIContent("Black/Clubs/" + (Card.Rank)r), false, card, deck, menu);
                        }
                }
            }
        }
        menu.ShowAsContext();
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

        // remove everything and create a new standard deck
        if (GUILayout.Button("Generate a New Deck"))
        {
            Undo.RecordObjects(targets, "New deck generated.");
            deckManager.RemoveAllAndCreateNew();
        }

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
                reorderableDeck.DoLayoutList();
            }
            catch
            {
                return;
            }

        deckManager.blnExpandDiscardPnl = EditorGUILayout.Foldout(deckManager.blnExpandDiscardPnl, "Discard Pile [" + deckManager.discardPile.Count + "]");
        if (deckManager.blnExpandDiscardPnl)
            try
            {
                reorderableDiscardPile.DoLayoutList();
            }
            catch
            {
                return;
            }

        deckManager.blnExpandInUsePnl = EditorGUILayout.Foldout(deckManager.blnExpandInUsePnl, "In Use Pile [" + deckManager.inUsePile.Count + "]");
        if (deckManager.blnExpandInUsePnl)
            try
            {
                reorderableInUsePile.DoLayoutList();
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

    // add an item to the appropriate list
    private void AddItem(GUIContent content, bool on, Card card, ReorderableList deck, GenericMenu menu)
    {
        if (deck == reorderableDeck)
            menu.AddItem(content, on, AddNewCardToDeck, card);
        else if (deck == reorderableDiscardPile)
            menu.AddItem(content, on, AddNewCardToDiscardPile, card);
        else if (deck == reorderableInUsePile)
            menu.AddItem(content, on, AddNewCardToInUsePile, card);
    }

    // spawn the card from the deck
    private void SpawnCard(Card card)
    {
        // if the application is playing
        if (Application.isPlaying)
        {
            // instantiate the object
            Card tempCard = (Card)card;
            GameObject goCard = Instantiate(tempCard.card, deckManager.transform.position, deckManager.transform.rotation);
            goCard.SetActive(false);
            goCard.transform.parent = deckManager.goPool.transform;
            card.card = goCard;
            goCard.name = tempCard.color + " " + tempCard.rank + " of " + tempCard.suit;
        }
    }

    // add in a new card to deck
    private void AddNewCardToDeck(object card)
    {
        Undo.RecordObjects(targets, "Added card to deck.");
        deckManager.deck.Add((Card)card);
        serializedObject.ApplyModifiedProperties();
        SpawnCard((Card)card);
    }

    // add in a new card to discard pile
    private void AddNewCardToDiscardPile(object card)
    {
        Undo.RecordObjects(targets, "Added card to discard pile.");
        deckManager.discardPile.Add((Card)card);
        serializedObject.ApplyModifiedProperties();
        SpawnCard((Card)card);
    }

    // add in a new card to in use pile
    private void AddNewCardToInUsePile(object card)
    {
        Undo.RecordObjects(targets, "Added card to in use pile.");
        deckManager.inUsePile.Add((Card)card);
        serializedObject.ApplyModifiedProperties();
        SpawnCard((Card)card);
    }
}