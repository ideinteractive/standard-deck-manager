using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// DeckOptionsEditor
/// Description: Handles all the various shuffle methods for our deck.
/// </summary>

public class DeckOptionsEditor : EditorWindow
{
    // reference this editor
    public static DeckOptionsEditor Instance { get; private set; }

    // reference to the DeckManager
    public static DeckManagerEditor deckManagerEditor;

    // create an empty card for reference
    private Vector2 m_vecScrollPos;

    // check if this window is open or not
    public static bool IsOpen
    {
        get { return Instance != null; }
    }

    // on initialization
    private static void Init()
    {
        // get existing open window or if none, make a new one
        DeckOptionsEditor window = (DeckOptionsEditor)EditorWindow.GetWindow(typeof(DeckOptionsEditor), false, "Deck Options");
        window.Show();

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.Instance;
    }

    // when this is enabled and active
    private void OnEnable()
    {
        // set this instance
        Instance = this;

        // set the reference to the current inspected object
        deckManagerEditor = DeckManagerEditor.Instance;
    }

    // repaint the inspector if it gets updated
    private void OnInspectorUpdate()
    {
        Repaint();
    }

    // mark the scene as dirty
    private void MarkSceneDirty()
    {
        // if we are not in play mode
        if(!Application.isPlaying)
            // mark the scene as dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    // draw the ui
    private void OnGUI()
    {
        EditorGUILayout.Space();

        // if the deck manager editor is empty
        if (deckManagerEditor == null)
        {
            // try and find a reference to the deck manager editor
            if (DeckManagerEditor.Instance)
            {
                deckManagerEditor = DeckManagerEditor.Instance;

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
        GUIStyle styleRowHeader = new GUIStyle();
        styleRowHeader.padding = new RectOffset(0, 0, 3, 3);
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        GUIStyle stylePaddingLeft = new GUIStyle();
        stylePaddingLeft.padding = new RectOffset(10, 0, 3, 3);

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
            MarkSceneDirty();
        }

        // shuffle the discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled.");
            deckManagerEditor.deckManager.ShuffleDiscardPile();
            MarkSceneDirty();
        }

        // shuffle the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled.");
            deckManagerEditor.deckManager.ShuffleInUsePile();
            MarkSceneDirty();
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
            MarkSceneDirty();
        }

        // shuffle the deck with the in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Deck shuffled with in use pile.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Discard Pile with:");

        // shuffle the discard and deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled with deck.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.deck);
            MarkSceneDirty();
        }

        // shuffle the discard and in use pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Discard pile shuffled with in use pile.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("In Use Pile with:");

        // shuffle the in use pile and deck
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled with deck.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
            MarkSceneDirty();
        }

        // shuffle the in use pile and discard pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "In use pile shuffled with discard pile.");
            deckManagerEditor.deckManager.ShuffleDecksTogether(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
            MarkSceneDirty();
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
            MarkSceneDirty();
        }

        // shuffle the in use pile and add it to the top of the deck
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to top of deck.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of Discard Pile");

        // shuffle the deck and add it to the top of the discard pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to top of discard pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
            MarkSceneDirty();
        }

        // shuffle the in use pile and add it to the top of the discard pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to top of discard pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Top of In Use Pile");

        // shuffle the deck and add it to the top of the in use pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to top of in use pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
            MarkSceneDirty();
        }

        // shuffle the discard pile and add it to the top of the in use pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to top of in use pile.");
            deckManagerEditor.deckManager.ShuffleToTopOfDeck(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
            MarkSceneDirty();
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
            MarkSceneDirty();
        }

        // shuffle the in use pile and add it to the bottom of the deck
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to bottom of deck.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.deck);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of Discard Pile");

        // shuffle the deck and add it to the bottom of the discard pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to bottom of discard pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.discardPile);
            MarkSceneDirty();
        }

        // shuffle the in use pile and add it to the bottom of the discard pile
        if (GUILayout.Button("In Use Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled in use pile to bottom of discard pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.inUsePile, deckManagerEditor.deckManager.discardPile);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("To Bottom of In Use Pile");

        // shuffle the deck and add it to the bottom of the in use pile
        if (GUILayout.Button("Deck"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled deck to bottom of in use pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.deck, deckManagerEditor.deckManager.inUsePile);
            MarkSceneDirty();
        }

        // shuffle the discard pile and add it to the bottom of the in use pile
        if (GUILayout.Button("Discard Pile"))
        {
            Undo.RecordObjects(deckManagerEditor.targets, "Shuffled discard pile to bottom of in use pile.");
            deckManagerEditor.deckManager.ShuffleToBottomOfDeck(deckManagerEditor.deckManager.discardPile, deckManagerEditor.deckManager.inUsePile);
            MarkSceneDirty();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
    }
}