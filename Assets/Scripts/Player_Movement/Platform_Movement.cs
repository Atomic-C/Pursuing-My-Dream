using UnityEngine;

public class Platform_Movement : MonoBehaviour {

    [Header("Movement Settings")]
    // Which layer will the RayCast collide with
    [SerializeField] private LayerMask platformLayerMask;

    private Rigidbody2D rigidBody2d;
    private CircleCollider2D circleCollider2d;

    // RaycastHit2d used in the GroundCheck function
    private RaycastHit2D raycastHitDown;
    private RaycastHit2D raycastHitUp;

    // GameObjects positioned on the player foot and head and used in the GroundCheck function
    public Transform playerFootPosition;
    public Transform playerHeadPosition;

    // ENUM to change which type of RayCast2d to use
    public CastType WhichCastToUse;

    // Adjust the length of the RayCast2d (going directly down / up from the player), depending on the game platforms colliders
    public float rayCastOffSet = .1f;

    public float jumpSpeed = 8f;
    public float movementSpeed = 5f;

    // Float that holds the physics calculation of the player x / y axis movement
    private float xInput, yInput;

    // Bool used to check if the player is grounded
    private bool isGrounded;

    // Coyote time variables
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    // Jump buffer variable
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    // Player animator
    private Animator animator;

    // Enable debugging features
    public bool enableDebug;
    public bool castHitLog;

    // Float used as a timer for the CastHit function
    private float timer;

    // Vector3 used to hold the last grounded position
    public Vector3 lastPosition;

    // Initialize variables
    void Start()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
        circleCollider2d = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Inputs here
    void Update()
    {
        GroundCheck(WhichCastToUse);
        CheckInputs();
    }

    // Physics here
    private void FixedUpdate()
    {
        CalculatePhysics();
    }

    // Function that checks all inputs, used in the Update event
    private void CheckInputs()
    {
        // Refactoring following the best practices, as seen on the movement script provided by Master D
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Jump");
        
        // Implementation of jump buffering
        if (yInput.Equals(1))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Implementation of coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        Flip();

        // This is used to allow the player to jump down from a platform (only possible in platforms with the Platform Effector)
        if (raycastHitDown.collider != null)
        {
            if (raycastHitDown.collider.CompareTag("PlatformEffector") && Input.GetKey(KeyCode.S))
            {
                raycastHitDown.collider.isTrigger = true;
            }
        }
        // Movement using the game object transform (no physics manipulation, right?)
        //transform.Translate(new Vector2(horizontalSpeed * Time.deltaTime, 0));
    }

