using System;
using System.Collections.Generic;
using StandardDeckManager.Scripts;
using UnityEditor;
using UnityEngine;

namespace StandardDeckManager.Editor
{
    /// <summary>
    /// CardEditor
    /// Description: Custom editor to edit our cards in our Deck Manager.
    /// </summary>

    public class CardEditor : EditorWindow
    {
        public static CardEditor Instance { get; private set; } // create a reference to this editor
        private static DeckManagerEditor _deckManagerEditor; // reference our deck manager editor

        // check if this window is open or not
        public static bool IsOpen => Instance != null;

        public int intCardIndex; // reference our card index
        public bool blnEditingCardFromDeck; // checks if we are editing from the deck
        public bool blnEditingCardFromDiscard; // checks if we are editing from the discard
        public bool blnEditingCardFromInUse; // checks if we are editing from the in use

        // on initialization
        [MenuItem("Standard Deck Manager/Card Editor")]
        private static void Init()
        {
            // get existing open window or if none, make a new one
            var window = (CardEditor) EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
            window.minSize = new Vector2(325, 140);
            window.Show();

            // set the reference to the current inspected object
            _deckManagerEditor = DeckManagerEditor.instance;
        }

        // on enable
        private void OnEnable()
        {
            Instance = this; // set this instance
            _deckManagerEditor = DeckManagerEditor.instance; // set the reference to the current inspected object
        }

        // repaint the inspector if it gets updated
        private void OnInspectorUpdate()
        {
            Repaint();
        }

        // draw our fields onto the inspector
        private void DrawFields(List<Card> deck)
        {
            // try to draw our fields and if we can't turn them off
            try
            {
                EditorGUILayout.HelpBox(
                    "Changing or editing a card with the Card Editor has no impact during play mode.",
                    MessageType.Info);

                EditorGUILayout.Space();

                // assign the color and rank
                deck[intCardIndex].color = (Card.Color) EditorGUILayout.EnumPopup("Color", deck[intCardIndex].color);
                deck[intCardIndex].rank = (Card.Rank) EditorGUILayout.EnumPopup("Rank", deck[intCardIndex].rank);

                switch (deck[intCardIndex].suit)
                {
                    // set up the correct suit based on our card suit
                    case Card.Suit.Clubs:
                        deck[intCardIndex].blackSuit = Card.BlackSuit.Clubs;
                        break;
                    case Card.Suit.Spades:
                        deck[intCardIndex].blackSuit = Card.BlackSuit.Spades;
                        break;
                    case Card.Suit.Diamonds:
                        deck[intCardIndex].redSuit = Card.RedSuit.Diamonds;
                        break;
                    case Card.Suit.Hearts:
                        deck[intCardIndex].redSuit = Card.RedSuit.Hearts;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // if our color is red
                if (deck[intCardIndex].color == Card.Color.Red)
                {
                    deck[intCardIndex].redSuit =
                        (Card.RedSuit) EditorGUILayout.EnumPopup("Suit", deck[intCardIndex].redSuit);

                    // assign our suit base on the selection
                    deck[intCardIndex].suit = deck[intCardIndex].redSuit == Card.RedSuit.Diamonds
                        ? Card.Suit.Diamonds
                        : Card.Suit.Hearts;
                }
                else
                {
                    deck[intCardIndex].blackSuit =
                        (Card.BlackSuit) EditorGUILayout.EnumPopup("Suit", deck[intCardIndex].blackSuit);

                    // assign our suit base on the selection
                    deck[intCardIndex].suit = deck[intCardIndex].blackSuit == Card.BlackSuit.Clubs
                        ? Card.Suit.Clubs
                        : Card.Suit.Spades;
                }

                deck[intCardIndex].value = EditorGUILayout.IntField("Value", deck[intCardIndex].value);
                deck[intCardIndex].card =
                    (GameObject) EditorGUILayout.ObjectField("Card", deck[intCardIndex].card, typeof(GameObject), true);
                deck[intCardIndex].blnAutoAssign =
                    EditorGUILayout.Toggle("Auto Assign", deck[intCardIndex].blnAutoAssign);

                // if auto assign is true automatically assign an object based on selection
                if (deck[intCardIndex].blnAutoAssign)
                    deck[intCardIndex].card =
                        Resources.Load("Prefabs/" + deck[intCardIndex].color + " " + deck[intCardIndex].rank + " " +
                                       deck[intCardIndex].suit) as GameObject;

                DeckManagerEditor.instance.EditCard(intCardIndex, deck[intCardIndex], deck);
            }
            catch
            {
                blnEditingCardFromDeck = false;
                blnEditingCardFromDiscard = false;
                blnEditingCardFromInUse = false;
                GUIUtility.ExitGUI();
            }
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

        // draw the ui
        private void OnGUI()
        {
            // header styles
            var styleRowHeader = new GUIStyle
            {
                padding = new RectOffset(0, 0, 3, 3),
                normal = {background = SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f))}
            };

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(styleRowHeader);
            EditorGUILayout.LabelField("Edit Selected Card", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            // inform our user no card is selected;
            if (!blnEditingCardFromDeck && !blnEditingCardFromDiscard && !blnEditingCardFromInUse)
            {
                try
                {
                    EditorGUILayout.HelpBox(
                        "There is no card currently selected. Please select a card from the Deck Manager to edit.",
                        MessageType.Info);
                }
                catch
                {
                    return;
                }
            }

            // if the deck manager editor is null 
            if (_deckManagerEditor == null)
            {
                // try and find a reference to the deck manager editor
                if (DeckManagerEditor.instance)
                {
                    _deckManagerEditor = DeckManagerEditor.instance;

                }
                else
                {
                    blnEditingCardFromDeck = false;
                    blnEditingCardFromDiscard = false;
                    blnEditingCardFromInUse = false;
                    return;
                }
            }

            // if we are editing the card
            if (blnEditingCardFromDeck)
                DrawFields(_deckManagerEditor.deckManager.deck);
            else if (blnEditingCardFromDiscard)
                DrawFields(_deckManagerEditor.deckManager.discardPile);
            else if (blnEditingCardFromInUse)
                DrawFields(_deckManagerEditor.deckManager.inUsePile);

            EditorGUILayout.Space();
        }
    }
}