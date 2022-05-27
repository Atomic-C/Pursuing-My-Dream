using UnityEngine;

/// <summary>
/// Simple logic to hide and show the UI explanation on contact with the player
/// </summary>
public class ShowExplanation : MonoBehaviour
{
    /// <summary>
    /// Which object to manipulate
    /// </summary>
    public GameObject effectorExplanation;

    private Camera mainCamera;

    public Transform exclamationSignPosition;

    private Transform cameraTargetPosition;

    private void Start()
    {
        effectorExplanation.SetActive(false);
        mainCamera = Camera.main;
        cameraTargetPosition = GameObject.Find("CameraTarget").transform;
    }

    // The initial idea was to deactivate everything related to the UI explanation that was connected to this script
    // But since all those objects are "children" of the main one (effectorExplanation), by just deactivating / activating it
    // will affect all the others
    //public GameObject[] pointingSigns;

    /// <summary>
    /// On trigger with the player, activate the parent object
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetVisibility();
            AudioManager.instance.PlaySound("ShowExplanation", gameObject.transform.position);
            mainCamera.gameObject.GetComponent<CameraFollow>().targetPosition = exclamationSignPosition;
        }
    }

    /// <summary>
    /// On trigger exit with the player, deactivate the parent object
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetVisibility();
            mainCamera.gameObject.GetComponent<CameraFollow>().targetPosition = cameraTargetPosition;
        }
    }

    /// <summary>
    /// Function to activate / deactivate the parent object
    /// </summary>
    private void SetVisibility()
    {
        effectorExplanation.SetActive(!effectorExplanation.activeSelf);

        // Initial idea to deactivate / activate everything
        /*for (int i = 0; i < pointingSigns.Length; i++)
        {
        pointingSigns[i].SetActive(!pointingSigns[i].activeSelf);
        }*/
    }
}
