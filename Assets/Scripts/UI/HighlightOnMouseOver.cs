using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite highligthSprite;

    private Image _objectImage;
    private Sprite _originalSprite;

    private void Awake()
    {
        _objectImage = GetComponent<Image>();
        _originalSprite = _objectImage.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _objectImage.sprite = highligthSprite;
        AudioManager.instance.PlaySound("RetryMouseOver", transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _objectImage.sprite = _originalSprite;
    }
}
