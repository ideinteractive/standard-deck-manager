using System.Collections.Generic;
using StandardDeckManager.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

namespace StandardDeckManager.Editor
{
    /// <summary>
    /// DeckManagerEditor
    /// Description: Custom editor to display all of the cards inside the Deck Manager.
    /// </summary>

    [CustomEditor(typeof(DeckManager))]
    public class DeckManagerEditor : UnityEditor.Editor
    {
        // references to our deck manager and editor
        public static DeckManagerEditor instance;

        public DeckManager deckManager;
        
        // reorderable lists for each deck
        private ReorderableList _reorderableDeck;
        private ReorderableList _reorderableDiscardPile;
        private ReorderableList _reorderableInUsePile;

        // on enable
        private void OnEnable()
        {
            // set our target and editor instance
            instance = this;
            deckManager = (DeckManager) target;
            
            // set up our reorderable lists
            _reorderableDeck = new ReorderableList(deckManager.deck, typeof(Card), true, true, true, true);
            _reorderableDeck.drawElementCallback += DrawDeckElement;
            _reorderableDeck.onSelectCallback += SelectCard;
            _reorderableDeck.onAddDropdownCallback = DrawGenericMenu;
            _reorderableDeck.onRemoveCallback = RemoveCard;
            _reorderableDeck.drawHeaderCallback += DrawHeader;
            
            _reorderableDiscardPile = new ReorderableList(deckManager.discardPile, typeof(Card), true, true, true, true);
            _reorderableDiscardPile.drawElementCallback += DrawDiscardPileElement;
            _reorderableDiscardPile.onSelectCallback += SelectCard;
            _reorderableDiscardPile.onAddDropdownCallback = DrawGenericMenu;
            _reorderableDiscardPile.onRemoveCallback = RemoveCard;
            _reorderableDiscardPile.drawHeaderCallback += DrawHeader;
            
            _reorderableInUsePile = new ReorderableList(deckManager.inUsePile, typeof(Card), true, true, true, true);
            _reorderableInUsePile.drawElementCallback += DrawInUsePileElement;
            _reorderableInUsePile.onSelectCallback += SelectCard;
            _reorderableInUsePile.onAddDropdownCallback = DrawGenericMenu;
            _reorderableInUsePile.onRemoveCallback = RemoveCard;
            _reorderableInUsePile.drawHeaderCallback += DrawHeader;
        }

        // on destroy
        private void OnDestroy()
        {
            // remove the reference to this editor
            instance = null;
        }

