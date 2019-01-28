using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameControls
/// Description: Common game input shared throughout the scenes.
/// </summary>

public class GameControls : MonoBehaviour
{
    // static variables
    public static GameControls Instance;

    // on awake
    void Awake()
    {
        // if we already have an instance
        if (Instance != null)
        {
            // destroy this object
            Destroy(Instance);
        }
        else
        {
            // set this instance
            Instance = this;

            // don't desroy this object
            DontDestroyOnLoad(this);
        }
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
    }
}