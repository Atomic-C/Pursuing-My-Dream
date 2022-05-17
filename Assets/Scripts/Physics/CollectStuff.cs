using TMPro;
using UnityEngine;

/// <summary>
/// Simple class that destroys collectables on touch and increment the player coins ammount each time
/// </summary>
public class CollectStuff : MonoBehaviour
{
    /// <summary>
    /// Player coins counter
    /// </summary>
    public int coins = 0;

    /// <summary>
    /// UI coins counter
    /// </summary>
    public TextMeshProUGUI coinsUiText;

    /// <summary>
    /// On trigger enter with the player, increment the coins counter, reflect this change to the UI text counter, destroy the coin and plays the collect sound
    /// </summary>
    /// <param name="otherObject">The other object collider</param>
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
}
