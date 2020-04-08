using UnityEngine;
using UnityEngine.SceneManagement;

namespace StandardDeckManager.Demo.Scripts
{
    /// <summary>
    /// MenuManager
    /// Description: Handles our buttons on the main menu
    /// </summary>

    public class MenuManager : MonoBehaviour
    {
        // on initialization
        private void Start()
        {
            this.GetComponent<AudioSource>().Play();
        }

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
            SceneManager.LoadScene("WarDemo");
        }

        // go to match demo
        public void GoToMatch()
        {
            // load the scene
            SceneManager.LoadScene("MemoryMatchDemo");
        }
    }
}