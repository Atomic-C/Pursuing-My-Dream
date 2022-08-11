using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Shows an UI explanation when the mouse is over this game object
/// </summary>
public class ShowExplanationOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Game object used as the explanation
    /// </summary>
    public GameObject rankExplanation;

    /// <summary>
    /// Activate the explanation object on mouse over
    /// </summary>
    /// <param name="eventData">Event data sent by the event system</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        rankExplanation.SetActive(true);
    }

    /// <summary>
    /// Deactivate the explanation object on mouse exit
    /// </summary>
    /// <param name="eventData">Event data sent by the event system</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        rankExplanation.SetActive(false);
    }
}
