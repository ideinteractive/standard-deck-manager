using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CardEditor
/// Description: Custom editor to edit our cards in our Deck Manager.
/// </summary>

public class CardEditor : EditorWindow
{
    public static CardEditor instance { get; private set; } // create a reference to this editor
    private static DeckManagerEditor deckManagerEditor; // reference our deck manager editor

    // check if this window is open or not
    public static bool IsOpen
    {
        get { return instance != null; }
    }

    public int intCardIndex;    // reference our card index
    public bool blnEditingCardFromDeck; // checks if we are editing from the deck
    public bool blnEditingCardFromDiscard;  // checks if we are editing from the discard
    public bool blnEditingCardFromInUse;    // checks if we are editing from the in use

    // on initialization
    [MenuItem("Standard Deck Manager/Card Editor")]
    private static void Init()
    {
        // get existing open window or if none, make a new one
        CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
        window.minSize = new Vector2(325, 140);
        window.Show();

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.instance;
    }

    // on enable
    private void OnEnable()
    {
        instance = this;    // set this instance
        deckManagerEditor = DeckManagerEditor.instance; // set the reference to the current inspected object
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
            EditorGUILayout.HelpBox("Changing or editing a card with the Card Editor has no impact during play mode.", MessageType.Info);

            EditorGUILayout.Space();

            // allow the user to edit the current selected card property
            if (!Application.isPlaying)
                Undo.RecordObjects(deckManagerEditor.targets, "Edit card properties.");
            deck[intCardIndex].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deck[intCardIndex].color);
            deck[intCardIndex].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deck[intCardIndex].rank);

            // set up the correct suit based on our card suit
            if (deck[intCardIndex].suit == Card.Suit.Clubs)
                deck[intCardIndex].blackSuit = Card.BlackSuit.Clubs;
            else if (deck[intCardIndex].suit == Card.Suit.Spades)
                deck[intCardIndex].blackSuit = Card.BlackSuit.Spades;
            if (deck[intCardIndex].suit == Card.Suit.Diamonds)
                deck[intCardIndex].redSuit = Card.RedSuit.Diamonds;
            else if (deck[intCardIndex].suit == Card.Suit.Hearts)
                deck[intCardIndex].redSuit = Card.RedSuit.Hearts;

            // if our color is red
            if (deck[intCardIndex].color == Card.Color.Red)
            {
                deck[intCardIndex].redSuit = (Card.RedSuit)EditorGUILayout.EnumPopup("Suit", deck[intCardIndex].redSuit);

                // assign our suit base on the selection
                if(deck[intCardIndex].redSuit == Card.RedSuit.Diamonds)
                    deck[intCardIndex].suit = Card.Suit.Diamonds;
                else
                    deck[intCardIndex].suit = Card.Suit.Hearts;
            } else
            {
                deck[intCardIndex].blackSuit = (Card.BlackSuit)EditorGUILayout.EnumPopup("Suit", deck[intCardIndex].blackSuit);

                // assign our suit base on the selection
                if (deck[intCardIndex].blackSuit == Card.BlackSuit.Clubs)
                    deck[intCardIndex].suit = Card.Suit.Clubs;
                else
                    deck[intCardIndex].suit = Card.Suit.Spades;
            }

            deck[intCardIndex].value = EditorGUILayout.IntField("Value", deck[intCardIndex].value);
            deck[intCardIndex].card = (GameObject)EditorGUILayout.ObjectField("Card", deck[intCardIndex].card, typeof(GameObject), true);
            deck[intCardIndex].blnAutoAssign = EditorGUILayout.Toggle("Auto Assign", deck[intCardIndex].blnAutoAssign);
            // if auto assign is true automatically assign an object based on selection
            if(deck[intCardIndex].blnAutoAssign)
                deck[intCardIndex].card = Resources.Load("Prefabs/" + deck[intCardIndex].color + " " + deck[intCardIndex].rank + " " + deck[intCardIndex].suit) as GameObject;

        } catch
        {
            blnEditingCardFromDeck = false;
            blnEditingCardFromDiscard = false;
            blnEditingCardFromInUse = false;
            GUIUtility.ExitGUI();
        }
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

    // draw the ui
    private void OnGUI()
    {
        // header styles
        GUIStyle styleRowHeader = new GUIStyle
        {
            padding = new RectOffset(0, 0, 3, 3)
        };
        styleRowHeader.normal.background = SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Edit Selected Card", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // inform our user no card is selected;
        if (!blnEditingCardFromDeck && !blnEditingCardFromDiscard && !blnEditingCardFromInUse)
        {
            try
            {
                EditorGUILayout.HelpBox("There is no card currently selected. Please select a card from the Deck Manager to edit.", MessageType.Info);
            }
            catch
            {
                return;
            }
        }

        // if the deck manager editor is null 
        if (deckManagerEditor == null)
        {
            // try and find a reference to the deck manager editor
            if (DeckManagerEditor.instance)
            {
                deckManagerEditor = DeckManagerEditor.instance;

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
            DrawFields(deckManagerEditor.deckManager.deck);
        else if (blnEditingCardFromDiscard)
            DrawFields(deckManagerEditor.deckManager.discardPile);
        else if (blnEditingCardFromInUse)
            DrawFields(deckManagerEditor.deckManager.inUsePile);

        EditorGUILayout.Space();
    }
}