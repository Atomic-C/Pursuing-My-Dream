using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        //transform.LookAt(target); - o ângulo camera segue o player, útil em jogo 3d

    }

}
