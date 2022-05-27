using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform objectToFollow;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(objectToFollow.position.x, objectToFollow.position.y, 0) * Time.deltaTime;        
    }
}
