using UnityEngine;

/// <summary>
/// Class that controls the crosshair sprite
/// </summary>
public class CrosshairController : MonoBehaviour
{
    public Sprite pausePointer;

    /// <summary>
    /// Vector3 that holds the position of the mouse
    /// </summary>
    private Vector3 _mousePosition;

    /// <summary>
    /// The scene main camera
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// The crosshair original sprite
    /// </summary>
    private Sprite _originalSprite;

    /// <summary>
    /// The sprite renderer of the crosshair game object
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalSprite = _spriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
        GameIsPaused();
    }

    /// <summary>
    /// Function that changes the sprite of this object with another
    /// </summary>
    /// <param name="sprite">Sprite that this object will use</param>
    public void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        _originalSprite = sprite;
    }

    /// <summary>
    /// Function that makes the crosshair sprite follow the mouse position
    /// </summary>
    private void FollowMouse()
    {
        _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0f);
    }

    /// <summary>
    /// If the game was paused, change the crosshair sprite with a hand pointer one
    /// </summary>
    private void GameIsPaused()
    {
        if (Time.timeScale == 0)
            _spriteRenderer.sprite = pausePointer;
        else
            _spriteRenderer.sprite = _originalSprite;
    }


}
