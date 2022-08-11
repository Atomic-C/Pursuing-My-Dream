using TMPro;
using UnityEngine;

/// <summary>
/// Script responsible for controlling all collectable items (coins, upgrades, etc)
/// </summary>
public class CollectableManager : MonoBehaviour
{
    /// <summary>
    /// Bool that initializes this script, used by the GameManager script
    /// </summary>
    public bool canStart;

    /// <summary>
    /// Player coins counter
    /// </summary>
    public int coins = 0;

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

    private void Awake()
    {
        if (canStart)
        {
            _playerHealth = GetComponent<PlayerHealth>();
            InitializeCoins();
        }
            
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
    /// Function used by the ShopItemManager script
    /// Player bought an item so decrease the coins ammount and play a sound
    /// </summary>
    /// <param name="ammount">The ammount of coins spent in the purchase</param>
    public void CoinSpent(int ammount)
    {
        coins -= ammount;
        coinsUiText.text = "x " + coins;
        AudioManager.instance.PlaySound("ItemBought", gameObject.transform.position);
    }

    /// <summary>
    /// Initiliaze the player coins counter
    /// </summary>
    private void InitializeCoins()
    {
        coinsUiText.text = "x " + coins;
    }

    /// <summary>
    /// Function responsible for the coin pickup mechanic, increasing the player coins counter in the UI
    /// </summary>
    private void CoinPickup(int value)
    {
        coins += value;
        coinsUiText.text = "x " + coins;
        AudioManager.instance.PlaySound("CoinPickup", gameObject.transform.position);
    }
}
