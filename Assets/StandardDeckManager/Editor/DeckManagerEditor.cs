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
    public static DeckManagerEditor instance { get; private set; }  // create a reference to this editor
    public DeckManager deckManager; // reference our deck manager
    private ReorderableList reorderableDeck;    // create a reorderable list for our deck
    private ReorderableList reorderableDiscardPile; // create a reorderable list for our discard pile
    private ReorderableList reorderableInUsePile;   // create a reoderable list for our in use pile

    // on enable
    private void OnEnable()
    {
        instance = this;    // set this instance
        deckManager = (DeckManager)target;  // set up the reference to the current inspected object

        // create our reorderable list
        reorderableDeck = new ReorderableList(serializedObject, serializedObject.FindProperty("deck"));
        reorderableDiscardPile = new ReorderableList(serializedObject, serializedObject.FindProperty("discardPile"));
        reorderableInUsePile = new ReorderableList(serializedObject, serializedObject.FindProperty("inUsePile"));

        // draw out our column headers and reorderable lists for our deck, discard pile and in use pile
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
        instance = null;
    }

    // callback for when we select a card in the deck
    private void SelectCard(ReorderableList deck)
    {
        // if the deck index is greater than 0 and is less than the deck count
        if (deck.index >= 0 && deck.index < deck.count)
        {
            // check if the card editor is open
            if (CardEditor.IsOpen)
            {
                // pass our information to the card editor
                CardEditor.instance.intCardIndex = deck.index;

                // depending on which reorderable list we are using check it off in the card editor
                if (deck == reorderableDeck)
                    CardEditor.instance.blnEditingCardFromDeck = true;
                else
                    CardEditor.instance.blnEditingCardFromDeck = false;

                if (deck == reorderableDiscardPile)
                    CardEditor.instance.blnEditingCardFromDiscard = true;
                else
                    CardEditor.instance.blnEditingCardFromDiscard = false;

                if (deck == reorderableInUsePile)
                    CardEditor.instance.blnEditingCardFromInUse = true;
                else
                    CardEditor.instance.blnEditingCardFromInUse = false;
            }
        }
    }

    // callback for when we remove a card from the deck
    private void RemoveCard(ReorderableList deck)
    {
        if (0 <= deck.index && deck.index < deck.count) {
        serializedObject.Update();

        // determine which deck we are removing the card from
        if (deck == reorderableDeck) {
            var element = reorderableDeck.serializedProperty.GetArrayElementAtIndex(deck.index);
            Destroy(element.FindPropertyRelative("card").objectReferenceValue);
        }
        else if (deck == reorderableDiscardPile) {
            var element = reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(deck.index);
            Destroy(element.FindPropertyRelative("card").objectReferenceValue);
        }
        else if (deck == reorderableInUsePile) {
            var element = reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(deck.index);
            Destroy(element.FindPropertyRelative("card").objectReferenceValue);
        }

        // remove our item
        ReorderableList.defaultBehaviours.DoRemoveButton(deck);

        serializedObject.ApplyModifiedProperties(); 
    }
    }

    // add an item to the appropriate reorderable list
    private void AddItem(GUIContent content, bool on, Card card, ReorderableList deck, GenericMenu menu)
    {
        if (deck == reorderableDeck) {
            menu.AddItem(content, false, () => {
                serializedObject.Update();
                // create a new item in our reoderable list
                var index = reorderableDeck.serializedProperty.arraySize;
                reorderableDeck.serializedProperty.arraySize++;
                reorderableDeck.index = index;
                var element = reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // spawn our card
                SpawnCard((Card)card, element.FindPropertyRelative("card"));
                serializedObject.ApplyModifiedProperties(); 
            });
        }
        else if (deck == reorderableDiscardPile) {
            menu.AddItem(content, false, () => {
                serializedObject.Update();
                // create a new item in our reoderable list
                var index = reorderableDiscardPile.serializedProperty.arraySize;
                reorderableDiscardPile.serializedProperty.arraySize++;
                reorderableDiscardPile.index = index;
                var element = reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // spawn our card
                SpawnCard((Card)card, element.FindPropertyRelative("card"));
                serializedObject.ApplyModifiedProperties(); 
            });
        }
        else if (deck == reorderableInUsePile) {
             menu.AddItem(content, false, () => {
                serializedObject.Update();
                // create a new item in our reoderable list
                var index = reorderableInUsePile.serializedProperty.arraySize;
                reorderableInUsePile.serializedProperty.arraySize++;
                reorderableInUsePile.index = index;
                var element = reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // spawn our card
                SpawnCard((Card)card, element.FindPropertyRelative("card"));
                serializedObject.ApplyModifiedProperties(); 
            });
        }
    }

    // generate a new deck
    public void RemoveAllAndCreateNew()
    {
        // remove all cards and generate new deck
        Undo.RecordObjects(targets, "Generate new deck.");
        deckManager.RemoveAll();

        // while the deck count is under 52
        while (DeckManager.instance.deck.Count < 52)
        {
            // for each suit 
            for (int i = 0; i <= (int)Card.Suit.Hearts; i++)
            {
                // if i is an odd number
                if (i % 2 == 1)
                {
                    // for each card
                    for (int c = 0; c <= (int)Card.Rank.King; c++)
                    {
                        // create a new card and add it to the deck
                        // add the card to the deck
                        Card card = new Card
                        {
                            suit = (Card.Suit)i,
                            color = (Card.Color)1,
                            rank = (Card.Rank)c,
                            value = c + 1
                        };
         
                        serializedObject.Update();
                        // create a new item in our reoderable list
                        var index = reorderableDeck.serializedProperty.arraySize;
                        reorderableDeck.serializedProperty.arraySize++;
                        reorderableDeck.index = index;
                        var element = reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                        // assign our properties
                        element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                        element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                        element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                        element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                        element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                        // spawn our card
                        SpawnCard((Card)card, element.FindPropertyRelative("card"));
                        serializedObject.ApplyModifiedProperties(); 
                    }
                }
                else
                {
                    // for each card
                    for (int c = 0; c <= (int)Card.Rank.King; c++)
                    {
                        // create a new card and add it to the deck
                        // add the card to the deck
                        Card card = new Card
                        {
                            suit = (Card.Suit)i,
                            color = (Card.Color)0,
                            rank = (Card.Rank)c,
                            value = c + 1
                        };
                        
                        serializedObject.Update();
                        // create a new item in our reoderable list
                        var index = reorderableDeck.serializedProperty.arraySize;
                        reorderableDeck.serializedProperty.arraySize++;
                        reorderableDeck.index = index;
                        var element = reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                        // assign our properties
                        element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                        element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                        element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                        element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                        element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                        // spawn our card
                        SpawnCard((Card)card, element.FindPropertyRelative("card"));
                        serializedObject.ApplyModifiedProperties(); 
                    }
                }
            }
        }

        // inform the user the deck has been updated
        Debug.Log("Standard 52 Playing Card Deck - Imported");
    }

    // spawn a card from the deck
    private void SpawnCard(Card card, SerializedProperty deckCard)
    {
        // if the application is playing
        if (Application.isPlaying)
        {
            // instantiate the object
            Card tempCard = (Card)card;
            GameObject goCard = Instantiate(tempCard.card, deckManager.transform.position, deckManager.transform.rotation);
            goCard.SetActive(false);
            goCard.transform.parent = deckManager.goPool.transform;
            goCard.name = tempCard.color + " " + tempCard.rank + " of " + tempCard.suit;
            deckCard.objectReferenceValue = goCard;
        }
    }

    // draw our elements
    private void DrawElements(Rect rect, Card card, List<Card> deck)
    {
        // output the card color, rank and suit
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 2, rect.width, rect.height), card.color + " " + card.rank + " of " + card.suit);

        // if it is the main deck
        if (deck == deckManager.deck)
        {
            // if the discard card button is pressed
            if (GUI.Button(new Rect(rect.width - 70, rect.y, 30, 15), "DP"))
            {
                serializedObject.Update();

                // create a new item in our reoderable list
                var index = reorderableDiscardPile.serializedProperty.arraySize;
                reorderableDiscardPile.serializedProperty.arraySize++;
                reorderableDiscardPile.index = index;
                var element = reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // remove our item from the previous list
                serializedObject.FindProperty("deck").DeleteArrayElementAtIndex(deckManager.deck.IndexOf(card));

                serializedObject.ApplyModifiedProperties(); 
            }
            
            // if the in use card button is pressed
            if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "IU"))
            {
                serializedObject.Update();

                // create a new item in our reoderable list
                var index = reorderableInUsePile.serializedProperty.arraySize;
                reorderableInUsePile.serializedProperty.arraySize++;
                reorderableInUsePile.index = index;
                var element = reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // remove our item from the previous list
                serializedObject.FindProperty("deck").DeleteArrayElementAtIndex(deckManager.deck.IndexOf(card));

                serializedObject.ApplyModifiedProperties(); 
            }
        }

        // if it is the discard pile
        if (deck == deckManager.discardPile)
        {
            // if the deck button is pressed
            if (GUI.Button(new Rect(rect.width - 65, rect.y, 25, 15), "D"))
            {
                serializedObject.Update();

                // create a new item in our reoderable list
                var index = reorderableDeck.serializedProperty.arraySize;
                reorderableDeck.serializedProperty.arraySize++;
                reorderableDeck.index = index;
                var element = reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // remove our item from the previous list
                serializedObject.FindProperty("discardPile").DeleteArrayElementAtIndex(deckManager.discardPile.IndexOf(card));

                serializedObject.ApplyModifiedProperties(); 
            }

            // if the in use card button is pressed
            if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "IU"))
            {
                serializedObject.Update();

                // create a new item in our reoderable list
                var index = reorderableInUsePile.serializedProperty.arraySize;
                reorderableInUsePile.serializedProperty.arraySize++;
                reorderableInUsePile.index = index;
                var element = reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // remove our item from the previous list
                serializedObject.FindProperty("discardPile").DeleteArrayElementAtIndex(deckManager.discardPile.IndexOf(card));

                serializedObject.ApplyModifiedProperties(); 
            }
        }

        // if it is the in use pile
        if (deck == deckManager.inUsePile)
        {
            // if the deck button is pressed
            if (GUI.Button(new Rect(rect.width - 65, rect.y, 25, 15), "D"))
            {
                serializedObject.Update();

                // create a new item in our reoderable list
                var index = reorderableDeck.serializedProperty.arraySize;
                reorderableDeck.serializedProperty.arraySize++;
                reorderableDeck.index = index;
                var element = reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // remove our item from the previous list
                serializedObject.FindProperty("inUsePile").DeleteArrayElementAtIndex(deckManager.inUsePile.IndexOf(card));

                serializedObject.ApplyModifiedProperties(); 
            }

            // if the discard card button is pressed
            if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "DP"))
            {
                serializedObject.Update();

                // create a new item in our reoderable list
                var index = reorderableDiscardPile.serializedProperty.arraySize;
                reorderableDiscardPile.serializedProperty.arraySize++;
                reorderableDiscardPile.index = index;
                var element = reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card as GameObject;

                // remove our item from the previous list
                serializedObject.FindProperty("inUsePile").DeleteArrayElementAtIndex(deckManager.inUsePile.IndexOf(card));

                serializedObject.ApplyModifiedProperties(); 
            }
        }

        // if the edit card button is pressed
        if (GUI.Button(new Rect(rect.width, rect.y, 25, 15), "E"))
        {
            // edit the card with the card editor
            // get existing open window or if none, make a new one
            CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
            window.minSize = new Vector2(325, 140);
            window.Show();

            // pass our information to the card editor
            CardEditor.instance.intCardIndex = deck.IndexOf(card);

            // depending on which reorderable list we are using check it off in the card editor
            if (deck == deckManager.deck)
                CardEditor.instance.blnEditingCardFromDeck = true;
            else
                CardEditor.instance.blnEditingCardFromDeck = false;

            if (deck == deckManager.discardPile)
                CardEditor.instance.blnEditingCardFromDiscard = true;
            else
                CardEditor.instance.blnEditingCardFromDiscard = false;

            if (deck == deckManager.inUsePile)
                CardEditor.instance.blnEditingCardFromInUse = true;
            else
                CardEditor.instance.blnEditingCardFromInUse = false;
        }
    }


    // draw our elements for the deck
    private void DrawDeckElement(Rect rect, int index, bool active, bool focused)
    {
        // get the current card
        Card card = deckManager.deck[index];

        // draw our elements
        DrawElements(rect, card, deckManager.deck);
    }

    // draw our elements for the discard pile
    private void DrawDiscardPileElement(Rect rect, int index, bool active, bool focused)
    {
        // get the current card
        Card card = deckManager.discardPile[index];

        // draw our elements
        DrawElements(rect, card, deckManager.discardPile);
    }

    // draw our elements for the in use pile
    private void DrawInUsePileElement(Rect rect, int index, bool active, bool focused)
    {
        // get the current card
        Card card = deckManager.inUsePile[index];

        // draw our elements
        DrawElements(rect, card, deckManager.inUsePile);
    }

    // draw our generic menu for adding in new cards
    private void DrawGenericMenu(Rect rect, ReorderableList deck)
    { 
        // create a new menu to reference
        var menu = new GenericMenu();

        // perform two loops for our two different colors
        for (int c = 0; c < 2; c++)
        {
            // if c is an odd number
            // return red
            if (c % 2 == 1)
            {
                // perform two loops for our different suit matching out color
                for (int s = 0; s < 2; s++)
                {
                    // perform thirteen loops for our different ranks
                    for (int r = 0; r < 13; r++)
                    {
                        // create a new card
                        Card card = new Card();

                        // assign our card properties
                        card.color = Card.Color.Red;
                        card.rank = (Card.Rank)r;
                        card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;

                        // if s is an odd number
                        if (s % 2 == 1) {
                            // return hearts
                            card.suit = Card.Suit.Hearts;
                            // add our card
                            AddItem(new GUIContent("Red/Hearts/" + (Card.Rank)r), false, card, deck, menu);
                        } else {
                            // return diamonds
                            card.suit = Card.Suit.Diamonds;
                            // add our card
                            AddItem(new GUIContent("Red/Diamonds/" + (Card.Rank)r), false, card, deck, menu);
                        }
                    }
                }
            }
            else
            {
                // perform two loops for our different suit matching out color
                for (int s = 0; s < 2; s++)
                {
                    // perform thirteen loops for our different ranks
                    for (int r = 0; r < 13; r++)
                    {
                        // create a new card
                        Card card = new Card();

                        // assign our card properties
                        card.color = Card.Color.Black;
                        card.rank = (Card.Rank)r;
                        card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;

                        // if s is an odd number
                        if (s % 2 == 1) {
                            // return spades
                            card.suit = Card.Suit.Spades;
                            // add our card
                            AddItem(new GUIContent("Black/Spades/" + (Card.Rank)r), false, card, deck, menu);
                        } else {
                            // return clubs
                            card.suit = Card.Suit.Clubs;
                            // add our card
                            AddItem(new GUIContent("Black/Clubs/" + (Card.Rank)r), false, card, deck, menu);
                        }
                    }
                }
            }
        }
        // display our menu
        menu.ShowAsContext();
    }

    // create a texture background
    public static Texture2D SetBackground(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();

        return result;
    }

    // override our inspector interface
    public override void OnInspectorGUI()
    {
        // update serialized object representation
        serializedObject.Update();

        // if the instance is empty
        if (instance == null)
            // set this instance
            instance = this;

        /// GUI STYLES

        // header styles
        GUIStyle styleRowHeader = new GUIStyle
        {
            padding = new RectOffset(0, 0, 3, 3)
        };
        styleRowHeader.normal.background = SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));

        /// EDITOR

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Tools and Options", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // optn the card editor window
        if (GUILayout.Button("Open Card Editor Window"))
        {
            // get existing open window or if none, make a new one
            CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
            window.minSize = new Vector2(325, 140);
            window.Show();
        }

        // get existing open window or if none, make a new one
        if (GUILayout.Button("Open Deck Shuffle Options"))
        {
            DeckShuffleOptionsEditor window = (DeckShuffleOptionsEditor)EditorWindow.GetWindow(typeof(DeckShuffleOptionsEditor), false, "Deck Options");
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
        EditorGUILayout.LabelField("Deck / Discard Pile / In Use Pile", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // if the deck panel is expanded try and show our reorderable list
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

        // if the discard panel is expanded try and show our reorderable list
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

        // if the in use panel is expanded try and show our reorderable list
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

        // apply property modifications
        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector ();
    }
}
