using UnityEditor;
using UnityEngine;

/// <summary>
/// DeckShuffleOptionsEditor
/// Description: Handles all the various shuffle methods for our deck.
/// </summary>

public class DeckShuffleOptionsEditor : EditorWindow
{
    public static DeckShuffleOptionsEditor instance { get; private set; }   // create a reference to this editor
    public static DeckManagerEditor deckManagerEditor;  // reference our deck manager editor
    private Vector2 m_vecScrollPos; // create a reference to contain our scroll position

    // check if this window is open or not
    public static bool IsOpen
    {
        get { return instance != null; }
    }

    // on initialization
    [MenuItem("Standard Deck Manager/Deck Shuffle Options")]
    private static void Init()
    {
        // get existing open window or if none, make a new one
        DeckShuffleOptionsEditor window = (DeckShuffleOptionsEditor)EditorWindow.GetWindow(typeof(DeckShuffleOptionsEditor), false, "Deck Options");
        window.Show();

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.instance;
    }

    // when this is enabled and active
    private void OnEnable()
    {
        // set this instance
        instance = this;

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.instance;
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

    // repaint the inspector if it gets updated
    private void OnInspectorUpdate()
    {
        Repaint();
    }

    // draw the ui
    private void OnGUI()
    {
        EditorGUILayout.Space();

        // if the deck manager editor is empty
        if (deckManagerEditor == null)
        {
            // try and find a reference to the deck manager editor
            if (DeckManagerEditor.instance)
            {
                deckManagerEditor = DeckManagerEditor.instance;

            }
            else
            {
                // if there isn't display a warning
                EditorGUILayout.HelpBox("No instance of the Deck Manager found. Please make sure you have an object selected with the Deck Manager script attached to it.", MessageType.Info);
                EditorGUILayout.Space();
                return;
            }
        }

        // header styles
        GUIStyle styleRowHeader = new GUIStyle
        {
            padding = new RectOffset(0, 0, 3, 3)
        };
        styleRowHeader.normal.background = SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));

        m_vecScrollPos = EditorGUILayout.BeginScrollView(m_vecScrollPos);

        EditorGUILayout.BeginHorizontal(styleRowHeader);
        EditorGUILayout.LabelField("Shuffle Individual Decks", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // shuffle the deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Deck shuffled.");
            deckManagerEditor.deckManager.ShuffleDeck();
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled.");
            deckManagerEditor.deckManager.ShuffleDiscardPile();
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled.");
            deckManagerEditor.deckManager.ShuffleInUsePile();
            EditorUtility.SetDirty(deckManagerEditor.target);
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
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the deck with the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Deck shuffled with in use pile.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Discard Pile with:");

        // shuffle the discard and deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled with deck.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the discard and in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled with in use pile.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("In Use Pile with:");

        // shuffle the in use pile and deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled with deck.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the in use pile and discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled with discard pile.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
            EditorUtility.SetDirty(deckManagerEditor.target);
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
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the in use pile and add it to the top of the deck
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to top of deck.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of Discard Pile");

        // shuffle the deck and add it to the top of the discard pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to top of discard pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the in use pile and add it to the top of the discard pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to top of discard pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of In Use Pile");

        // shuffle the deck and add it to the top of the in use pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to top of in use pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the discard pile and add it to the top of the in use pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to top of in use pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
            EditorUtility.SetDirty(deckManagerEditor.target);
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
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the in use pile and add it to the bottom of the deck
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to bottom of deck.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of Discard Pile");

        // shuffle the deck and add it to the bottom of the discard pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to bottom of discard pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the in use pile and add it to the bottom of the discard pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to bottom of discard pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of In Use Pile");

        // shuffle the deck and add it to the bottom of the in use pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to bottom of in use pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        // shuffle the discard pile and add it to the bottom of the in use pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to bottom of in use pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
            EditorUtility.SetDirty(deckManagerEditor.target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
    }
}