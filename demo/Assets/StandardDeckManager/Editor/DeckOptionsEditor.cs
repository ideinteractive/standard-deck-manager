using UnityEditor;
using UnityEngine;

/// <summary>
/// SDMTools
/// Description: Editor tools to allow for easy debugging of the standard deck manager.
/// </summary>

public class DeckOptionsEditor : EditorWindow
{/*
    // reference this editor
    public static SDMTools Instance { get; private set; }

    // reference to the DeckManager
    public static DeckManagerEditor deckManagerEditor;
    public static DeckManager deckManager;

    // create an empty card for reference
    private Card card = new Card();
    private bool blnAutoAssignCardObject;
    private Vector2 m_vecScrollPos;

    // on initialization
    private static void Init()
    {
        // get existing open window or if none, make a new one
        SDMTools window = (SDMTools)EditorWindow.GetWindow(typeof(SDMTools), false, "SDM Tools");
        window.Show();

        // set the reference to the current inspected objects
        //deckManager = GameObject.FindObjectOfType<DeckManager>();
        deckManagerEditor = DeckManagerEditor.Instance;
        deckManager = GameObject.FindObjectOfType<DeckManager>();
    }

    // on enable
    private void OnEnable()
    {
        // set this instance
        Instance = this;

        // set the reference to the current inspected objects
        //deckManager = GameObject.FindObjectOfType<DeckManager>();
        deckManagerEditor = DeckManagerEditor.Instance;
        deckManager = GameObject.FindObjectOfType<DeckManager>();
    }

    // when the window gets updated
    private void OnInspectorUpdate()
    {
        // repaint the editor window
        Repaint();
    }

    // generate a random card
    private Card RandomCard()
    {
        Card card = new Card();
        card.color = (Card.Color)Random.Range(0, (int)Card.Color.Red + 1);
        card.rank = (Card.Rank)Random.Range(0, (int)Card.Rank.King + 1);
        card.suit = (Card.Suit)Random.Range(0, (int)Card.Suit.Spades + 1);
        card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
        card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
        return card;
    }

    // draw the ui
    void OnGUI()
    {
        /// EDITOR

        EditorGUILayout.Space();
        if(deckManager == null)
        {
            deckManager = GameObject.FindObjectOfType<DeckManager>();
        }
        // if the deck manager editor is empty
        if (deckManagerEditor == null)
        {
            // if there is an instance of it
            if (DeckManagerEditor.Instance)
            {
                // set it as our reference
                deckManagerEditor = DeckManagerEditor.Instance;
            }
            else
            {
                // if there isn't, display an error
                EditorGUILayout.HelpBox("No instance of the Deck Manager found. Please make sure you have an object selected with the Deck Manager script attached to it.", MessageType.Info);
                EditorGUILayout.Space();
               // return;
            }
        }

        // header styles
        GUIStyle styleRowHeader = new GUIStyle();
        styleRowHeader.padding = new RectOffset(0, 0, 3, 3);
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        GUIStyle stylePaddingLeft = new GUIStyle();
        stylePaddingLeft.padding = new RectOffset(10, 0, 3, 3);

        m_vecScrollPos = EditorGUILayout.BeginScrollView(m_vecScrollPos);

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Add a New Card", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(stylePaddingLeft);

        // allows use to create a new card to add
        card.color = (Card.Color)EditorGUILayout.EnumPopup("Color", card.color);
        card.rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", card.rank);
        card.suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", card.suit);
        card.value = EditorGUILayout.IntField("Value", card.value);
        card.card = (GameObject)EditorGUILayout.ObjectField("Card", card.card, typeof(GameObject), true);
        blnAutoAssignCardObject = EditorGUILayout.Toggle("Auto Assign Object", blnAutoAssignCardObject);

        // if auto assigning object is true
        if (blnAutoAssignCardObject)
        {
            // auto assign an object to the card object slot if available
            if (Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject)
            {
                card.card = Resources.Load("Prefabs/" + card.color + " " + card.rank + " " + card.suit) as GameObject;
            }
            else
            {
                EditorGUILayout.Space();

                // inform the user there is no object available
                EditorGUILayout.HelpBox("The current card has no object available to be assigned in the Resources folder.", MessageType.Info);
                card.card = null;
            }
        }

        EditorGUILayout.Space();

        // add a new card to the deck
        if (GUILayout.Button("Add to Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Added card to deck.");
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckManager.AddCard(card, deckManagerEditor.deckManager.deck);
            card = new Card();
        }

        // add a new card to the discard pile
        if (GUILayout.Button("Add to Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Added card to discard pile.");
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckManagerEditor.deckManager.AddCard(card, deckManagerEditor.deckManager.discardPile);
            card = new Card();
        }

        // add a new card to the in use pile
        if (GUILayout.Button("Add to In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Added card to in use pile.");
            card.strName = card.color.ToString() + " " + card.rank.ToString() + " of " + card.suit.ToString();
            deckManagerEditor.deckManager.AddCard(card, deckManagerEditor.deckManager.inUsePile);
            card = new Card();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Add a Randon Card", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // inform the user there is no object available
        EditorGUILayout.HelpBox("Please keep in mind random cards have a value of 0. Values will have to be assigned manually.", MessageType.Info);
        EditorGUILayout.Space();
        Undo.RecordObjects(deckManagerEditor.targets, "Added card to deck.");
        // add a random card to the deck
        if (GUILayout.Button("Add to Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Added card to deck.");
            deckManagerEditor.deckManager.AddCard(RandomCard(), deckManagerEditor.deckManager.deck);
        }

        // add a random card to the discard pile
        if (GUILayout.Button("Add to Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Added card to discard pile.");
            deckManagerEditor.deckManager.AddCard(RandomCard(), deckManagerEditor.deckManager.discardPile);
        }

        // add a random card to the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Added card to in use pile.");
            deckManagerEditor.deckManager.AddCard(RandomCard(), deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Shuffle Individual Decks", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // shuffle the deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Deck shuffled.");
            deckManagerEditor.deckManager.Shuffle();
        }

        // shuffle the discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled.");
            deckManagerEditor.deckManager.ShuffleDiscardPile();
        }

        // shuffle the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled.");
            deckManagerEditor.deckManager.ShuffleInUsePile();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Shuffle Decks Together", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Deck with:");

        // shuffle the deck with the discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Deck shuffled with discard pile.");
            deckManagerEditor.deckManager.ShuffleDecks(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
        }

        // shuffle the deck with the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Deck shuffled with in use pile.");
            deckManagerEditor.deckManager.ShuffleDecks(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Discard Pile with:");

        // shuffle the discard and deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled with deck.");
            deckManagerEditor.deckManager.ShuffleDecks(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
        }

        // shuffle the discard and in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled with in use pile.");
            deckManagerEditor.deckManager.ShuffleDecks(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("In Use Pile with:");

        // shuffle the in use pile and deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled with deck.");
            deckManagerEditor.deckManager.ShuffleDecks(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
        }

        // shuffle the in use pile and discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled with discard pile.");
            deckManagerEditor.deckManager.ShuffleDecks(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Shuffle To Top Deck", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of Deck");

        // shuffle the discard pile and add it to the top of the deck
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to top of deck.");
            deckManagerEditor.deckManager.ShuffleToTop(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
        }
        // shuffle the in use pile and add it to the top of the deck
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to top of deck.");
            deckManagerEditor.deckManager.ShuffleToTop(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of Discard Pile");

        // shuffle the deck and add it to the top of the discard pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to top of discard pile.");
            deckManagerEditor.deckManager.ShuffleToTop(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
        }

        // shuffle the in use pile and add it to the top of the discard pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to top of discard pile.");
            deckManagerEditor.deckManager.ShuffleToTop(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of In Use Pile");

        // shuffle the deck and add it to the top of the in use pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to top of in use pile.");
            deckManagerEditor.deckManager.ShuffleToTop(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
        }

        // shuffle the discard pile and add it to the top of the in use pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to top of in use pile.");
            deckManagerEditor.deckManager.ShuffleToTop(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Shuffle To Bottom of Deck", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of Deck");

        // shuffle the discard pile and add it to the bottom of the deck
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to bottom of deck.");
            deckManagerEditor.deckManager.ShuffleToBottom(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
        }

        // shuffle the in use pile and add it to the bottom of the deck
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to bottom of deck.");
            deckManagerEditor.deckManager.ShuffleToBottom(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of Discard Pile");

        // shuffle the deck and add it to the bottom of the discard pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to bottom of discard pile.");
            deckManagerEditor.deckManager.ShuffleToBottom(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
        }

        // shuffle the in use pile and add it to the bottom of the discard pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to bottom of discard pile.");
            deckManagerEditor.deckManager.ShuffleToBottom(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of In Use Pile");

        // shuffle the deck and add it to the bottom of the in use pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to bottom of in use pile.");
            deckManagerEditor.deckManager.ShuffleToBottom(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
        }

        // shuffle the discard pile and add it to the bottom of the in use pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to bottom of in use pile.");
            deckManagerEditor.deckManager.ShuffleToBottom(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
    }*/
}