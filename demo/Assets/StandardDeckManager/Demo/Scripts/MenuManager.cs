
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MenuManager
/// Description: Handles our buttons on the main menu
/// </summary>

public class MenuManager : MonoBehaviour
{
    // go to blackjack demo
    public void GoToBlackjack()
    {
        // load the scene
        SceneManager.LoadScene("BlackjackDemo");
    }

    // go to war demo
    public void GoToWar()
    {
        // load the scene
        SceneManager.LoadScene("MemoryMatchDemo");
    }

    // go to match demo
    public void GoToMatch()
    {
        // load the scene
        SceneManager.LoadScene("WarDemo");
    }
}