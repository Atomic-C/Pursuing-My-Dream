using UnityEngine;

/// <summary>
/// Class that controls the background parallax effect
/// </summary>
public class ParalllaxBackground : MonoBehaviour
{
    /// <summary>
    /// Vector2 used to make the background move slower than the main camera, in the x and y axis
    /// </summary>
    [SerializeField] private Vector2 _parallaxEffectMultiplier;

    /// <summary>
    /// Bool used to trigger the infinite scroll horizontally
    /// </summary>
    [SerializeField] private bool _infiniteHorizontal;

    /// <summary>
    /// Bool used to trigger the infinite scroll vertically
    /// </summary>
    [SerializeField] private bool _infiniteVertical;

    /// <summary>
    /// Bool used to determine if the sprite used had its scale changed
    /// </summary>
    [SerializeField] private bool _textureIsScaled;

    /// <summary>
    /// Bool used to determine if the parallax effect will be applied or not
    /// </summary>
    [SerializeField] private bool _useParallax;

    /// <summary>
    /// Float used to determine the speed in which the sprite will scroll (horizontally)
    /// </summary>
    [SerializeField] private float _autoScrollSpeed;
    
    /// <summary>
    /// The transform of the main camera
    /// </summary>
    private Transform _cameraTransform;

    /// <summary>
    /// The last position of the main camera
    /// </summary>
    private Vector3 _lastCameraPosition;

    /// <summary>
    /// Floats used to figure the sprite texture unit size 
    /// </summary>
    private float _textureUnitSizeX, _textureUnitSizeY;

    /// <summary>
    /// Cache the necessary variables
    /// </summary>
    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        // Calculate the texture unity size in both axis and if the sprite had its texture scale changed, use its value into the calculation
        _textureUnitSizeX = _textureIsScaled ? (texture.width / sprite.pixelsPerUnit) * transform.localScale.x : texture.width / sprite.pixelsPerUnit;
        _textureUnitSizeY = _textureIsScaled ? (texture.height / sprite.pixelsPerUnit) * transform.localScale.y : texture.height / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        if (_useParallax)
        {
            // Calculate how much the camera has moved since the previous frame
            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            // Apply the parallax multiplier and auto scroll speed
            transform.position += new Vector3((deltaMovement.x + _autoScrollSpeed) * _parallaxEffectMultiplier.x, deltaMovement.y * _parallaxEffectMultiplier.y);
            _lastCameraPosition = _cameraTransform.position; 
        }

        if (_infiniteHorizontal)
        {
            // If the difference beteween the camera position and this transform position is equal or bigger than the sprite texture unit size (x axis, positive or negative value)
            // it means the player moved past the sprite texture size, then relocate the sprite creating the illusion of infinite background
            if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
            { 
                float offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
                transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX, transform.position.y);      
            }
        }

        if(_infiniteVertical)
        {
            // The same as above but for the y axis
            if (Mathf.Abs(_cameraTransform.position.y - transform.position.y) >= _textureUnitSizeY)
            {
                float offsetPositionY = (_cameraTransform.position.y - transform.position.y) % _textureUnitSizeY;
                transform.position = new Vector3(_cameraTransform.position.x, transform.position.y + offsetPositionY);
            }
        }
    }
}
