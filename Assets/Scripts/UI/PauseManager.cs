using UnityEngine;

public class PauseManager : MonoBehaviour
{
    /// <summary>
    /// Value used as the pause timer
    /// </summary>
    public float pauseTimer;

    /// <summary>
    /// Bool used to show / hide the mouse cursor
    /// </summary>
    private bool _showCursor;

    /// <summary>
    /// Bool that allows or not to pause the game
    /// </summary>
    private bool _canPause;

    /// <summary>
    /// Bool that is used to communicate with the player health script
    /// </summary>
    private bool _playerIsDead;

    /// <summary>
    /// Value used as the pause timer, which will be affected by the passing of time
    /// </summary>
    private float _pauseTimer;

    /// <summary>
    /// The pause menu object
    /// </summary>
    private GameObject _pauseMenu;

    private void Awake()
    {
        _showCursor = false;
        _pauseTimer = pauseTimer;
        _canPause = true;
        _playerIsDead = false;
        _pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
    }

    /// <summary>
    /// Start with the mouse cursor hidden
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerIsDead)
        {
            AllowPause();
            PauseGame();
        }
    }

    /// <summary>
    /// Function used by the player health script, when the player died, disabling the pause feature
    /// </summary>
    public void PlayerDied()
    {
        _playerIsDead = true;
    }

    /// <summary>
    /// Function that show / hide the mouse cursor when escape is pressed
    /// And pauses / unpauses the game, showing / hiding the pause menu
    /// </summary>
    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _canPause)
        {
            _showCursor = !_showCursor;

            if (_showCursor)
            {
                Time.timeScale = 0f;
                AudioManager.instance.PauseSound("Music");
                _pauseMenu?.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                AudioManager.instance.PlaySound("Music", transform.position);
                _pauseMenu?.SetActive(false);
            }

            AudioManager.instance.PlaySound("Pause", transform.position);
            _canPause = !_canPause;
        }

        Cursor.visible = _showCursor;
    }

    /// <summary>
    /// Function that runs when the player just paused / unpaused
    /// Has a built-in timer that prevents multiple pauses / unpauses in order to avoid multiple pause sounds playing
    /// </summary>
    private void AllowPause()
    {
        if (!_canPause)
        {
            // Using unscaled delta time because the time scale, at this point, is zero
            _pauseTimer -= Time.unscaledDeltaTime;
            if (_pauseTimer <= 0f)
            {
                _canPause = !_canPause;
                _pauseTimer = pauseTimer;
            }
        }
    }
}
