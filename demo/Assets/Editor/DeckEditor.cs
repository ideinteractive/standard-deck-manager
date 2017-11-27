using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/// <summary>
/// DeckEditor
/// Description: Custom editor for the DeckManager.
/// </summary>

[CustomEditor(typeof(DeckManager))]
public class DeckEditor : Editor
{
    // reference to the DeckManager
    private DeckManager deckManager;

    // create reorderable lists for each deck
    private ReorderableList deck;

    private void OnEnable()
    {
        // set the reference to the current inspected object
        deckManager = (DeckManager)target;

        // set the reordered deck list to the current deck
        deck = new ReorderableList(serializedObject, serializedObject.FindProperty("deck"), true, true, true, true)
        {
            // render the title of the list
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Deck");
            }
        };

        // render each card detail in the list
        deck.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = deck.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("color"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 75, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("rank"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + 145, rect.y, 20, EditorGUIUtility.singleLineHeight), "of");
            EditorGUI.PropertyField(new Rect(rect.x + 170, rect.y, 80, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("suit"), GUIContent.none);
        };
    }

    // overwrite inspector interface
    public override void OnInspectorGUI()
    {
        // record any changes done within the DeckManager
        Undo.RecordObjects(targets, "DeckManager");

        EditorGUILayout.Space();

        // disiplay the reorderable deck list
        serializedObject.Update();
        deck.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
    }

    // import the standard deck template
    private void ImportStandardDeck()
    {
        
    }
}
