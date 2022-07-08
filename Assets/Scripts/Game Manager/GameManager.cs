using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public MagicalGemController magicalGemController;

    public float cameraFollow_SmoothSpeed;

    private Camera mainCamera;
    [SerializeField] private AudioControl audioControl;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        SpawnNecessaryEntities();
        SetupCamera();
    }

    private void SpawnNecessaryEntities()
    {
        player = Instantiate(player, transform.position, Quaternion.identity);
        magicalGemController = Instantiate(magicalGemController, transform.position, Quaternion.identity);
        magicalGemController.playerMovement = player.GetComponent<Platform_Movement>();
    }

    private void SetupCamera()
    {
        bool hasAudioControl = mainCamera.TryGetComponent<AudioControl>(out AudioControl audioControl);
        if (!hasAudioControl)
            mainCamera.gameObject.AddComponent<AudioControl>();

        bool hasAudioManager = mainCamera.TryGetComponent<AudioManager>(out AudioManager audioManager);
        if(!hasAudioManager)
            mainCamera.gameObject.AddComponent<AudioManager>();

        bool hasCameraFollow = mainCamera.TryGetComponent<CameraFollow>(out CameraFollow cameraFollow);
        if(!hasCameraFollow)
            mainCamera.gameObject.AddComponent<CameraFollow>();

        SetupCameraFollow();

        bool hasBoxCollider2D = mainCamera.gameObject.TryGetComponent<BoxCollider2D>(out BoxCollider2D boxcollider2d);
        if (!hasBoxCollider2D)
            mainCamera.gameObject.AddComponent<BoxCollider2D>();

        SetupBoxCollider2D();

        bool hasRadar = mainCamera.TryGetComponent<Radar>(out Radar radar);
        if(!hasRadar)
            mainCamera.gameObject.AddComponent<Radar>();

        SetupRadar();
    }
    private void SetupCameraFollow()
    {
        mainCamera.gameObject.GetComponent<CameraFollow>().targetPosition = GameObject.Find("Camera Target").transform;
        mainCamera.gameObject.GetComponent<CameraFollow>().smoothSpeed = cameraFollow_SmoothSpeed;
    }

    private void SetupBoxCollider2D()
    {
        mainCamera.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

        float sizeY = mainCamera.orthographicSize * 2;
        float ratio = (float)Screen.width / (float)Screen.height;
        float sizeX = sizeY / ratio;

        mainCamera.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(sizeX, sizeY);

    }

    private void SetupRadar()
    {
        mainCamera.gameObject.GetComponent<Radar>().triggerCollider = mainCamera.GetComponent<BoxCollider2D>();
        mainCamera.gameObject.GetComponent<Radar>().playerPosition = player.transform;
    }
}
