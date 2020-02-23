using System.Collections.Generic;
using StandardDeckManager.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StandardDeckManager.Editor
{
    /// <summary>
    /// DeckManagerEditor
    /// Description: Custom editor to display all of the cards inside the Deck Manager.
    /// </summary>

    [CustomEditor(typeof(DeckManager))]
    public class DeckManagerEditor : UnityEditor.Editor
    {
        public static DeckManagerEditor Instance { get; private set; }  // create a reference to this editor
        public DeckManager deckManager; // reference our deck manager
        private ReorderableList _reorderableDeck;    // create a reorderable list for our deck
        private ReorderableList _reorderableDiscardPile; // create a reorderable list for our discard pile
        private ReorderableList _reorderableInUsePile;   // create a reorderable list for our in use pile

        // on enable
        private void OnEnable()
        {
            Instance = this;    // set this instance
            deckManager = (DeckManager)target;  // set up the reference to the current inspected object

            // create our reorderable list
            _reorderableDeck = new ReorderableList(serializedObject, serializedObject.FindProperty("deck"));
            _reorderableDiscardPile = new ReorderableList(serializedObject, serializedObject.FindProperty("discardPile"));
            _reorderableInUsePile = new ReorderableList(serializedObject, serializedObject.FindProperty("inUsePile"));

            // draw out our column headers and reorderable lists for our deck, discard pile and in use pile
            _reorderableDeck.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
            };
            _reorderableDeck.drawElementCallback += DrawDeckElement;
            _reorderableDeck.onSelectCallback += SelectCard;
            _reorderableDeck.onAddDropdownCallback = DrawGenericMenu;
            _reorderableDeck.onRemoveCallback = RemoveCard;

            _reorderableDiscardPile.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
            };
            _reorderableDiscardPile.drawElementCallback += DrawDiscardPileElement;
            _reorderableDiscardPile.onSelectCallback += SelectCard;
            _reorderableDiscardPile.onAddDropdownCallback = DrawGenericMenu;
            _reorderableDiscardPile.onRemoveCallback = RemoveCard;

            _reorderableInUsePile.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
            };
            _reorderableInUsePile.drawElementCallback += DrawInUsePileElement;
            _reorderableInUsePile.onSelectCallback += SelectCard;
            _reorderableInUsePile.onAddDropdownCallback = DrawGenericMenu;
            _reorderableInUsePile.onRemoveCallback = RemoveCard;
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
            // if the deck index is greater than 0 and is less than the deck count
            if (deck.index < 0 || deck.index >= deck.count) return;
            // check if the card editor is open
            if (!CardEditor.IsOpen) return;
            // pass our information to the card editor
            CardEditor.Instance.intCardIndex = deck.index;

            // depending on which reorderable list we are using check it off in the card editor
            CardEditor.Instance.blnEditingCardFromDeck = deck == _reorderableDeck;

            CardEditor.Instance.blnEditingCardFromDiscard = deck == _reorderableDiscardPile;

            CardEditor.Instance.blnEditingCardFromInUse = deck == _reorderableInUsePile;
        }

        // callback for when we remove a card from the deck
        private void RemoveCard(ReorderableList deck)
        {
            if (0 > deck.index || deck.index >= deck.count) return;
            serializedObject.Update();

            // determine which deck we are removing the card from
            if (deck == _reorderableDeck) {
                var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(deck.index);
                Destroy(element.FindPropertyRelative("card").objectReferenceValue);
            }
            else if (deck == _reorderableDiscardPile) {
                var element = _reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(deck.index);
                Destroy(element.FindPropertyRelative("card").objectReferenceValue);
            }
            else if (deck == _reorderableInUsePile) {
                var element = _reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(deck.index);
                Destroy(element.FindPropertyRelative("card").objectReferenceValue);
            }

            // remove our item
            ReorderableList.defaultBehaviours.DoRemoveButton(deck);

            serializedObject.ApplyModifiedProperties();
        }

        // add an item to the appropriate reorderable list
        private void AddItem(GUIContent content, Card card, ReorderableList deck, GenericMenu menu)
        {
            if (deck == _reorderableDeck) {
                menu.AddItem(content, false, () => {
                    serializedObject.Update();
                    // create a new item in our reorderable list
                    var index = _reorderableDeck.serializedProperty.arraySize;
                    _reorderableDeck.serializedProperty.arraySize++;
                    _reorderableDeck.index = index;
                    var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // spawn our card
                    SpawnCard(card, element.FindPropertyRelative("card"));
                    serializedObject.ApplyModifiedProperties(); 
                });
            }
            else if (deck == _reorderableDiscardPile) {
                menu.AddItem(content, false, () => {
                    serializedObject.Update();
                    // create a new item in our reorderable list
                    var index = _reorderableDiscardPile.serializedProperty.arraySize;
                    _reorderableDiscardPile.serializedProperty.arraySize++;
                    _reorderableDiscardPile.index = index;
                    var element = _reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // spawn our card
                    SpawnCard(card, element.FindPropertyRelative("card"));
                    serializedObject.ApplyModifiedProperties(); 
                });
            }
            else if (deck == _reorderableInUsePile) {
                menu.AddItem(content, false, () => {
                    serializedObject.Update();
                    // create a new item in our reorderable list
                    var index = _reorderableInUsePile.serializedProperty.arraySize;
                    _reorderableInUsePile.serializedProperty.arraySize++;
                    _reorderableInUsePile.index = index;
                    var element = _reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // spawn our card
                    SpawnCard(card, element.FindPropertyRelative("card"));
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
            while (_reorderableDeck.serializedProperty.arraySize < 52)
            {
                // for each suit 
                for (int i = 0; i <= (int)Card.Suit.Hearts; i++)
                {
                    // if i is an odd number
                    if (i % 2 == 1)
                    {
                        // for each card
                        for (var c = 0; c <= (int)Card.Rank.King; c++)
                        {
                            // create a new card and add it to the deck
                            // add the card to the deck
                            var card = new Card((Card.Suit) i, (Card.Color) 1, (Card.Rank) c, c + 1);
         
                            serializedObject.Update();
                            // create a new item in our reorderable list
                            var index = _reorderableDeck.serializedProperty.arraySize;
                            _reorderableDeck.serializedProperty.arraySize++;
                            _reorderableDeck.index = index;
                            var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                            // assign our properties
                            element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                            element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                            element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                            element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                            element.FindPropertyRelative("card").objectReferenceValue = card.card;

                            // spawn our card
                            SpawnCard(card, element.FindPropertyRelative("card"));
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
                            Card card = new Card((Card.Suit) i, 0, (Card.Rank) c, c + 1);

                            serializedObject.Update();
                            // create a new item in our reorderable list
                            var index = _reorderableDeck.serializedProperty.arraySize;
                            _reorderableDeck.serializedProperty.arraySize++;
                            _reorderableDeck.index = index;
                            var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                            // assign our properties
                            element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                            element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                            element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                            element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                            element.FindPropertyRelative("card").objectReferenceValue = card.card;

                            // spawn our card
                            SpawnCard(card, element.FindPropertyRelative("card"));
                            serializedObject.ApplyModifiedProperties(); 
                        }
                    }
                }
            }

            // inform the user the deck has been updated
            Debug.Log("Standard 52 Playing Card Deck - Imported");
        }

        // edit a card
        public void EditCard(int index, Card card, List<Card> deck) {
            serializedObject.Update();

            if (deck == deckManager.deck) {
                // get the current item in our deck
                _reorderableDeck.index = index;
                var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card;
                element.FindPropertyRelative("blnAutoAssign").boolValue = card.blnAutoAssign;

                // spawn our card
                SpawnCard(card, element.FindPropertyRelative("card"));
            } else if (deck == deckManager.discardPile) {
                // get the current item in our deck
                _reorderableDiscardPile.index = index;
                var element = _reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card;
                element.FindPropertyRelative("blnAutoAssign").boolValue = card.blnAutoAssign;

                // spawn our card
                SpawnCard(card, element.FindPropertyRelative("card"));
            } else if (deck == deckManager.inUsePile) {
                // get the current item in our deck
                _reorderableInUsePile.index = index;
                var element = _reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                // assign our properties
                element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                element.FindPropertyRelative("card").objectReferenceValue = card.card;
                element.FindPropertyRelative("blnAutoAssign").boolValue = card.blnAutoAssign;

                // spawn our card
                SpawnCard(card, element.FindPropertyRelative("card"));
            }

            serializedObject.ApplyModifiedProperties(); 
        }

        // spawn a card from the deck
        private void SpawnCard(Card card, SerializedProperty deckCard)
        {
            // if the application is playing
            if (!Application.isPlaying) return;
            // instantiate the object
            var tempCard = card;
            var deckTransform = deckManager.transform;
            var goCard = Instantiate(tempCard.card, deckTransform.position, deckTransform.rotation);
            goCard.SetActive(false);
            goCard.transform.parent = deckManager.goPool.transform;
            goCard.name = tempCard.color + " " + tempCard.rank + " of " + tempCard.suit;
            deckCard.objectReferenceValue = goCard;
        }

        // draw our elements
        private void DrawElements(Rect rect, Card card, IList<Card> deck)
        {
            // output the card color, rank and suit
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 2, rect.width, rect.height), card.color + " " + card.rank + " of " + card.suit);

            // if it is the main deck
            if (Equals(deck, deckManager.deck))
            {
                // if the discard card button is pressed
                if (GUI.Button(new Rect(rect.width - 70, rect.y, 30, 15), "DP"))
                {
                    serializedObject.Update();

                    // create a new item in our reorderable list
                    var index = _reorderableDiscardPile.serializedProperty.arraySize;
                    _reorderableDiscardPile.serializedProperty.arraySize++;
                    _reorderableDiscardPile.index = index;
                    var element = _reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // remove our item from the previous list
                    if (deckManager.deck != null)
                        serializedObject.FindProperty("deck").DeleteArrayElementAtIndex(deckManager.deck.IndexOf(card));

                    serializedObject.ApplyModifiedProperties(); 
                }
            
                // if the in use card button is pressed
                if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "IU"))
                {
                    serializedObject.Update();

                    // create a new item in our reorderable list
                    var index = _reorderableInUsePile.serializedProperty.arraySize;
                    _reorderableInUsePile.serializedProperty.arraySize++;
                    _reorderableInUsePile.index = index;
                    var element = _reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // remove our item from the previous list
                    if (deckManager.deck != null)
                        serializedObject.FindProperty("deck").DeleteArrayElementAtIndex(deckManager.deck.IndexOf(card));

                    serializedObject.ApplyModifiedProperties(); 
                }
            }

            // if it is the discard pile
            if (Equals(deck, deckManager.discardPile))
            {
                // if the deck button is pressed
                if (GUI.Button(new Rect(rect.width - 65, rect.y, 25, 15), "D"))
                {
                    serializedObject.Update();

                    // create a new item in our reorderable list
                    var index = _reorderableDeck.serializedProperty.arraySize;
                    _reorderableDeck.serializedProperty.arraySize++;
                    _reorderableDeck.index = index;
                    var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // remove our item from the previous list
                    serializedObject.FindProperty("discardPile").DeleteArrayElementAtIndex(deckManager.discardPile.IndexOf(card));

                    serializedObject.ApplyModifiedProperties(); 
                }

                // if the in use card button is pressed
                if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "IU"))
                {
                    serializedObject.Update();

                    // create a new item in our reorderable list
                    var index = _reorderableInUsePile.serializedProperty.arraySize;
                    _reorderableInUsePile.serializedProperty.arraySize++;
                    _reorderableInUsePile.index = index;
                    var element = _reorderableInUsePile.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // remove our item from the previous list
                    serializedObject.FindProperty("discardPile").DeleteArrayElementAtIndex(deckManager.discardPile.IndexOf(card));

                    serializedObject.ApplyModifiedProperties(); 
                }
            }

            // if it is the in use pile
            if (Equals(deck, deckManager.inUsePile))
            {
                // if the deck button is pressed
                if (GUI.Button(new Rect(rect.width - 65, rect.y, 25, 15), "D"))
                {
                    serializedObject.Update();

                    // create a new item in our reorderable list
                    var index = _reorderableDeck.serializedProperty.arraySize;
                    _reorderableDeck.serializedProperty.arraySize++;
                    _reorderableDeck.index = index;
                    var element = _reorderableDeck.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

                    // remove our item from the previous list
                    serializedObject.FindProperty("inUsePile").DeleteArrayElementAtIndex(deckManager.inUsePile.IndexOf(card));

                    serializedObject.ApplyModifiedProperties(); 
                }

                // if the discard card button is pressed
                if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "DP"))
                {
                    serializedObject.Update();

                    // create a new item in our reorderable list
                    var index = _reorderableDiscardPile.serializedProperty.arraySize;
                    _reorderableDiscardPile.serializedProperty.arraySize++;
                    _reorderableDiscardPile.index = index;
                    var element = _reorderableDiscardPile.serializedProperty.GetArrayElementAtIndex(index);

                    // assign our properties
                    element.FindPropertyRelative("name").stringValue = card.color + " " + card.rank + " of " + card.suit;
                    element.FindPropertyRelative("color").enumValueIndex = (int)card.color;
                    element.FindPropertyRelative("suit").enumValueIndex = (int)card.suit;
                    element.FindPropertyRelative("rank").enumValueIndex = (int)card.rank;
                    element.FindPropertyRelative("card").objectReferenceValue = card.card;

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
                CardEditor.Instance.intCardIndex = deck.IndexOf(card);

                // depending on which reorderable list we are using check it off in the card editor
                CardEditor.Instance.blnEditingCardFromDeck = Equals(deck, deckManager.deck);

                CardEditor.Instance.blnEditingCardFromDiscard = Equals(deck, deckManager.discardPile);

                CardEditor.Instance.blnEditingCardFromInUse = Equals(deck, deckManager.inUsePile);
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
            for (var c = 0; c < 2; c++)
            {
                // if c is an odd number
                // return red
                if (c % 2 == 1)
                {
                    // perform two loops for our different suit matching out color
                    for (var s = 0; s < 2; s++)
                    {
                        // perform thirteen loops for our different ranks
                        for (var r = 0; r < 13; r++)
                        {
                            // if s is an odd number
                            if (s % 2 == 1) {
                                // create a new card
                                var card = new Card(Card.Suit.Hearts, Card.Color.Red, (Card.Rank)r, 0);
                                // add our card
                                AddItem(new GUIContent("Red/Hearts/" + (Card.Rank)r), card, deck, menu);
                            } else {
                                // create a new card
                                var card = new Card(Card.Suit.Diamonds, Card.Color.Red, (Card.Rank)r, 0);
                                // add our card
                                AddItem(new GUIContent("Red/Diamonds/" + (Card.Rank)r), card, deck, menu);
                            }
                        }
                    }
                }
                else
                {
                    // perform two loops for our different suit matching out color
                    for (var s = 0; s < 2; s++)
                    {
                        // perform thirteen loops for our different ranks
                        for (var r = 0; r < 13; r++)
                        {
                            // if s is an odd number
                            if (s % 2 == 1) {
                                // create a new card
                                var card = new Card(Card.Suit.Spades, Card.Color.Black, (Card.Rank)r, 0);
                                // add our card
                                AddItem(new GUIContent("Black/Spades/" + (Card.Rank)r), card, deck, menu);
                            } else {
                                // create a new card
                                var card = new Card(Card.Suit.Clubs, Card.Color.Black, (Card.Rank)r, 0);
                                // add our card
                                AddItem(new GUIContent("Black/Clubs/" + (Card.Rank)r), card, deck, menu);
                            }
                        }
                    }
                }
            }
            // display our menu
            menu.ShowAsContext();
        }

        // create a texture background
        private static Texture2D SetBackground(int width, int height, Color color)
        {
            var pixels = new Color[width * height];

            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            var result = new Texture2D(width, height);
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
            if (Instance == null)
                // set this instance
                Instance = this;

            // GUI STYLES

            // header styles
            var styleRowHeader = new GUIStyle
            {
                padding = new RectOffset(0, 0, 3, 3),
                normal = {background = SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f))}
            };

            // EDITOR

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(styleRowHeader);
            EditorGUILayout.LabelField("Tools and Options", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // open the card editor window
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
                var window = (DeckShuffleOptionsEditor)EditorWindow.GetWindow(typeof(DeckShuffleOptionsEditor), false, "Deck Options");
                window.Show();
            }

            // remove everything and create a new standard deck
            if (GUILayout.Button("Generate a New Deck"))
            {
                Undo.RecordObjects(targets, "New deck generated.");
                RemoveAllAndCreateNew();
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
                    _reorderableDeck.DoLayoutList();
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
                    _reorderableDiscardPile.DoLayoutList();
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
                    _reorderableInUsePile.DoLayoutList();
                }
                catch
                {
                    return;
                }

            EditorGUILayout.Space();

            // apply property modifications
            serializedObject.ApplyModifiedProperties();
        }
    }
}