    // Function that flips the character sprite, simulating moving to the left / right
    private void Flip()
    {
        if (xInput > 0)
        {
            this.transform.localScale = new Vector2(1, transform.localScale.y);
        } else if (xInput < 0)
        {
            this.transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    // Function that controls all physics calculations, used in the FixedUpdate event
    private void CalculatePhysics()
    {
        // Refactoring following the best practices, as seen on the movement script provided by Master D
        float horizontalSpeed = xInput * movementSpeed;
        float verticalSpeed = rigidBody2d.velocity.y;

        // Coyote time and jump buffer in action
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            verticalSpeed = jumpSpeed;

            jumpBufferCounter = 0f;
            // Play the jumping sound
            AudioManager.instance.PlaySound("Jump");
        }

        if (yInput.Equals(0))
            coyoteTimeCounter = 0f;


        // Slippery movement
        //rigidBody2d.AddForce(new Vector2(horizontalSpeed, 0), ForceMode2D.Force);

        // Movement using physics (needed to determine the character animation)
        rigidBody2d.velocity = new Vector2(horizontalSpeed, verticalSpeed);

        // Trying to use this in FixedUpdate caused some problems
        // Not every space key press was being detected (FixedUpdate has a slower pace in comparison to Update, i think)
        // Thus, making the jumping to fail sometimes
        // If the character is on the ground and space is pressed
        //if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        //{
        // Two ways for simulate a jump, i guess?
        //rigidBody2d.velocity = Vector2.up * jumpSpeed;
        //rigidBody2d.AddForce(new Vector2(rigidBody2d.velocity.x, jumpSpeed), ForceMode2D.Impulse);

        // Play the jumping sound
        //AudioManager.instance.PlayAudio(jumpSound);
        //}

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
    // or if it is jumping through a platform effector with one way property enabled
    // Has built-in debug features
    private void GroundCheck(CastType WhichCastType)
    {
        isGrounded = false;

        switch (WhichCastType)
        {
            // Ray cast
            case CastType.RAY:
                // Ground check not using parenting
                //raycastHitDown = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.down, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Ground check using parenting
                raycastHitDown = Physics2D.Raycast(playerFootPosition.transform.position, Vector2.down, rayCastOffSet, platformLayerMask);

                // Platform effector check not using parenting
                //raycastHitUp = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.up, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Platform effector check using parenting
                raycastHitUp = Physics2D.Raycast(playerHeadPosition.transform.position, Vector2.up, rayCastOffSet, platformLayerMask);

                if (enableDebug)
                {
                    // Ground check debug not using parenting
                    //Debug.DrawRay(circleCollider2d.bounds.center, Vector2.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitDown, true));

                    // Ground check debug using parenting
                    Debug.DrawRay(playerFootPosition.transform.position, Vector2.down * rayCastOffSet, CastHit(raycastHitDown, true));

                    // Platform effector check debug not using parenting
                    //Debug.DrawRay(circleCollider2d.bounds.center, Vector2.up * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitUp, false));

                    // Platform effector check debug using parenting
                    Debug.DrawRay(playerHeadPosition.transform.position, Vector2.up * rayCastOffSet, CastHit(raycastHitUp, false));
                }
                break;
            // Box cast
            case CastType.BOX:
                // Ground check not using parenting
                //raycastHitDown = Physics2D.BoxCast(circleCollider2d.bounds.center, circleCollider2d.bounds.extents, 0f, Vector2.down, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Ground check using parenting
                raycastHitDown = Physics2D.BoxCast(playerFootPosition.transform.position, circleCollider2d.bounds.extents, 0f, Vector2.down, rayCastOffSet, platformLayerMask);

                // Plaftorm effector check not using parenting
                //raycastHitUp = Physics2D.BoxCast(circleCollider2d.bounds.center, circleCollider2d.bounds.extents, 0f, Vector2.up, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Platform effector check using parenting
                raycastHitUp = Physics2D.BoxCast(playerHeadPosition.transform.position, circleCollider2d.bounds.extents, 0f, Vector2.up, rayCastOffSet, platformLayerMask);

                if (enableDebug)
                {
                    // Ground check debug not using parenting
                    /*Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitDown, true));
                    Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitDown, true));
                    Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, circleCollider2d.bounds.extents.y + rayCastOffSet), Vector2.right * (circleCollider2d.bounds.size), CastHit(raycastHitDown, true));*/

                    // Ground check debug using parenting
                    Debug.DrawRay(playerFootPosition.transform.position + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.down * rayCastOffSet, CastHit(raycastHitDown, true));
                    Debug.DrawRay(playerFootPosition.transform.position - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.down * rayCastOffSet, CastHit(raycastHitDown, true));
                    Debug.DrawRay(playerFootPosition.transform.position - new Vector3(circleCollider2d.bounds.extents.x, rayCastOffSet), Vector2.right * (circleCollider2d.bounds.size), CastHit(raycastHitDown, true));

                    // Platform effector check debug not using parenting
                    /*Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.up * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitUp, false));
                    Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.up * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitUp, false));
                    Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, circleCollider2d.bounds.extents.y + rayCastOffSet), Vector2.left * (circleCollider2d.bounds.size), CastHit(raycastHitUp, false));*/

                    // Platfrom effector check debug using parenting
                    Debug.DrawRay(playerHeadPosition.transform.position - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.up * rayCastOffSet, CastHit(raycastHitUp, false));
                    Debug.DrawRay(playerHeadPosition.transform.position + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.up * rayCastOffSet, CastHit(raycastHitUp, false));
                    Debug.DrawRay(playerHeadPosition.transform.position + new Vector3(circleCollider2d.bounds.extents.x, rayCastOffSet), Vector2.left * (circleCollider2d.bounds.size), CastHit(raycastHitUp, false));
                }
                break;
            // Circle cast
            // The visual representation of the circle cast is being drawn on the OnDrawGizmosSelected function
            default:
                // Ground check not using parenting
                //raycastHitDown = Physics2D.CircleCast(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.min.y), rayCastOffSet, Vector2.down, rayCastOffSet, platformLayerMask);

                // Ground check using parenting
                raycastHitDown = Physics2D.CircleCast(playerFootPosition.transform.position, rayCastOffSet, Vector2.down, rayCastOffSet, platformLayerMask);

                // Platform effector check not using parenting
                //raycastHitUp = Physics2D.CircleCast(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.max.y), rayCastOffSet, Vector2.up, rayCastOffSet, platformLayerMask);

                // Platform effector check using parenting
                raycastHitUp = Physics2D.CircleCast(playerHeadPosition.transform.position, rayCastOffSet, Vector2.up, rayCastOffSet, platformLayerMask);
                break;
        }

        if(castHitLog) 
            CastHitLog();

        // If the player jumped through a platform effector, deactivate its collider to prevent jumping animation bugs
        // Since the isGrounded bool is changed below, if the rayCast projected down from the player hits a platform with the respective layer mask
        // which triggers the "on ground" animation (iddle or moving), by deactivating its collider, it is possible to prevent the bug where the
        // animation quickly transition from jumping > iddle > jumping
        // These platforms have a script on them, responsible for reseting their properties to default
        if (raycastHitUp.collider != null && raycastHitUp.collider.CompareTag("PlatformEffector"))
        {
            raycastHitUp.collider.enabled = false;
        }

        // isGrounded bool receives true or false, if the cast hit any collider with the platform layer mask
        isGrounded = raycastHitDown.collider != null;

        // If the player is grounded, cache this position (used to reset the player position later, if it falls from the level platform)
        if (isGrounded)
        {
            lastPosition = this.transform.position;
            // Work-around for a problem found: when the player is near the edge of a platform, it will slowly slip until it falls. When caching this last position
            // that causes the player to slip, this can lead to other falls for the distracted human behind the keyboard. Which can be annoying...
            // To fix this, the last position of the X axis is cached with a little calculation: it the xInput is negative (the player is moving to the left) then the
            // value cached will be a little to the right, avoiding the edge slip problem. The same goes for when the player is moving to the right (cached a little to the left)
            lastPosition.x = xInput < 0 ? lastPosition.x + 0.5f : lastPosition.x - 0.5f;
        }

        // Set the animator Jump bool accordingly
        // Is grounded = true, and vice versa
        animator.SetBool("Jump", !isGrounded);

    }

    // Function used for debugging
    // The bool is just used for the Debug.Log part, for printing into the console which collider the cast hit
    private Color CastHit(RaycastHit2D castHit, bool castDown)
    {
        // Green for hitting any collider, red for no collider hit
        Color rayColor;
        if (castHit.collider != null)
        {
        rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        return rayColor;
    }

    private void CastHitLog()
    {
        // Check which collider is being hit and print in the console (time based)
        timer = timer <= 0 ? 2f : timer;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Debug.Log("Cast up hit " + raycastHitUp.collider);
            Debug.Log("Cast down hit " + raycastHitDown.collider);
        }
    }

    // ENUM that store the cast types
    // Can be used to implement the other ones (Line, Capsule and Overlap variants)
    public enum CastType {
        RAY,
        BOX,
        CIRCLE
    }

    // Since i didnt found a solution to debug the circle cast, thats the best i could do. Using Gizmos.DrawSphere to have a visual represetation
    // of the circle cast. With or without the use of object parenting
    void OnDrawGizmosSelected()
    {
        if (enableDebug && WhichCastToUse == CastType.CIRCLE)
        {
            // Ground check color
            Gizmos.color = CastHit(raycastHitDown, true);

            // Ground check debug not using parenting
            //Gizmos.DrawWireSphere(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.min.y) , rayCastOffSet);

            // Ground check debug using parenting
            Gizmos.DrawWireSphere(playerFootPosition.transform.position, rayCastOffSet);

            // Platform effector check color
            Gizmos.color = CastHit(raycastHitUp, false);

            // Platform effector check debug not using parenting
            //Gizmos.DrawWireSphere(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.max.y), rayCastOffSet);

            // Platform effector check debug using parenting
            Gizmos.DrawWireSphere(playerHeadPosition.transform.position, rayCastOffSet);
        }

    }
}


