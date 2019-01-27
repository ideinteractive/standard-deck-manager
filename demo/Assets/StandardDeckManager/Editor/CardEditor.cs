using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// CardEditor
/// Description: Custom editor to edit our cards in our Deck Manager.
/// </summary>

public class CardEditor : EditorWindow
{
    // reference this window
    public static CardEditor Instance { get; private set; }

    // reference to the DeckManager
    private static DeckManagerEditor deckManagerEditor;

    // check if this window is open or not
    public static bool IsOpen
    {
        get { return Instance != null; }
    }

    // create a variable to hold card information
    public int intCardIndex;
    public bool blnEditingCardFromDeck;
    public bool blnEditingCardFromDiscard;
    public bool blnEditingCardFromInUse;

    // on initialization
    [MenuItem("Standard Deck Manager/Card Editor")]
    private static void Init()
    {
        // get existing open window or if none, make a new one
        CardEditor window = (CardEditor)EditorWindow.GetWindow(typeof(CardEditor), false, "Card Editor");
        window.minSize = new Vector2(325, 140);
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

    // if this window is closed
    public void OnDestroy()
    {
        // if we are using the editor
        if (Application.isEditor)
        {
            // inform the editor we are no longer editing the card
            blnEditingCardFromDeck = false;
            blnEditingCardFromDiscard = false;
            blnEditingCardFromInUse = false;
        }
    }

    // repaint the inspector if it gets updated
    void OnInspectorUpdate()
    {
        Repaint();
    }

    // draw the ui
    void OnGUI()
    {
        // header styles
        GUIStyle styleRowHeader = new GUIStyle();
        styleRowHeader.padding = new RectOffset(0, 0, 3, 3);
        styleRowHeader.normal.background = EditorStyle.SetBackground(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.2f));
        GUIStyle stylePaddingLeft = new GUIStyle();
        stylePaddingLeft.padding = new RectOffset(10, 0, 3, 3);

        // if the deck manager editor is null 
        if (deckManagerEditor == null)
        {
            // try and find a reference to the deck manager editor
            try
            {
                deckManagerEditor = DeckManagerEditor.Instance;
            }
            catch
            {
                return;
            }
        }

        // render our header if we are editing a card
        if (blnEditingCardFromDeck || blnEditingCardFromDiscard || blnEditingCardFromInUse)
        {
            // update serialized object representation
            deckManagerEditor.serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(styleRowHeader);
            EditorGUILayout.LabelField("Edit Selected Card", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        // if we are editing the card
        if (blnEditingCardFromDeck)
        {
            // allows use to create a new card to add
            deckManagerEditor.deckManager.deck[intCardIndex].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deckManagerEditor.deckManager.deck[intCardIndex].color);
            deckManagerEditor.deckManager.deck[intCardIndex].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deckManagerEditor.deckManager.deck[intCardIndex].rank);
            deckManagerEditor.deckManager.deck[intCardIndex].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deckManagerEditor.deckManager.deck[intCardIndex].suit);
            deckManagerEditor.deckManager.deck[intCardIndex].value = EditorGUILayout.IntField("Value", deckManagerEditor.deckManager.deck[intCardIndex].value);
            deckManagerEditor.deckManager.deck[intCardIndex].card = (GameObject)EditorGUILayout.ObjectField("Card", deckManagerEditor.deckManager.deck[intCardIndex].card, typeof(GameObject), true);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(deckManagerEditor.target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
        else if (blnEditingCardFromDiscard)
        {
            // allows use to create a new card to add
            deckManagerEditor.deckManager.discardPile[intCardIndex].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deckManagerEditor.deckManager.discardPile[intCardIndex].color);
            deckManagerEditor.deckManager.discardPile[intCardIndex].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deckManagerEditor.deckManager.discardPile[intCardIndex].rank);
            deckManagerEditor.deckManager.discardPile[intCardIndex].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deckManagerEditor.deckManager.discardPile[intCardIndex].suit);
            deckManagerEditor.deckManager.discardPile[intCardIndex].value = EditorGUILayout.IntField("Value", deckManagerEditor.deckManager.discardPile[intCardIndex].value);
            deckManagerEditor.deckManager.discardPile[intCardIndex].card = (GameObject)EditorGUILayout.ObjectField("Card", deckManagerEditor.deckManager.discardPile[intCardIndex].card, typeof(GameObject), true);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(deckManagerEditor.target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
        else if (blnEditingCardFromInUse)
        {
            // update serialized object representation
            deckManagerEditor.serializedObject.Update();

            // allows use to create a new card to add
            deckManagerEditor.deckManager.inUsePile[intCardIndex].color = (Card.Color)EditorGUILayout.EnumPopup("Color", deckManagerEditor.deckManager.inUsePile[intCardIndex].color);
            deckManagerEditor.deckManager.inUsePile[intCardIndex].rank = (Card.Rank)EditorGUILayout.EnumPopup("Rank", deckManagerEditor.deckManager.inUsePile[intCardIndex].rank);
            deckManagerEditor.deckManager.inUsePile[intCardIndex].suit = (Card.Suit)EditorGUILayout.EnumPopup("Suit", deckManagerEditor.deckManager.inUsePile[intCardIndex].suit);
            deckManagerEditor.deckManager.inUsePile[intCardIndex].value = EditorGUILayout.IntField("Value", deckManagerEditor.deckManager.inUsePile[intCardIndex].value);
            deckManagerEditor.deckManager.inUsePile[intCardIndex].card = (GameObject)EditorGUILayout.ObjectField("Card", deckManagerEditor.deckManager.inUsePile[intCardIndex].card, typeof(GameObject), true);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(deckManagerEditor.target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
        else
        {
            // if no card is currently selected display a warning
            EditorGUILayout.HelpBox("There is no card currently selected. Please select a card from the Deck Manager to edit.", MessageType.Info);
        }

        EditorGUILayout.Space();
    }
}