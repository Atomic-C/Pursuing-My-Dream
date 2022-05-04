using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Simple script that destroys collectables on touch and increment the player coins ammount each time
public class CollectStuff : MonoBehaviour
{
    public int coins = 0;
    public TextMeshProUGUI coinsUiText; 
    public AudioClip coinPickSound;

    void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (otherObject.gameObject.CompareTag("collectable"))
        {
            coins++;
            coinsUiText.text = "x " + coins;
            Destroy(otherObject.gameObject);
            AudioManager.instance.PlayAudio(coinPickSound);
        }
    }

    /*void OnCollisionEnter2D(Collision2D otherObject)
    {
        if (otherObject.gameObject.CompareTag("collectable"))
        {
            coins++;
            Destroy(otherObject.gameObject);
            AudioManager.instance.PlayAudio(coinPickSound);
        }
    }*/
}
