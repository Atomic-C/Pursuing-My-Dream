using UnityEngine;

// Simple script to animate the background
public class Scene_Animation : MonoBehaviour
{
    // Animation speed
    public float speed = 0.1f;
    // The game object renderer
    public Renderer rend;
    // Enum used to determine which renderer this game object is using
    public RendererType rendererType;

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

    // The renderer enum
    public enum RendererType
    {
        SPRITE,
        MESH
    }
}
