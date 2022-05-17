using UnityEngine;

/// <summary>
/// Class that controls the background parallax effect
/// </summary>
public class ParalllaxBackground : MonoBehaviour
{

    [SerializeField] private Vector2 parallaxEffectMultiplier;

    /// <summary>
    /// Bool used to trigger the infinite scroll horizontally
    /// </summary>
    [SerializeField] private bool infiniteHorizontal;

    /// <summary>
    /// Bool used to trigger the infinite scroll vertically
    /// </summary>
    [SerializeField] private bool infiniteVertical;

    /// <summary>
    /// Bool used to determine if the sprite used had its scale changed
    /// </summary>
    [SerializeField] private bool textureIsScaled;

    /// <summary>
    /// Bool used to determine if the parallax effect will be applied or not
    /// </summary>
    [SerializeField] private bool useParallax;

    /// <summary>
    /// Float used to determine the speed in which the sprite will scroll (horizontally)
    /// </summary>
    [SerializeField] private float autoScrollSpeed;
    
    /// <summary>
    /// The transform of the camera
    /// </summary>
    private Transform cameraTransform;

    /// <summary>
    /// The last position of the camera
    /// </summary>
    private Vector3 lastCameraPosition;

    /// <summary>
    /// 
    /// </summary>
    private float textureUnitSizeX, textureUnitSizeY;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = textureIsScaled ? (texture.width / sprite.pixelsPerUnit) * transform.localScale.x : texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = textureIsScaled ? (texture.height / sprite.pixelsPerUnit) * transform.localScale.y : texture.height / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        if (useParallax)
        {
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3((deltaMovement.x + autoScrollSpeed) * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;
        }

        if (infiniteHorizontal)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
            }
        }

        if(infiniteVertical)
        {
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(cameraTransform.position.x, transform.position.y + offsetPositionY);
            }
        }
    }
}
