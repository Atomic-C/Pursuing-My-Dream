using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_x_Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.attachedRigidbody) return;
        other.attachedRigidbody.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    }
}
