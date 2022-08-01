using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Script that controls a fade out effect, entirely through code without the need of using animations (for damage numbers on enemies)
/// </summary>
public class DamageNumberFadeEffect : MonoBehaviour
{
    /// <summary>
    /// The speed in which the fade effect will play
    /// </summary>
    public float fadeSpeed;

    /// <summary>
    /// This object TextMeshPro component
    /// </summary>
    private TextMeshPro _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        StartCoroutine(FadeEffect());
    }

    /// <summary>
    /// Function that simulates the fade out effect
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeEffect()
    {
        for (float i = 1; i >= 0; i -= fadeSpeed)
        {
            _textMeshProUGUI.color = new Color(_textMeshProUGUI.color.r, _textMeshProUGUI.color.g, _textMeshProUGUI.color.b, i);
            yield return null;
        }

        // Destroy this object after the number disappeared
        Destroy(gameObject);
    } 
}
