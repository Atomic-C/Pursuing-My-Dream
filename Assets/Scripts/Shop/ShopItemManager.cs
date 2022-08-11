using TMPro;
using UnityEngine;

/// <summary>
/// Controls the proccess of buying a single item
/// </summary>
public class ShopItemManager : MonoBehaviour
{
    /// <summary>
    /// Prefab of the item being bought
    /// </summary>
    public GameObject item;

    /// <summary>
    /// Prefab of the buy indicator
    /// </summary>
    public GameObject buyIndicator;

    /// <summary>
    /// Price of this item
    /// </summary>
    [SerializeField] int _itemPrice;

    /// <summary>
    /// Floats used to make the item come flying
    /// </summary>
    [SerializeField] float _minRandomRangeX, _minRandomRangeY, _maxRandomRange;

    /// <summary>
    /// Reference to the player collectable manager (controls the coins the player has)
    /// </summary>
    CollectableManager _collectableManager;

    /// <summary>
    /// The actual buy indicator
    /// </summary>
    GameObject _buyIndicator;

    /// <summary>
    /// Gameobject used to represent the item and its price
    /// </summary>
    GameObject _priceTag;

    /// <summary>
    /// Reference the shopkeeper script, to influence its animation
    /// </summary>
    ShopKeeper _shopKeeper;

    private void Awake()
    {
        // Get the price gameobject (contains the text mesh pro to show the price ammount)
        _priceTag = transform.Find("Price").gameObject;
        // Then set the UI price according to the itemPrice variable
        _priceTag.GetComponent<TextMeshPro>().text = "$" + _itemPrice;
        // Set the item fromShop bool to true, to deactivate its life span depleting, disapearing from the scene (losing an item that you just bought sucks)
        item.GetComponent<EnemyDrop>().fromShop = true;
        // Set the buy indicator above the item and deactivate it
        _buyIndicator = Instantiate(buyIndicator, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
        _buyIndicator.SetActive(false);

        _shopKeeper = GameObject.FindGameObjectWithTag("ShopKeeper").GetComponent<ShopKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        // Since the collectable manager is being set up by the GameManager object, use this validation to guarantee that after it is set up, its reference will be
        // grabbed
        if(_collectableManager == null)
            _collectableManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CollectableManager>();
        CheckInput();
    }

    /// <summary>
    /// Activate the buy indicator when the player stand near the item display table
    /// </summary>
    /// <param name="collision">The other object collider 2D</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _buyIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivate the buy indicator when the player leave the item display table proximity
    /// </summary>
    /// <param name="collision">The other object collider 2D</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _buyIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// If the player stands near the item display table and press the E key, an attempt to buy this item will be made
    /// </summary>
    private void CheckInput()
    {
        if(_buyIndicator.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            BuyAttempt();
        }
    }

    /// <summary>
    /// Verify if the player has enough money to buy the item and instantiate it if it has
    /// If not, play an error sound
    /// </summary>
    private void BuyAttempt()
    {
        if(_collectableManager.coins >= _itemPrice)
        {
            _collectableManager.CoinSpent(_itemPrice);
            GameObject itemBought = Instantiate(item, transform.position, Quaternion.identity);
            itemBought.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(_minRandomRangeX, _maxRandomRange),
                                                                        Random.Range(_minRandomRangeY, _maxRandomRange)), ForceMode2D.Force);
            // Deactivate collision with the player to avoid the item being immediately picked up upon buying (to see the item flying around heh)
            Physics2D.IgnoreCollision(itemBought.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>());

            // Purchase attempt successfull, the shopkeeper will react to it by calling its PurchaseAttempt function
            _shopKeeper.PurchaseAttempt(true);
        }
        else
        {
            AudioManager.instance.PlaySound("NotEnoughMoney", transform.position);

            // Purchase attempt successfull, the shopkeeper will react to it by calling its PurchaseAttempt function
            _shopKeeper.PurchaseAttempt(false);
        }
    }
}
