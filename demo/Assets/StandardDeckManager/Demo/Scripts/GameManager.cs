using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager
/// Description: Manages everything related to scene loading and our music.
/// </summary>

public class GameManager : MonoBehaviour
{
    // static variables
    public static GameManager instance;

    // public variables
    public float maxVolume = 0.25f;             // the max volume level for music
    public float fadeSpeed = 0.5f;              // the music fade speed
    public AudioSource audSrcOne;               // first audio source
    public AudioSource audSrcTwo;               // second audio source
    public AudioClip audClpMainMenu;            // music for main menu
    public AudioClip audClpBlackjack;           // music for blackjack demo
    public AudioClip audClpMatch;               // music for matching demo
    public AudioClip audClpWar;                 // music for war demo

    // private variables
    private bool m_changeMusicToMainMenu;       // change music to the main menu music
    private bool m_changeMusicFromMainMenu;     // change music from the main menu music
    private bool m_fadeInMainMenuMusic;         // fade the main menu music in

    // on awake
    private void Awake()
    {
        // don't destroy this gameobject
        DontDestroyOnLoad(this);

        // check if there's an instance of this object already created
        // and if not assign it, else destory it
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    // on initialization
    void Start()
    {
        // prevent the second audio src from playing
        audSrcTwo.Stop();
        audSrcTwo.volume = 0;

        // set up our references and variables
        m_changeMusicFromMainMenu = false;
        m_changeMusicToMainMenu = false;

        // if it is the main menu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            // if the clip is not set to the main menu music
            // on the first audio src set it and play it
            if (audSrcOne.clip != audClpMainMenu)
                audSrcOne.clip = audClpMainMenu;
            audSrcOne.volume = maxVolume;
            audSrcOne.Play();
        }
    }

    // once per frame
    void Update()
    {
        // if change music from main menu is true
        if (m_changeMusicFromMainMenu)
        {
            // increase the volume for audio src two
            if (audSrcTwo.volume < maxVolume)
                audSrcTwo.volume += Time.deltaTime * fadeSpeed;

            // decrease the volume for audio src one
            audSrcOne.volume -= Time.deltaTime * fadeSpeed;

            // if the volumes are all adjusted
            if (audSrcOne.volume == 0 && audSrcTwo.volume >= maxVolume)
            {
                // set change music back to false
                m_changeMusicFromMainMenu = false;
            }
        }

        // if change music to main menu is true
        if (m_changeMusicToMainMenu)
        {
            // decrease the volume for audio src two
            audSrcTwo.volume -= Time.deltaTime * fadeSpeed;

            // increase the volume for audio src one
            if(audSrcOne.volume < maxVolume)
                audSrcOne.volume += Time.deltaTime * fadeSpeed;

            // if the volumes are all adjusted
            if (audSrcTwo.volume == 0 && audSrcOne.volume >= maxVolume)
            {
                // set change music back to false
                m_changeMusicToMainMenu = false;
            }
        }
    }

    // go to blackjack demo
    public void GoToBlackjack()
    {
        // load the scene
        SceneManager.LoadScene("BlackjackDemo");

        // change the audio
        if (audSrcTwo.clip != audClpBlackjack)
            audSrcTwo.clip = audClpBlackjack;

        // change the music
        m_changeMusicFromMainMenu = true;
        audSrcTwo.Play();
    }

    // go to war demo
    public void GoToWar()
    {
        // load the scene
        SceneManager.LoadScene("WarDemo");

        // change the audio
        if (audSrcTwo.clip != audClpWar)
            audSrcTwo.clip = audClpWar;

        // change the music
        m_changeMusicFromMainMenu = true;
        audSrcTwo.Play();
    }

    // go to match demo
    public void GoToMatch()
    {
        // load the scene
        SceneManager.LoadScene("MatchDemo");

        // change the audio
        if (audSrcTwo.clip != audClpMatch)
            audSrcTwo.clip = audClpMatch;

        // change the music
        m_changeMusicFromMainMenu = true;
        audSrcTwo.Play();
    }

    // go to main menu 
    public void GoToMainMenu()
    {
        // load the scene
        SceneManager.LoadScene("MainMenu");

        // change the audio
        if(audSrcOne.clip != audClpMainMenu)
            audSrcOne.clip = audClpMainMenu;

        // change the music
        m_changeMusicToMainMenu = true;
        audSrcOne.Play();
    }
}
