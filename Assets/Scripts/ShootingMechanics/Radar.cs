using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utilitary class used to detect all objects by their collider 2D and a respective layer using a trigger collider as detection radius
/// </summary>
public class Radar : MonoBehaviour
{
    /// <summary>
    /// Collider 2D used as a trigger to detect objects
    /// </summary>
    public Collider2D triggerCollider;

    /// <summary>
    /// Which layer this detection will affect
    /// </summary>
    public LayerMask layerMask;

    /// <summary>
    /// The position of the player object
    /// </summary>
    public Transform playerPosition;

    /// <summary>
    /// Instance of the radar
    /// </summary>
    public static Radar instance = null;

    /// <summary>
    /// Singleton design pattern to guarantee only one instance of this exists
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets all colliders 2D with the respective layer associated with them
    /// </summary>
    /// <returns>List of colliders 2D</returns>
    private List<Collider2D> GetLayerMaskColliders()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMask);

        triggerCollider.OverlapCollider(contactFilter2D, colliders);

        return colliders;
    }

    /// <summary>
    /// Get the closest collider 2D position from a list of colliders, from the player position
    /// </summary>
    /// <param name="collidersList">List of colliders 2D</param>
    /// <returns>The position of the closest collider</returns>
    private Transform GetClosestPosition(List<Collider2D> collidersList)
    {
        Transform closestFound = null;

        if(collidersList.Count > 0)
        {
            // Optimized code learned through research
            // By doing the square root operation manually, the code becomes lighter and has the same result. The order of closest object remains the same.
            float distance = Mathf.Infinity;
            closestFound = collidersList[0].transform;

            foreach (Collider2D potentialTarget in collidersList)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - transform.position;
                float sqrDirectionToTarget = directionToTarget.sqrMagnitude;
                if(sqrDirectionToTarget < distance)
                {
                    distance = sqrDirectionToTarget;
                    closestFound = potentialTarget.transform;
                }
            }

            // Original code, not so optimized according to my research
            // Vector3.Distance execute expensive square root operation, affecting performance
            /*float distance = 0f;
            float closestDistance = Vector3.Distance(playerPosition.position, list[0].transform.position);
            closestFound = list[0].transform;

            for (int i = 0; i < list.Count; i++)
            {
                distance = Vector3.Distance(playerPosition.position, list[i].transform.position);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFound = list[i].transform;
                }
            }*/
            return closestFound;
        } else
            return closestFound;
    }

    /// <summary>
    /// Get the target position
    /// </summary>
    /// <returns>The target position</returns>
    public Transform TargetPosition()
    {
        return GetClosestPosition(GetLayerMaskColliders());
    }
}
