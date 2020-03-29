using UnityEditor;
using UnityEngine;

namespace StandardDeckManager.Editor
{
    /// <summary>
    /// DeckShuffleOptionsEditor
    /// Description: Handles all the various shuffle methods for our deck.
    /// </summary>

    public class DeckShuffleOptionsEditor : EditorWindow
    {
        private static DeckManagerEditor _deckManagerEditor;  // reference our deck manager editor
        private Vector2 _mVecScrollPos; // create a reference to contain our scroll position

        // check if this window is open or not

        // on initialization
        [MenuItem("Standard Deck Manager/Deck Shuffle Options")]
        private static void Init()
        {
            // get existing open window or if none, make a new one
            DeckShuffleOptionsEditor window = (DeckShuffleOptionsEditor)EditorWindow.GetWindow(typeof(DeckShuffleOptionsEditor), false, "Deck Options");
            window.Show();

            // set the reference to the current inspected object
            _deckManagerEditor = DeckManagerEditor.instance;
        }

        // when this is enabled and active
        private void OnEnable()
        {
            // set this instance

            // set the reference to the current inspected object
            _deckManagerEditor = DeckManagerEditor.instance;
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
            if (_deckManagerEditor == null)
            {
                // try and find a reference to the deck manager editor
                if (DeckManagerEditor.instance)
                {
                    _deckManagerEditor = DeckManagerEditor.instance;

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
            var styleRowHeader = new GUIStyle
            {
                padding = new RectOffset(0, 0, 3, 3),
                normal = {background = SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f))}
            };

            _mVecScrollPos = EditorGUILayout.BeginScrollView(_mVecScrollPos);

            EditorGUILayout.BeginHorizontal(styleRowHeader);
            EditorGUILayout.LabelField("Shuffle Individual Decks", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // shuffle the deck
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Deck shuffled.");
                _deckManagerEditor.deckManager.ShuffleDeck();
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the discard pile
            if (GUILayout.Button("Discard Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Discard pile shuffled.");
                _deckManagerEditor.deckManager.ShuffleDiscardPile();
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the in use pile
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "In use pile shuffled.");
                _deckManagerEditor.deckManager.ShuffleInUsePile();
                EditorUtility.SetDirty(_deckManagerEditor.target);
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
                Undo.RecordObjects(_deckManagerEditor.targets, "Deck shuffled with discard pile.");
                _deckManagerEditor.deckManager.ShuffleDecksTogether(_deckManagerEditor.deckManager.deck, _deckManagerEditor.deckManager.discardPile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the deck with the in use pile
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Deck shuffled with in use pile.");
                _deckManagerEditor.deckManager.ShuffleDecksTogether(_deckManagerEditor.deckManager.deck, _deckManagerEditor.deckManager.inUsePile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Discard Pile with:");

            // shuffle the discard and deck
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Discard pile shuffled with deck.");
                _deckManagerEditor.deckManager.ShuffleDecksTogether(_deckManagerEditor.deckManager.discardPile, _deckManagerEditor.deckManager.deck);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the discard and in use pile
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Discard pile shuffled with in use pile.");
                _deckManagerEditor.deckManager.ShuffleDecksTogether(_deckManagerEditor.deckManager.discardPile, _deckManagerEditor.deckManager.inUsePile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("In Use Pile with:");

            // shuffle the in use pile and deck
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "In use pile shuffled with deck.");
                _deckManagerEditor.deckManager.ShuffleDecksTogether(_deckManagerEditor.deckManager.inUsePile, _deckManagerEditor.deckManager.deck);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the in use pile and discard pile
            if (GUILayout.Button("Discard Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "In use pile shuffled with discard pile.");
                _deckManagerEditor.deckManager.ShuffleDecksTogether(_deckManagerEditor.deckManager.inUsePile, _deckManagerEditor.deckManager.discardPile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
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
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled discard pile to top of deck.");
                _deckManagerEditor.deckManager.ShuffleToTopOfDeck(_deckManagerEditor.deckManager.discardPile, _deckManagerEditor.deckManager.deck);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the in use pile and add it to the top of the deck
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled in use pile to top of deck.");
                _deckManagerEditor.deckManager.ShuffleToTopOfDeck(_deckManagerEditor.deckManager.inUsePile, _deckManagerEditor.deckManager.deck);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("To Top of Discard Pile");

            // shuffle the deck and add it to the top of the discard pile
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled deck to top of discard pile.");
                _deckManagerEditor.deckManager.ShuffleToTopOfDeck(_deckManagerEditor.deckManager.deck, _deckManagerEditor.deckManager.discardPile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the in use pile and add it to the top of the discard pile
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled in use pile to top of discard pile.");
                _deckManagerEditor.deckManager.ShuffleToTopOfDeck(_deckManagerEditor.deckManager.inUsePile, _deckManagerEditor.deckManager.discardPile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("To Top of In Use Pile");

            // shuffle the deck and add it to the top of the in use pile
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled deck to top of in use pile.");
                _deckManagerEditor.deckManager.ShuffleToTopOfDeck(_deckManagerEditor.deckManager.deck, _deckManagerEditor.deckManager.inUsePile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the discard pile and add it to the top of the in use pile
            if (GUILayout.Button("Discard Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled discard pile to top of in use pile.");
                _deckManagerEditor.deckManager.ShuffleToTopOfDeck(_deckManagerEditor.deckManager.discardPile, _deckManagerEditor.deckManager.inUsePile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
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
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled discard pile to bottom of deck.");
                _deckManagerEditor.deckManager.ShuffleToBottomOfDeck(_deckManagerEditor.deckManager.discardPile, _deckManagerEditor.deckManager.deck);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the in use pile and add it to the bottom of the deck
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled in use pile to bottom of deck.");
                _deckManagerEditor.deckManager.ShuffleToBottomOfDeck(_deckManagerEditor.deckManager.inUsePile, _deckManagerEditor.deckManager.deck);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("To Bottom of Discard Pile");

            // shuffle the deck and add it to the bottom of the discard pile
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled deck to bottom of discard pile.");
                _deckManagerEditor.deckManager.ShuffleToBottomOfDeck(_deckManagerEditor.deckManager.deck, _deckManagerEditor.deckManager.discardPile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the in use pile and add it to the bottom of the discard pile
            if (GUILayout.Button("In Use Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled in use pile to bottom of discard pile.");
                _deckManagerEditor.deckManager.ShuffleToBottomOfDeck(_deckManagerEditor.deckManager.inUsePile, _deckManagerEditor.deckManager.discardPile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("To Bottom of In Use Pile");

            // shuffle the deck and add it to the bottom of the in use pile
            if (GUILayout.Button("Deck"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled deck to bottom of in use pile.");
                _deckManagerEditor.deckManager.ShuffleToBottomOfDeck(_deckManagerEditor.deckManager.deck, _deckManagerEditor.deckManager.inUsePile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            // shuffle the discard pile and add it to the bottom of the in use pile
            if (GUILayout.Button("Discard Pile"))
            {
                Undo.RecordObjects(_deckManagerEditor.targets, "Shuffled discard pile to bottom of in use pile.");
                _deckManagerEditor.deckManager.ShuffleToBottomOfDeck(_deckManagerEditor.deckManager.discardPile, _deckManagerEditor.deckManager.inUsePile);
                EditorUtility.SetDirty(_deckManagerEditor.target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }
    }
}