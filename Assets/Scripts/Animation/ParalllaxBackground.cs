using UnityEngine;

public class ParalllaxBackground : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private bool infiniteHorizontal, infiniteVertical, textureIsScaled, useParallax;
    [SerializeField] private float autoScrollSpeed;
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
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
