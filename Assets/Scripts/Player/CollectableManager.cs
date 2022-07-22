using TMPro;
using UnityEngine;

/// <summary>
/// Script responsible for controlling all collectable items (coins, upgrades, etc)
/// </summary>
public class CollectableManager : MonoBehaviour
{
    public bool canStart;

    /// <summary>
    /// UI coins counter
    /// </summary>
    public TextMeshProUGUI coinsUiText;

    /// <summary>
    /// Holds a reference to the UpgradeManager script
    /// </summary>
    public UpgradeManager upgradeManager;

    /// <summary>
    /// Holds a reference to the MagicalGemController script
    /// </summary>
    public MagicalGemController magicalGemController;

    /// <summary>
    /// Holds a reference to the PlayerHealth script
    /// </summary>
    private PlayerHealth _playerHealth;

    /// <summary>
    /// Player coins counter
    /// </summary>
    private int _coins = 0;

    private void Awake()
    {
        if(canStart)
            _playerHealth = GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// On trigger enter with the player, increment the coins counter, reflect this change to the UI text counter, destroy the coin and plays the collect sound
    /// </summary>
    /// <param name="otherObject">The other object collider</param>
    void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (otherObject.gameObject.CompareTag("collectable"))
        {
            switch (otherObject.gameObject.GetComponent<EnemyDrop>().dropType)
            {
                case EnemyDrop.DropType.Health:
                    _playerHealth.PickedUpHealth(otherObject.gameObject.GetComponent<EnemyDrop>().incrementValue);
                    break;
                case EnemyDrop.DropType.Energy:
                    magicalGemController.PickedUpEnergy(otherObject.gameObject.GetComponent<EnemyDrop>().incrementValue);
                    break;
                case EnemyDrop.DropType.Upgrade:
                    upgradeManager.PickedUpUpgrade();
                    break;
                case EnemyDrop.DropType.Coin:
                    CoinPickup((int)otherObject.gameObject.GetComponent<EnemyDrop>().incrementValue);
                    break;
            }

            Destroy(otherObject.gameObject);
        }
    }

    public void Initialize()
    {
        Awake();
    }

    /// <summary>
    /// Set the reference to the instantiated UpgradeManager object
    /// </summary>
    /// <param name="upgradeManager">The instantiated UpgradeManager object</param>
    public void SetUpgradeManager(UpgradeManager upgradeManager)
    {
        this.upgradeManager = upgradeManager;
    }

    /// <summary>
    /// Function responsible for the coin pickup mechanic, increasing the player coins counter in the UI
    /// </summary>
    private void CoinPickup(int value)
    {
        _coins += value;
        coinsUiText.text = "x " + _coins;
        AudioManager.instance.PlaySound("CoinPickup", gameObject.transform.position);
    }
}
