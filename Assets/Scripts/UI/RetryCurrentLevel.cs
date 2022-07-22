using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Reload the current level the player is currently in. Used in the case the player dies to retry.
/// </summary>
public class RetryCurrentLevel : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Enum that indicates a scene
    /// </summary>
    public SceneLoader.Scene scene;

    /// <summary>
    /// Reloads the scene indicated in the respective variable
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.LoadScene(scene);
    }
}
