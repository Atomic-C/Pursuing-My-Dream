using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script that controls a fade in / fade out effect, entirely through code without the need of using animations
/// </summary>
public class UIFadeEffect : MonoBehaviour
{
    /// <summary>
    /// The speed in which the fade effect will play
    /// </summary>
    public float fadeSpeed;

    /// <summary>
    /// This object Image component
    /// </summary>
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// Function that simply start the fade effect coroutine
    /// </summary>
    /// <param name="fadeIn">Bool that dictates which effect will happen</param>
    public void StartFadeEffect(bool fadeIn)
    {
        StartCoroutine(FadeEffect(fadeIn));
    }

    /// <summary>
    /// Function that simulates the fade in / fade out effect0
    /// </summary>
    /// <param name="fadeIn">Bool that dictates which effect will happen</param>
    /// <returns></returns>
    private IEnumerator FadeEffect(bool fadeIn)
    {
        if (fadeIn)
        {
            for (float i = 0; i <= 1; i += fadeSpeed)
            {
                _image.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 1; i >= 0; i -= fadeSpeed)
            {
                _image.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        
        
    } 
}
