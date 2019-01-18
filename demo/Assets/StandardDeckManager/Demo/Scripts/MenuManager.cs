using UnityEngine;

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
        GameManager.instance.GoToBlackjack();
    }

    // go to war demo
    public void GoToWar()
    {
        // load the scene
        GameManager.instance.GoToWar();
    }

    // go to match demo
    public void GoToMatch()
    {
        // load the scene
        GameManager.instance.GoToMatch();
    }
}
