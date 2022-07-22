using TMPro;
using UnityEngine;

/// <summary>
/// Script responsible for automatically setting up all the necessary game objects / components references for the yellow slime to work
/// Including its shooting mechanics, health / damage and death mechanics and ready to implement enemies that drop items that uses its collect and upgrade systems
/// </summary>
public class GameManager : MonoBehaviour
{
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
    /// Game object with the follow helper script attached
    /// </summary>
    public GameObject cameraFollow;
    
    /// <summary>
    /// Reference of the camera currently in the scene
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// Reference the player currently in the scene
    /// </summary>
    private GameObject _player;

    /// <summary>
    /// Reference the gem controller currently in the scene
    /// </summary>
    private MagicalGemController _magicalGemController;

    /// <summary>
    /// Reference the auto letter box object currently in the scene
    /// </summary>
    private GameObject _forceCameraRatio;

    /// <summary>
    /// Get the main camera in the scene
    /// </summary>
    private void Awake()
    {
        _mainCamera = Camera.main;   
    }

    private void Start()
    {
        SpawnNecessaryEntities();
        SetupCamera();
        SetupPlayer();
        SetupGemController();
        InitializeScripts();
    }

    /// <summary>
    /// Function that spawns everything necessary for all the system belonging to the yellow slime to work
    /// </summary>
    private void SpawnNecessaryEntities()
    {
        // Instantiate the follow helper object
        Instantiate(cameraFollow, transform.position, Quaternion.identity);

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
        _player.GetComponent<CollectableManager>().upgradeManager = magicalGemController.GetComponent<UpgradeManager>();

        // Set the collectable manager script reference to the magical gem controller script
        _player.GetComponent<CollectableManager>().magicalGemController = magicalGemController;
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
    /// Set up the camera references to work properly
    /// </summary>
    private void SetupCamera()
    {
        SetupCameraFollow();

        SetupRadar();

        Invoke("SetupAutoLetterBox", .1f);
    }

    /// <summary>
    /// Get the target for the camera follow script, using the CameraTarget tag (its a child game object of the player object)
    /// </summary>
    private void SetupCameraFollow()
    {
        _mainCamera.gameObject.GetComponent<CameraFollow>().targetPosition = GameObject.FindGameObjectWithTag("CameraTarget").transform;
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
    /// Set the necessary reference for the radar script to work
    /// </summary>
    private void SetupRadar()
    {
        _mainCamera.gameObject.GetComponent<Radar>().triggerCollider = mainCamera.GetComponent<BoxCollider2D>();
        _mainCamera.gameObject.GetComponent<Radar>().playerPosition = player.transform;
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
    /// All scripts have their initialization disabled by default. Those are only enabled, after everything has been set up correctly
    /// </summary>
    private void InitializeScripts()
    {
        _magicalGemController.canStart = true;
        _magicalGemController.Initialize();
        _magicalGemController.GetComponent<UpgradeManager>().canStart = true;
        _magicalGemController.GetComponent<UpgradeManager>().Initialize();

        _player.GetComponent<PlayerHealth>().canStart = true;
        _player.GetComponent<PlayerHealth>().Initialize();

        _player.GetComponent<CollectableManager>().canStart = true;
        _player.GetComponent<CollectableManager>().Initialize();
    }
}
