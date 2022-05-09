using UnityEngine;

// Simple logic to hide and show the UI explanation on contact with the player
public class ShowExplanation : MonoBehaviour
{
    // Which object to manipulate
    public GameObject effectorExplanation;
    
    // The initial idea was to deactivate everything related to the UI explanation that was connected to this script
    // But since all those objects are "children" of the main one (effectorExplanation), by just deactivating / activating it
    // will affect all the others
    //public GameObject[] pointingSigns;

    // On trigger with the player, activate the parent object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetVisibility();
            AudioManager.instance.PlaySound("ShowExplanation");
        }
    }

    // On trigger exit with the player, deactivate the parent object
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetVisibility();
        }
    }

    // Function to activate / deactivate the parent object
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
