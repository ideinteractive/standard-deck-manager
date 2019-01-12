#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// DeckOptions
/// Description: Demonstrates some of the basic functionality of the DeckManager.
/// This script should not be included in the compiled version of your project.
/// </summary>

[ExecuteInEditMode]
public class DeckOptions : MonoBehaviour
{
    // public variables
    public DeckManager deckManager;    // reference our deck manager

	// on awake
	void Awake ()
    {
		// set the deck manager
        try
        {
            deckManager = this.GetComponent<DeckManager>();
        } catch
        {
            Debug.LogWarning("This component has no Deck Manager.");
        }
	}
}
#endif