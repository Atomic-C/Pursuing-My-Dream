using UnityEngine;

/// <summary>
/// Simple script to animate the background
/// </summary>
public class Scene_Animation : MonoBehaviour
{
    /// <summary>
    /// Animation speed
    /// </summary>
    public float speed = 0.1f;

    /// <summary>
    /// The game object renderer
    /// </summary>
    private Renderer rend;

    /// <summary>
    /// Enum used to determine which renderer this game object is using
    /// </summary>
    public RendererType rendererType;

    // Cache the sprite renderer component
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    /// <summary>
    /// Animate the sprite / mesh renderer depending on the renderer type enum
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        switch (rendererType)
        {
            // If its a sprite renderer, animate differently...
            case RendererType.SPRITE:
                Vector3 offsetV3 = new Vector3(speed * Time.deltaTime, 0);
                rend.transform.position += offsetV3;
                break;
            // ...than a mesh renderer
            case RendererType.MESH:
                Vector2 offsetV2 = new Vector2(speed * Time.deltaTime, 0);
                rend.material.mainTextureOffset += offsetV2;
                break;
        }
    }

    /// <summary>
    /// The renderer enum
    /// </summary>
    public enum RendererType
    {
        SPRITE,
        MESH
    }
}
