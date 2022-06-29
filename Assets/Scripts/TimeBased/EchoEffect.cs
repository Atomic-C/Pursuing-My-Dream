using UnityEngine;

/// <summary>
/// Class used to provide a 'echo' effect, where a trail of an specific sprite (or an alternate one, if applied) is repetitively instantiate behind the target object
/// giving a nice visual aspect
/// </summary>
public class EchoEffect : MonoBehaviour
{
    /// <summary>
    /// The game object to be instantiate
    /// </summary>
    public GameObject echoEffectPrefab;

    /// <summary>
    /// Bool used to determine if this echo belongs to the alternate shoot 
    /// </summary>
    public bool alternateShoot;

    /// <summary>
    /// The alternate shoot sprite
    /// </summary>
    public Sprite alternateSprite;

    /// <summary>
    /// The timer before each instance of the echo effect
    /// </summary>
    public float echoTimer;

    /// <summary>
    /// The position of the object receiving the effect
    /// </summary>
    private Transform _target;

    /// <summary>
    /// Timer that will be used in the calculation (by Time.deltaTime)
    /// </summary>
    private float _actualEchoTimer;

    // Start is called before the first frame update
    void Start()
    {
        _target = transform;
        _actualEchoTimer = echoTimer;
    }
    /// <summary>
    /// Each time the timer depletes, instantiate the echo game object at the position of this object, change the echo sprite if it is from an alternate shoot, destroy the echo
    /// object after 2 seconds and resets the timer
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (_actualEchoTimer <= 0)
        {
            GameObject echo = Instantiate(echoEffectPrefab, _target.position, Quaternion.identity);
            if(alternateShoot)
                echo.GetComponent<SpriteRenderer>().sprite = alternateSprite;
            Destroy(echo, 2f);
            _actualEchoTimer = echoTimer;
        }
        else
        {
            _actualEchoTimer -= Time.deltaTime;
        }
    }
}
