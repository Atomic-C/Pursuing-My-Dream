using TMPro;
using UnityEngine;

/// <summary>
/// Class that spawns an game object after every x seconds
/// </summary>
public class ConstantForceSpawn : MonoBehaviour
{
    /// <summary>
    /// Which object to spawn
    /// </summary>
    public GameObject objToSpawn;

    /// <summary>
    /// Float used as a timer to spawn the object (fixed and set in the editor)
    /// </summary>
    public float spawnTimer;

    /// <summary>
    /// UI timer to displaye the time before each spawn
    /// </summary>
    public TextMeshProUGUI uiTimer;

    /// <summary>
    /// Bool used to show or hide the timer before spawn
    /// </summary>
    public bool showTimer;

    /// <summary>
    /// The explanation game object
    /// </summary>
    public GameObject constantForceExplanation;

    /// <summary>
    /// Float used as an actual timer (will be manipulated as times passes)
    /// </summary>
    private float _timer;

    /// <summary>
    /// Set the current timer / UI timer at start
    /// </summary>
    private void Start()
    {
        _timer = spawnTimer;
        ShowHideUITimer();
    }

    /// <summary>
    /// Decrement the timer as time passes by, instantiate the game object at this one position as the timer is depleted and reset it
    /// Also, show / hide and set the UI timer
    /// </summary>
    void Update()
    {
        _timer -= Time.deltaTime;

        // Show the UI timer when the explanation object is active
        ShowHideUITimer();
        if (_timer <= 0)
        {
            Instantiate(objToSpawn, transform.position, Quaternion.identity);
            _timer = spawnTimer;
        }
    }

    /// <summary>
    /// Function that show and set its value / hide the UI timer
    /// </summary>
    private void ShowHideUITimer()
    {
        showTimer = constantForceExplanation.activeSelf;
        uiTimer.gameObject.SetActive(showTimer);

        if (showTimer) {
            int integerTimer = (int)_timer + 1;
            uiTimer.text = integerTimer.ToString();
        }
    }
}
