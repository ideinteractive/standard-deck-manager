using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameControls
/// Description: Common game input shared throughout the scenes.
/// </summary>

public class GameControls : MonoBehaviour
{
    // static variables
    private static GameControls Instance;

    // on awake
    void Awake()
    {
        // if we don't have an instance
        if(!Instance)
        {
            // set this instance
            Instance = this;
        } else
        {
            // else destroy it
            Destroy(this.gameObject);
        }

        // don't destroy this object on load
        DontDestroyOnLoad(this);
    }

    // once per frame
    private void Update()
    {
        // if the M key is pressed
        if (Input.GetKeyDown(KeyCode.M))
        {
            // mute the audio source or unmute it
            if (AudioListener.volume < 1)
                AudioListener.volume = 1;
            else
                AudioListener.volume = 0;
        }

        // if the R key is pressed
        if (Input.GetKeyDown(KeyCode.R))
            // restart the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // if the B key is pressed
        if (Input.GetKeyDown(KeyCode.B))
        {
            // if the scene is not the main menu
            if (SceneManager.GetActiveScene().name != "MainMenu")
                // load the main menu scene
                SceneManager.LoadScene("MainMenu");
        }
    }
}