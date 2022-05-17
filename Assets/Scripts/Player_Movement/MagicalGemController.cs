using UnityEngine;

/// <summary>
/// Class responsible for controlling the shooting magical gem
/// </summary>
public class MagicalGemController : MonoBehaviour
{
    /// <summary>
    /// The player position
    /// </summary>    
    public Transform playerPosition;

    /// <summary>
    /// Vector3 used as an offset to the following movement
    /// </summary>
    private Vector3 followOffset = new Vector3(0,1,0);

    /// <summary>
    /// The types of bullets available
    /// </summary>
    public GameObject[] bullets;

    /// <summary>
    /// Statistics of the bullet: its strenght, speed and rate of fire
    /// </summary>
    public float bulletStrenght, bulletSpeed, rateOfFire;

    /// <summary>
    /// Float used to give the sensation of the gem going to above the player position so its not instant 
    /// </summary>
    public float smoothSpeed = 0.125f;

    private void FixedUpdate()
    {
        SmoothFollow.instance.FollowObject(transform,playerPosition,followOffset,smoothSpeed);
    }
}
