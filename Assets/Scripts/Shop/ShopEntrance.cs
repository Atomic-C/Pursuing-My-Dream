using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load the scenes from the first level to shop and vice-versa
/// </summary>
public class ShopEntrance : MonoBehaviour
{
    /// <summary>
    /// Prefab of the arrow indicator that tell the player to press UP (W in this case)
    /// </summary>
    public GameObject arrowUp;

    /// <summary>
    /// Actual arrow indicator
    /// </summary>
    private GameObject _arrowUp;

    /// <summary>
    /// Instantiate the arrow object at the portal position and deactivate it
    /// </summary>
    private void Awake()
    {
        _arrowUp = Instantiate(arrowUp, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);
    }

    private void Update()
    {
        CheckInput();
    }

    /// <summary>
    /// Activates the arrow object when the player stays inside its trigger
    /// </summary>
    /// <param name="collision">The other object collider</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _arrowUp.GetComponent<Animator>().SetBool("TouchingPlayer", true);
        } 
    }

    /// <summary>
    /// Deactivates the arrow object when the player exits the trigger radius
    /// </summary>
    /// <param name="collision">The other object collider</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _arrowUp.GetComponent<Animator>().SetBool("TouchingPlayer", false);
        }
    }

    /// <summary>
    /// Check if the player pressed W while near the portal
    /// </summary>
    private void CheckInput()
    {
        if (_arrowUp.GetComponent<Animator>().GetBool("TouchingPlayer") && Input.GetKeyDown(KeyCode.W))
        {
            GameObject.FindGameObjectWithTag("MainCamera").transform.Find("Fade_InOut_Canvas").GetComponent<Animator>().SetTrigger("FadeOut");
            Invoke("LoadScene", 0.2f);
        }
    }

    /// <summary>
    /// Save the player stats into the scriptable object and Load the shop / first level scene 
    /// </summary>
    private void LoadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SavePlayerStats();

        SceneLoader.LoadScene(currentSceneName == SceneLoader.Scene.Shop.ToString() ? SceneLoader.Scene.First_Level : SceneLoader.Scene.Shop);
    }
}
