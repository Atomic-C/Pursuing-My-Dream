using UnityEngine;

public class ShowExplanation : MonoBehaviour
{
    public GameObject effectorExplanation;

    private void Awake()
    {
        effectorExplanation.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            effectorExplanation.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            effectorExplanation.SetActive(false);
        }
    }
}
