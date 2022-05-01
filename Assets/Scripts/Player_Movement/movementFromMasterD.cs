using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Collider2D))]
public class movementFromMasterD : MonoBehaviour {

    [Header("Movement Settings")]
    public float moveSpeed = 2;
    public float jumpSpeed = 5;

    private Rigidbody2D _rb;
    private float _xInput, _jumpInput;

    private void Awake () {
        _rb = GetComponent<Rigidbody2D> ();
    }

    private void Update () {
        _xInput = Input.GetAxisRaw ("Horizontal");
        _jumpInput = Input.GetAxisRaw ("Jump");
    }

    private void FixedUpdate () {
        float vX = _xInput * moveSpeed;
        float vY = _rb.velocity.y;

        if(_jumpInput.Equals(1))
            vY = jumpSpeed;

        _rb.velocity = new Vector2(vX, vY);
    }
}