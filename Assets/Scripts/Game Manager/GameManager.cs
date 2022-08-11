using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script responsible for automatically setting up all the necessary game objects / components references for the yellow slime to work
/// Including its shooting mechanics, health / damage and death mechanics and ready to implement enemies that drop items that uses its collect and upgrade systems
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Scriptable object that holds all current statistics, to be passed between scenes
    /// </summary>
    public PlayerStats playerStats;

    /// <summary>
    /// Bool that dictate if the game manager will do the necessary spawn and setup of objects it the scene
    /// </summary>
    public bool spawnAndSetup;

    /// <summary>
    /// Holds the yellow slime game objects prefab
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Holds the magical gem controller prefab
    /// </summary>
    public MagicalGemController magicalGemController;

    /// <summary>
    /// Holds the camera prefab, already with all necessary components / children objects
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// The game object with the auto letter box script attached
    /// </summary>
    public GameObject forceCameraRatio;
    
    /// <summary>
    /// Reference of the camera currently in the scene
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// Reference the player currently in the scene
    /// </summary>
    private GameObject _player;

    /// <summary>
    /// Reference the player health script
    /// </summary>
    private PlayerHealth _playerHealth;

    /// <summary>
    /// Reference the gem controller currently in the scene
    /// </summary>
    private MagicalGemController _magicalGemController;

    /// <summary>
    /// Reference the upgrade manager script
    /// </summary>
    private UpgradeManager _upgradeManager;

    /// <summary>
    /// Reference the collectable manager script
    /// </summary>
    private CollectableManager _collectableManager;

    /// <summary>
    /// Reference the auto letter box object currently in the scene
    /// </summary>
    private GameObject _forceCameraRatio;

    private void Awake()
    {
        _mainCamera = Camera.main;

        // Get the current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // If it is not a loaded game, set the initial position depending on the current scene
        if (!playerStats.isLoad)
            playerStats.initialLevelPosition = sceneName == SceneLoader.Scene.First_Level.ToString() ? new Vector3(-181.1f, -2.25f, 0f) : new Vector3(-173.61f, -1.71f, 0);
    }

    private void Start()
    {
        if (spawnAndSetup)
        {
            SpawnNecessaryEntities();
            SetupCamera();
            SetupPlayer();
            SetupGemController();
        }

        InitializeScripts();
        CheckIndoor();

        // Begin with the camera fade in effect
        _mainCamera.transform.Find("Fade_InOut_Canvas").GetComponent<Animator>().SetTrigger("FadeIn");

        // Initialize the PlayerStats scriptable object dictionary if it is null
        if (playerStats.currentPosition == null)
        {
            playerStats.currentPosition = new System.Collections.Generic.Dictionary<string, Vector3>();
            // Workaround if the player starts at the shop (in case my teacher try to test exiting the shop, if the player appears at the shop entrance)
            // Set the current position of the First_Level key of the dictionary to be the position of the shop entrance
            playerStats.currentPosition.Add("First_Level", SceneManager.GetActiveScene().name == "Shop" ? new Vector3(-118.29f, 8.65f, 0) : Vector3.zero);
            playerStats.currentPosition.Add("Shop", Vector3.zero);
        }
            
        // Set the player, camera and magical gem controller positions to the default level initial position or to the position saved in the PlayerStats sciptable object
        // positions dictionary, if it exists
        _player.transform.position = playerStats.currentPosition[SceneManager.GetActiveScene().name] == Vector3.zero ? playerStats.initialLevelPosition : playerStats.currentPosition[SceneManager.GetActiveScene().name];
        _mainCamera.transform.position = playerStats.currentPosition[SceneManager.GetActiveScene().name] == Vector3.zero ? playerStats.initialLevelPosition : playerStats.currentPosition[SceneManager.GetActiveScene().name];
        _magicalGemController.transform.position = playerStats.currentPosition[SceneManager.GetActiveScene().name] == Vector3.zero ? playerStats.initialLevelPosition : playerStats.currentPosition[SceneManager.GetActiveScene().name];

    }

    /// <summary>
    /// Reset the PlayerStats scriptable object to the default values
    /// </summary>
    private void OnApplicationQuit()
    {
        playerStats.ResetPlayerStats(_playerHealth.maxHealth, _magicalGemController.maxEnergy);
    }

    /// <summary>
    /// Save the player statistics across several scripts into the PlayerStats scriptable object
    /// </summary>
    public void SavePlayerStats()
    {
        playerStats.strengthRank = _upgradeManager.strengthRank;
        playerStats.speedRank = _upgradeManager.speedRank;
        playerStats.rangeRank = _upgradeManager.rangeRank;
        playerStats.rateOfFireRank = _upgradeManager.rateOfFireRank;
        playerStats.energyRank = _upgradeManager.energyRank;
        playerStats.energyRegenRank = _upgradeManager.energyRegenRank;
        playerStats.lifeRank = _upgradeManager.lifeRank;
        playerStats.coins = _collectableManager.coins;
        playerStats.currentBullet = _magicalGemController.currentBullet;
        playerStats.currentHealth = _playerHealth.currentHealth;
        playerStats.currentEnergy = _magicalGemController.energy;
        playerStats.maxHealth = _playerHealth.currentMaxHealth;
        playerStats.maxEnergy = _magicalGemController.currentMaxEnergy;
        playerStats.currentPosition[SceneManager.GetActiveScene().name] = _player.GetComponent<Platform_Movement>().lastPosition; ;
    }

    /*public void LoadGame()
    {
        SceneLoader.LoadScene(playerStats.currentSceneName);

        Awake();
    }

    private void what()
    {
        if(Input.GetKeyDown(KeyCode.C))
            LoadGame();
    }*/

    /// <summary>
    /// Function that spawns everything necessary for all the system belonging to the yellow slime to work
    /// </summary>
    private void SpawnNecessaryEntities()
    {
        // Check if there is already a camera in the scene and destroy it
        if(_mainCamera != null)
            Destroy(_mainCamera.gameObject);

        // Then cache the refence to the instantiated preconfigured camera
        _mainCamera = Instantiate(mainCamera, transform.position, Quaternion.identity);

        // Instantiate the player object
        _player = Instantiate(player, transform.position, Quaternion.identity);

        // Instantiate the magical gem controller object
        _magicalGemController = Instantiate(magicalGemController, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Set up the camera references to work properly
    /// </summary>
    private void SetupCamera()
    {
        SetupCameraFollow();

        SetupRadar();

        Invoke("SetupAutoLetterBox", .1f);

        CheckBackground();
    }

    /// <summary>
    /// Deactivate the background visuals if the player is inside the shop
    /// </summary>
    private void CheckBackground()
    {
        foreach (GameObject outdoorEffects in GameObject.FindGameObjectsWithTag("OutdoorEffect"))
            outdoorEffects.SetActive(SceneManager.GetActiveScene().name == SceneLoader.Scene.Shop.ToString());
    }

    /// <summary>
    /// Instantiate the auto letter box object and activate it (it is deactivated by default to avoid a bug where it automatically tries to cache a reference to the main camera
    /// which is not available)
    /// </summary>
    private void SetupAutoLetterBox()
    {
        _forceCameraRatio = Instantiate(forceCameraRatio, transform.position, Quaternion.identity);
        _forceCameraRatio.gameObject.SetActive(true);

        Invoke("SetupBoxCollider2D", .5f);
    }

    /// <summary>
    /// Set the camera box collider 2D to be the same size as the camera size
    /// </summary>
    private void SetupBoxCollider2D()
    {
        _mainCamera.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

        float sizeY = _mainCamera.orthographicSize * 2;
        float sizeX = sizeY * _mainCamera.aspect;

        _mainCamera.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(sizeX, sizeY);

    }

    /// <summary>
    /// Get the target for the camera follow script, using the CameraTarget tag (its a child game object of the player object)
    /// </summary>
    private void SetupCameraFollow()
    {
        _mainCamera.gameObject.GetComponent<CameraFollow>().targetPosition = GameObject.FindGameObjectWithTag("CameraTarget").transform;
    }

    /// <summary>
    /// Set the necessary reference for the radar script to work
    /// </summary>
    private void SetupRadar()
    {
        _mainCamera.gameObject.GetComponent<Radar>().triggerCollider = mainCamera.GetComponent<BoxCollider2D>();
        _mainCamera.gameObject.GetComponent<Radar>().playerPosition = player.transform;
    }

   
    /// <summary>
    /// Set up all the necessary references for the player scripts to work
    /// </summary>
    private void SetupPlayer()
    {
        // Get the Canvas object (child of the main camera)
        GameObject UICanvas = GameObject.FindGameObjectWithTag("UICanvas");

        // Set the player health script canvas reference
        _player.GetComponent<PlayerHealth>().canvas = UICanvas.GetComponent<Canvas>();

        // Set the collectable manager script reference to the coins UI counter object
        _player.GetComponent<CollectableManager>().coinsUiText = UICanvas.GetComponentInChildren<TextMeshProUGUI>();

        // Set the collectable manager script reference to the magical gem controller upgrade manager script
        _player.GetComponent<CollectableManager>().upgradeManager = _magicalGemController.GetComponent<UpgradeManager>();

        // Set the collectable manager script reference to the magical gem controller script
        _player.GetComponent<CollectableManager>().magicalGemController = _magicalGemController;
    }

    /// <summary>
    /// Set up all the necessary references for the magical gem controller scripts to work
    /// </summary>
    private void SetupGemController()
    {
        // Set the gem controller energy bar object reference
        _magicalGemController.energyBar = GameObject.FindGameObjectWithTag("EnergyBar");
        // Then deactivate the object (the current bullet is always the default one, which has no alternate shoot and thus, no energy bar / icon)
        _magicalGemController.energyBar.SetActive(false);
        // Set the gem controller energy bar icon object reference
        _magicalGemController.energyBarIcon = GameObject.FindGameObjectWithTag("EnergyBarIcon");
        // Then deactivate the object
        _magicalGemController.energyBarIcon.SetActive(false);
        // Get the reference to the player movement script
        _magicalGemController.playerMovement = _player.GetComponent<Platform_Movement>();
        // Set the gem controller upgrade manager, player health script reference
        _magicalGemController.GetComponent<UpgradeManager>().playerHealth = _player.GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// All scripts have their initialization disabled by default. Those are only enabled, after everything has been set up correctly
    /// </summary>
    private void InitializeScripts()
    {
        if (_magicalGemController == null)
            _magicalGemController = GameObject.FindGameObjectWithTag("MagicalGemController").GetComponent<MagicalGemController>();

        _upgradeManager = _magicalGemController.GetComponent<UpgradeManager>();
        _upgradeManager.SetUpgradeRanks(playerStats.strengthRank, playerStats.speedRank, playerStats.rangeRank, 
        playerStats.rateOfFireRank, playerStats.energyRank, playerStats.energyRegenRank, playerStats.lifeRank);
        _upgradeManager.canStart = true;
        _upgradeManager.Initialize();

        _magicalGemController.currentBullet = playerStats.currentBullet;
        _magicalGemController.SetEnergyBetweenScenes(playerStats.currentEnergy, playerStats.maxEnergy);
        _magicalGemController.canStart = true;
        _magicalGemController.Initialize();
        

        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");

        _playerHealth = _player.GetComponent<PlayerHealth>();
        _playerHealth.SetHealthBetweenScenes(playerStats.lifeRank, playerStats.currentHealth);
        _playerHealth.canStart = true;
        _playerHealth.Initialize();

        _collectableManager = _player.GetComponent<CollectableManager>();
        _collectableManager.coins = playerStats.coins;
        _collectableManager.canStart = true;
        _collectableManager.Initialize();
    }

    /// <summary>
    /// Function to disable the clouds background / dandelion spore effect, if the player is inside the shop
    /// </summary>
    private void CheckIndoor()
    {
        if(SceneManager.GetActiveScene().name == SceneLoader.Scene.Shop.ToString())
        {
            _mainCamera.transform.Find("Clouds_Background").gameObject.SetActive(false);
            _mainCamera.transform.Find("DandelionSpore").gameObject.SetActive(false);
        }
    }
}
