using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simples script that controls a individual heart behavior
/// </summary>
public class Heart : MonoBehaviour
{
    /// <summary>
    /// Bool that determines if this heart is full or depleted
    /// </summary>
    public bool isDepleted;

    /// <summary>
    /// Sprites for the full / depleted heart art
    /// </summary>
    public Sprite depletedHeart, fullHeart;

    /// <summary>
    /// This heart Image component
    /// </summary>
    private Image heartImage;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    /// <summary>
    /// Player took a hit, so set this heart sprite to the depleted version
    /// </summary>
    public void TookHit()
    {
        heartImage.sprite = depletedHeart;
        isDepleted = true;
    }

    /// <summary>
    /// Player got a heart back, so set this heart sprite to the full version
    /// </summary>
    public void GotHeart()
    {
        heartImage.sprite = fullHeart;
        isDepleted = false;
    }
}
