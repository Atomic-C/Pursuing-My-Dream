using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Script that controls the shopkeeper character
/// </summary>
public class ShopKeeper : MonoBehaviour
{
    /// <summary>
    /// Bool that determines if the shopkeeper is talking
    /// </summary>
    public bool isTalking;

    /// <summary>
    /// Floats that serves as timers for the shopkeeper animations, the balloon speech display time and speed which the text will appear
    /// </summary>
    public float randomAnimationTimer, speechTimer, textDelay, textDisplay;

    /// <summary>
    /// Private timers that will be affected by the passing of time
    /// </summary>
    float _randomAnimationTimer, _speechTimer;

    /// <summary>
    /// The shopkeeper's animator
    /// </summary>
    Animator animator;

    /// <summary>
    /// The speech balloon
    /// </summary>
    GameObject _balloon;

    /// <summary>
    /// The text component of the speech balloon
    /// </summary>
    TextMeshPro _speech;

    /// <summary>
    /// Phrases when the purchase was successfull
    /// </summary>
    readonly string[] _itemSold = { "Thanks my friend! I really needed those coins..",
                                    "Ohhh, nice choice! I would have choosed the same!"};

    /// <summary>
    /// Phrases when the purchase failed
    /// </summary>
    readonly string[] _purchaseFailed = { "Sorry friend, it seems you're short of coins!", 
                                          "Not enough money for that, apparently...." };

    /// <summary>
    /// Phrases for random speech
    /// </summary>
    readonly string[] _randomSpeech = { "I gave up that life a long time ago. It's no use to keep fighting them..", 
                                        "I had a bright gem like yours, once. It don't float anymore..", 
                                        "How's the kingdom outside? I've heard the wizard lost another gem.."};

    /// <summary>
    /// Phrase that will play when the player enters the shop
    /// </summary>
    readonly string welcome = "Welcome to my humble shop friend! Feel free to browse my options..";

    private void Awake()
    {
        isTalking = false;
        _randomAnimationTimer = randomAnimationTimer;
        _speechTimer = speechTimer;
        animator = GetComponent<Animator>();
        _balloon = GameObject.FindGameObjectWithTag("ShopKeeperSpeech");
        _speech = _balloon.transform.Find("Text").GetComponent<TextMeshPro>();
        _balloon.SetActive(false);
    }

    private void Start()
    {
        // Play the welcome message at start
        StartCoroutine(AppearingText(welcome));
    }

    // Update is called once per frame
    void Update()
    {
        RandomAnimation();
        RandomSpeech();
    }

    /// <summary>
    /// Function used by the ShopItemManager script
    /// When an purchase attempt happens, this function is called, displaying a success or 
    /// fail message from the shopkeeper, if there's no message currently in display
    /// </summary>
    /// <param name="success"></param>
    public void PurchaseAttempt(bool success)
    {
        if (!isTalking && success)
        {
            StartCoroutine(AppearingText(_itemSold[Random.Range(0, _itemSold.Length)]));
        }
        else if(!isTalking)
        {
            StartCoroutine(AppearingText(_purchaseFailed[Random.Range(0, _purchaseFailed.Length)]));
        }
    } 

    /// <summary>
    /// Plays a random iddle animation
    /// </summary>
    private void RandomAnimation()
    {
        if (!isTalking)
        {
            _randomAnimationTimer -= Time.deltaTime;

            if(_randomAnimationTimer <= 0)
            {
                int randomAnim = Random.Range(1, 3);

                animator.SetInteger("Iddle_Blink", randomAnim);

                _randomAnimationTimer = randomAnimationTimer;
            }
        }
        else
        {
            _randomAnimationTimer -= Time.deltaTime;

            if(_randomAnimationTimer <= 0)
            {
                _randomAnimationTimer = randomAnimationTimer;
            }
        }
    }

    /// <summary>
    /// Display a random speech message
    /// </summary>
    private void RandomSpeech()
    {
        if (!isTalking)
        {
            _speechTimer -= Time.deltaTime;

            if (_speechTimer <= 0)
            {
                StartCoroutine(AppearingText(_randomSpeech[Random.Range(0, _randomSpeech.Length)]));
            }
        }
    }

    /// <summary>
    /// Coroutine that controls the speech balloon
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator AppearingText(string text)
    {
        isTalking = true;

        // Play the talking animation
        animator.SetBool("IsTalking", isTalking);

        // Activate the speech balloon
        _balloon.SetActive(true);

        // Display the message, letter by letter, according to the textDelay (speed in which the letters appears) variable 
        for (int i = 0; i < text.Length; i++)
        {
            _speech.text = text.Substring(0, i);

            yield return new WaitForSeconds(textDelay);
        }

        // Keeps the full message in display for the time set in the textDisplay variable
        yield return new WaitForSeconds(textDisplay);

        // Resets the speech timer
        _speechTimer = speechTimer;

        isTalking = false;

        // Exit the talking animation
        animator.SetBool("IsTalking", isTalking);

        // Deactivates the balloon
        _balloon.SetActive(false);
    }
}
