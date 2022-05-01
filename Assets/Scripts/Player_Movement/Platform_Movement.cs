using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Movement : MonoBehaviour {

    [Header("Movement Settings")]
    // Which layer will the RayCast will collide with
    [SerializeField] private LayerMask platformLayerMask;
    private Rigidbody2D rigidBody2d;
    private CircleCollider2D circleCollider2d;
    // Had to store the raycast2d, to be used in the OnDrawGizmosSelected
    private RaycastHit2D raycastHit;
    // Used to change which type of RayCast2d we are using
    public CastType WhichCastToUse;
    // Adjust the length of the RayCast2d (going directly down from the player), depending on the game platforms colliders
    public float rayCastOffSet = .1f;
    public float jumpSpeed = 8f;
    public float movementSpeed = 5f;
    // Float that holds the physics calculation of the player x axis movement
    private float horizontalSpeed;
    // Bool used to check if the character is grounded
    private bool isGrounded;
    // Character sounds
    public AudioClip jumpSound;
    // Character animator
    private Animator animator;

    void Start()
    {
        rigidBody2d = transform.GetComponent<Rigidbody2D>();
        circleCollider2d = transform.GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck(WhichCastToUse);
        CheckInputs();
    }

    private void FixedUpdate()
    {
        CalculatePhysics();
    }

    // Function that checks all inputs, used in the Update event
    private void CheckInputs()
    {
        horizontalSpeed = Input.GetAxisRaw("Horizontal") * movementSpeed;
        Flip();

        // Trying to use this in FixedUpdate caused some problems
        // Not every space key press was being detected (FixedUpdate has a slower pace in comparison to Update)
        // Thus, making the jumping to fail sometimes
        // If the character is on the ground and space is pressed
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Two ways for simulate a jump, i guess?
            //rigidBody2d.velocity = Vector2.up * jumpSpeed;
            rigidBody2d.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);

            //  Character just jumped, so set the animator bool Jump to true
            animator.SetBool("Jump", true);

            // Play the jumping sound
            AudioManager.instance.PlayAudio(jumpSound);
        }
        
        // Movement using the game object transform (no physics manipulation, right?)
        //transform.Translate(new Vector2(horizontalSpeed * Time.deltaTime, 0));

    }

    // Function that flips the character sprite, simulating moving to the left / right
    void Flip()
    {
        if (horizontalSpeed > 0)
        {
            this.transform.localScale = new Vector2(1, transform.localScale.y);
        } else if (horizontalSpeed < 0)
        {
            this.transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    // Function that controls all physics calculations, used in the FixedUpdate event
    private void CalculatePhysics()
    {

        // Slippery movement
        //rigidBody2d.AddForce(new Vector2(horizontalSpeed, 0), ForceMode2D.Force);

        // Movement using physics (needed to determine the character animation
        rigidBody2d.velocity = new Vector2(horizontalSpeed, rigidBody2d.velocity.y);

        // Set the animator yVelocity Float where:
        // > 0 = jumping
        // < 0 = falling
        animator.SetFloat("yVelocity", rigidBody2d.velocity.y);

        // Ternary that set the xVelocity to the desired ammount, to trigger the corresponding animation
        // 1 means moving animation
        // 0 means iddle animation
        // Using the rigid body y velocity so that the moving animations is only played if the character is "not in the air", aka jumping or falling
        animator.SetFloat("xVelocity", rigidBody2d.velocity.x != 0 && rigidBody2d.velocity.y == 0 ? 1f : 0f);

        // Little animation to crouch, just for the giggles
        animator.SetBool("Crouch", Input.GetKey(KeyCode.S) && rigidBody2d.velocity.x == 0);

    }

    // Function using the selected cast to detect if the character is in touch with the ground
    // Has built-in debug features
    private void GroundCheck(CastType WhichCastType)
    {
        isGrounded = false;

        switch (WhichCastType)
        {
            // Ray cast
            case CastType.RAY:
                raycastHit = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.down, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);
                Debug.DrawRay(circleCollider2d.bounds.center, Vector2.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHit));
                break;
            // Box cast
            case CastType.BOX:
                raycastHit = Physics2D.BoxCast(circleCollider2d.bounds.center, circleCollider2d.bounds.size, 0f, Vector2.down, rayCastOffSet, platformLayerMask);
                Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector2.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHit));
                Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector2.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHit));
                Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, circleCollider2d.bounds.extents.y + rayCastOffSet), Vector2.right * (circleCollider2d.bounds.size), CastHit(raycastHit));
                break;
            // Circle cast
            // Seems to work, but i had trouble to draw the circle for debug...
            default:
                raycastHit = Physics2D.CircleCast(new Vector2(circleCollider2d.transform.position.x, circleCollider2d.bounds.min.y), rayCastOffSet, Vector2.down, rayCastOffSet, platformLayerMask);
                break;
        }

        // isGrounded bool receives true or false, if the cast hit any collider with the platform layer mask
        isGrounded = raycastHit.collider != null;

        // Set the animator Jump bool accordingly
        // Is grounded = true, and vice versa
        animator.SetBool("Jump", !isGrounded);
         
    }

    // Function used for debugging
    private Color CastHit(RaycastHit2D castHit)
    {
        // Green for hitting any collider, red for no collider hit
        Color rayColor;
        if (castHit.collider != null)
        {
            rayColor = Color.green;
        } else
        {
            rayColor = Color.red;
        }

        // Remove the comment below to check which collider is being hit, in the console
        //Debug.Log(castHit.collider);
        return rayColor;
    }

    // ENUM that store the cast types
    // Can implement the other ones (Line, Capsule and Overlap variants)
    public enum CastType {
        RAY,
        BOX,
        CIRCLE
    }

    // Since i didnt found a solution to debug the circle cast, thats the best i could do. Using Gizmos.DrawSphere to have a visual represetation
    // of the circle cast. And i'm not sure the calculations are correct. I'm like, 80% sure, maybe?
    void OnDrawGizmosSelected()
    {
        //Gizmos.color = CastHit(raycastHit);
        //Gizmos.DrawSphere(new Vector2(circleCollider2d.transform.position.x, circleCollider2d.bounds.min.y), rayCastOffSet);
    }
}


