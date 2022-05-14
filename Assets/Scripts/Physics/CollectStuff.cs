using TMPro;
using UnityEngine;

// Simple script that destroys collectables on touch and increment the player coins ammount each time
public class CollectStuff : MonoBehaviour
{
    public int coins = 0;
    public TextMeshProUGUI coinsUiText;

    // On trigger enter with the player, increment the coins counter, reflect this change to the UI text counter, destroy the coin and plays the collect sound
    void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (otherObject.gameObject.CompareTag("collectable"))
        {
            coins++;
            coinsUiText.text = "x " + coins;
            Destroy(otherObject.gameObject);
            AudioManager.instance.PlaySound("CoinPickup", gameObject.transform.position);
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
