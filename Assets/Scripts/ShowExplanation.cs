using UnityEngine;

// Simple logic to hide and show the UI explanation on contact with the player
public class ShowExplanation : MonoBehaviour
{
    // Which object to manipulate
    public GameObject effectorExplanation;

    // Guarantee that the object will begin deactivated
    private void Awake()
    {
        effectorExplanation.SetActive(false);
    }

    // On trigger with the player, activate the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            effectorExplanation.SetActive(true);
        }
    }

    // On trigger exit with the player, deactivate the object
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            effectorExplanation.SetActive(false);
        }
    }
}