        private static void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "Cards", EditorStyles.boldLabel);
        }
        
        // spawn a card from the deck
        private void SpawnCard(Card card)
        {
            // if the application is not playing
            if (!Application.isPlaying) return;
 
            // instantiate the object
            var tempCard = card;
            var deckTransform = deckManager.transform;
            var goCard = Instantiate(tempCard.card, deckTransform.position, deckTransform.rotation);
            goCard.SetActive(false);
            goCard.transform.parent = deckManager.goPool.transform;
            goCard.name = tempCard.color + " " + tempCard.rank + " of " + tempCard.suit;
            card.card = goCard;
        }
        
        // add an item to the appropriate reorderable list
        private void AddItem(GUIContent content, Card card, ReorderableList deck, GenericMenu menu)
        {
            if (deck == _reorderableDeck) {
                menu.AddItem(content, false, () => {
                    // create and spawn our card
                    Undo.RecordObject(target, "Added new card to deck.");
                    deckManager.AddCard(card, deckManager.deck);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                    SpawnCard(card);
                });
            }
            else if (deck == _reorderableDiscardPile) {
                menu.AddItem(content, false, () => {
                    // create and spawn our card
                    Undo.RecordObject(target, "Added new card to discard pile.");
                    deckManager.AddCard(card, deckManager.discardPile);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                    SpawnCard(card);
                });
            }
            else if (deck == _reorderableInUsePile) {
                menu.AddItem(content, false, () => {
                    // create and spawn our card
                    Undo.RecordObject(target, "Added new card to in use pile.");
                    deckManager.AddCard(card, deckManager.inUsePile);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                    SpawnCard(card);
                });
            }
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
            // if there deck index is invalid return
            if (0 > deck.index || deck.index >= deck.count) return;
            
            if (deck == _reorderableDeck) {
                Undo.RecordObject(target, "Removed a card from deck.");
                deckManager.deck.RemoveAt(deck.index);
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
            else if (deck == _reorderableDiscardPile) {
                Undo.RecordObject(target, "Removed a card from discard pile.");
                deckManager.discardPile.RemoveAt(deck.index);
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
            else if (deck == _reorderableInUsePile) {
                Undo.RecordObject(target, "Removed a card from in use pile.");
                deckManager.inUsePile.RemoveAt(deck.index);
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
        }

        // generate a new deck
        public void RemoveAllAndCreateNew()
        {
            // remove all cards and generate new deck
            Undo.RecordObject(target, "Generated a new deck.");

            // make sure everything is cleared before proceeding
            deckManager.RemoveAll();
            
            // while the deck count is under 52
            while (deckManager.deck.Count < 52)
            {
                // for each suit 
                for (var i = 0; i <= (int)Card.Suit.Hearts; i++)
                {
                    // if i is an odd number
                    if (i % 2 == 1)
                    {
                        // for each card
                        for (var c = 0; c <= (int)Card.Rank.King; c++)
                        {
                            // create and spawn our card
                            var card = new Card((Card.Suit) i, (Card.Color) 1, (Card.Rank) c, c + 1);
                            Undo.RecordObject(target, "Added new card to discard pile.");
                            deckManager.AddCard(card, deckManager.deck);
                            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                            SpawnCard(card);
                        }
                    }
                    else
                    {
                        // for each card
                        for (var c = 0; c <= (int)Card.Rank.King; c++)
                        {
                            // create and spawn our card
                            var card = new Card((Card.Suit) i, 0, (Card.Rank) c, c + 1);
                            Undo.RecordObject(target, "Added new card to discard pile.");
                            deckManager.AddCard(card, deckManager.deck);
                            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                            SpawnCard(card);
                        }
                    }
                }
            }
            
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);

            // inform the user the deck has been updated
            Debug.Log("Standard 52 Playing Card Deck - Imported");
        }

         // remove all cards
        public void RemoveAll()
        {    
            // remove all cards
            Undo.RecordObject(target, "All cards removed.");
            deckManager.RemoveAll();
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }

        // edit a card
        public void EditCard(int index, Card card, List<Card> deck)
        {
            var tmpCard = new Card();
            
            if (deck == deckManager.deck) {
                // get our card
                tmpCard = deckManager.deck[index];
                
            } else if (deck == deckManager.discardPile) {
                // get our card
                tmpCard = deckManager.discardPile[index];
            } else if (deck == deckManager.inUsePile) {
                // get our card
                tmpCard = deckManager.inUsePile[index];
            }
            
            // assign and spawn our card
            Undo.RecordObject(target, "Edit Card");
            tmpCard.name = card.color + " " + card.rank + " of " + card.suit;
            tmpCard.color = card.color;
            tmpCard.suit = card.suit;
            tmpCard.rank = card.rank;
            tmpCard.card = card.card;
            tmpCard.blnAutoAssign = card.blnAutoAssign;
            
            // if the new card isn't the same as the current card
            if (tmpCard.card != card.card)
                SpawnCard(card);
            
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
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
                    // add the card to the discard pile and remove from deck
                    Undo.RecordObject(target, "Moved Card to Discard Pile");
                    deckManager.discardPile.Add((card));
                    deckManager.deck?.RemoveAt(deckManager.deck.IndexOf(card));
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            
                // if the in use card button is pressed
                if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "IU"))
                {
                    // add the card to the in use [o;e and remove from deck
                    Undo.RecordObject(target, "Moved Card to In Use Pile");
                    deckManager.inUsePile.Add((card));
                    deckManager.deck?.RemoveAt(deckManager.deck.IndexOf(card));
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }

            // if it is the discard pile
            if (Equals(deck, deckManager.discardPile))
            {
                // if the deck button is pressed
                if (GUI.Button(new Rect(rect.width - 65, rect.y, 25, 15), "D"))
                {
                    // add the card to the deck and remove from discard pile
                    Undo.RecordObject(target, "Moved Card to Deck");
                    deckManager.deck?.Add((card));
                    deckManager.discardPile?.RemoveAt(deckManager.discardPile.IndexOf(card));
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }

                // if the in use card button is pressed
                if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "IU"))
                {
                    // add the card to the in use pile and remove from discard pile
                    Undo.RecordObject(target, "Moved Card to In Use Pile");
                    deckManager.inUsePile?.Add((card));
                    deckManager.discardPile?.RemoveAt(deckManager.discardPile.IndexOf(card));
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }

            // if it is the in use pile
            if (Equals(deck, deckManager.inUsePile))
            {
                // if the deck button is pressed
                if (GUI.Button(new Rect(rect.width - 65, rect.y, 25, 15), "D"))
                {
                    // add the card to the deck and remove from in use pile
                    Undo.RecordObject(target, "Moved Card to Deck");
                    deckManager.deck?.Add((card));
                    deckManager.inUsePile?.RemoveAt(deckManager.inUsePile.IndexOf(card));
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }

                // if the discard card button is pressed
                if (GUI.Button(new Rect(rect.width - 35, rect.y, 30, 15), "DP"))
                {
                    // add the card to the discard pile and remove from in use pile
                    Undo.RecordObject(target, "Moved Card to Deck");
                    deckManager.discardPile?.Add((card));
                    deckManager.inUsePile?.RemoveAt(deckManager.inUsePile.IndexOf(card));
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }

            // if the edit card button is pressed
            if (!GUI.Button(new Rect(rect.width, rect.y, 25, 15), "E")) return;
            
            // edit the card with the card editor
            // get existing open window or if none, make a new one
            var window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
            window.minSize = new Vector2(325, 140);
            window.Show();

            // pass our information to the card editor
            CardEditor.Instance.intCardIndex = deck.IndexOf(card);

            // depending on which reorderable list we are using check it off in the card editor
            CardEditor.Instance.blnEditingCardFromDeck = Equals(deck, deckManager.deck);

            CardEditor.Instance.blnEditingCardFromDiscard = Equals(deck, deckManager.discardPile);

            CardEditor.Instance.blnEditingCardFromInUse = Equals(deck, deckManager.inUsePile);
        }

        // draw our elements for the deck
        private void DrawDeckElement(Rect rect, int index, bool active, bool focused)
        {
            // get the current card
            var card = deckManager.deck[index];

            // draw our elements
            DrawElements(rect, card, deckManager.deck);
        }

        // draw our elements for the discard pile
        private void DrawDiscardPileElement(Rect rect, int index, bool active, bool focused)
        {
            // get the current card
            var card = deckManager.discardPile[index];

            // draw our elements
            DrawElements(rect, card, deckManager.discardPile);
        }

        // draw our elements for the in use pile
        private void DrawInUsePileElement(Rect rect, int index, bool active, bool focused)
        {
            // get the current card
            var card = deckManager.inUsePile[index];

            // draw our elements
            DrawElements(rect, card, deckManager.inUsePile);
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
            if (instance == null)
                // set this instance
                instance = this;

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
                var window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
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
                RemoveAllAndCreateNew();
            }

            // remove everything
            if (GUILayout.Button("Remove All Cards"))
            {
                RemoveAll();
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
