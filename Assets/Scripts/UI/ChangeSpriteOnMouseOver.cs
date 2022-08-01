using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Change the object sprite by another on mouse over, used for UI objects
/// </summary>
public class ChangeSpriteOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The sprite that will be changed into
    /// </summary>
    public Sprite secondarySprite;

    /// <summary>
    /// The image component of this object
    /// </summary>
    private Image _objectImage;

    /// <summary>
    /// The object original sprite
    /// </summary>
    private Sprite _originalSprite;

    /// <summary>
    /// Cache references
    /// </summary>
    private void Awake()
    {
        _objectImage = GetComponent<Image>();
        _originalSprite = _objectImage.sprite;
    }

    /// <summary>
    /// On mouse over, change the sprite and play a sound
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _objectImage.sprite = secondarySprite;
        AudioManager.instance.PlaySound("MouseOver", transform.position);
    }

    /// <summary>
    /// On mouse exit, change the object sprite to its original one
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        _objectImage.sprite = _originalSprite;
    }
}
