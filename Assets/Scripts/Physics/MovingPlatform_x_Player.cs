using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_x_Player : MonoBehaviour
{
    public class OnTriggerEnterMoveRigidbody : MonoBehaviour
    {
        Vector3 lastPosition, lastMove;

        void FixedUpdate()
        {
            lastMove = transform.position - lastPosition;
            lastPosition = transform.position;
        }

        void OnTriggerStay(Collider other)
        {
            if (!other.attachedRigidbody) return;
            other.attachedRigidbody.MovePosition(other.attachedRigidbody.position + lastMove);
        }
    }
}
