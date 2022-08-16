using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Platform_Movement : MonoBehaviour 
{
    [Header("Movement Settings")]
    /// <summary>
    /// Bool that determines if the player is dead, disabling all controls
    /// </summary>
    public bool isDead;

    /// <summary>
    /// Bool that determines if the player took damage, disabling all controls (stun effect), if applicable
    /// </summary>
    public bool isHit;
    
    /// <summary>
    /// Bool used to check if the player is grounded
    /// </summary>
    public bool isGrounded;

    /// <summary>
    /// GameObject positioned on the player foot, used in the GroundCheck function
    /// </summary>
    public Transform playerFootPosition;

    /// <summary>
    /// GameObject positioned on the player head, used in the GroundCheck function
    /// </summary>
    public Transform playerHeadPosition;

    /// <summary>
    /// GameObject used as a target for the camera. This way, its possible to make the camera follow this object, not necessarilly using the player position 
    /// and thus, controlling dynamically the camera (cutscenes for example, to shift the player attention to something, is easily doable by just manipulating
    /// this object position
    /// </summary>
    public Transform mainCameraTarget;

    /// <summary>
    /// ENUM to change which type of RayCast2d to use
    /// </summary>
    public CastType WhichCastToUse;

    /// <summary>
    /// Adjust the length of the RayCast2d (going directly down / up from the player), depending on the game platforms colliders
    /// </summary>
    public float rayCastOffSet = .1f;

    /// <summary>
    /// Player jump speed
    /// </summary>
    public float jumpSpeed = 8f;

    /// <summary>
    /// Player movement speed
    /// </summary>
    public float movementSpeed = 5f;

    /// <summary>
    /// Vector3 used to hold the last grounded position (used in the ResetPlayerPosition script)
    /// </summary>
    public Vector3 lastPosition;

    /// <summary>
    /// Enable debugging features
    /// </summary>
    public bool enableDebug;

    /// <summary>
    /// Enable console log of the colliders being hit with the ground check function
    /// </summary>
    public bool castHitLog;

    /// <summary>
    /// Which layer will the RayCast collide with
    /// </summary>
    [SerializeField] private LayerMask _platformLayerMask;

    /// <summary>
    /// Jump buffer variables
    /// </summary>
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;

    /// <summary>
    /// Coyote time variables
    /// </summary>
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCounter;

    /// <summary>
    /// The player rigidbody 2d
    /// </summary>
    private Rigidbody2D _rigidBody2d;

    /// <summary>
    /// The player collider 2d
    /// </summary>
    private CircleCollider2D _circleCollider2d;

    /// <summary>
    /// RaycastHit2d used in the GroundCheck function to detect platforms bellow the player
    /// </summary>
    private RaycastHit2D _raycastHitDown;

    /// <summary>
    /// RaycastHit2d used in the GroundCheck function to detect platforms above the player
    /// </summary>
    private RaycastHit2D _raycastHitUp;

    /// <summary>
    /// Float that holds the physics calculation of the player x axis movement
    /// </summary>
    private float _xInput;

    /// <summary>
    /// Player animator
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// Float used as a timer for the CastHit function
    /// </summary>
    private float _timer;

    /// <summary>
    /// Initialize variables
    /// </summary>
    void Start()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _circleCollider2d = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Check if the player is grounded / player inputs
    /// </summary>
    void Update()
    {
        // Disable all controls if the player died / took damage (stun effect)
        if (!isDead)
            if (!isHit)
            {
                GroundCheck(WhichCastToUse);
                CheckInputs();
            }
    }

    /// <summary>
    /// Physics calculations
    /// </summary>
    private void FixedUpdate()
    {
        // Disable all controls if the player died / took damage (stun effect)
        if (!isDead)
            if(!isHit)
                CalculatePhysics();
    }

    /// <summary>
    /// Function that checks all inputs, used in the Update event
    /// </summary>
    private void CheckInputs()
    {
        // Refactoring following the best practices, as seen on the movement script provided by Master D
        _xInput = Input.GetAxisRaw("Horizontal");
        
        // Implementation of jump buffering
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        // Implementation of coyote time
        if (isGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        Flip();

        // This is used to allow the player to jump down from a platform (only possible in platforms with the Platform Effector)
        if (_raycastHitDown.collider != null)
        {
            if (_raycastHitDown.collider.CompareTag("PlatformEffector") && Input.GetKey(KeyCode.S))
            {
                _raycastHitDown.collider.isTrigger = true;
            }
        }
        // Movement using the game object transform (no physics manipulation, right?)
        //transform.Translate(new Vector2(horizontalSpeed * Time.deltaTime, 0));
    }

    /// <summary>
    /// Function that flips the character sprite, simulating moving to the left / right
    /// </summary>
    private void Flip()
    {
        if (_xInput > 0)
        {
            this.transform.localScale = new Vector2(1, transform.localScale.y);
        } else if (_xInput < 0)
        {
            this.transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    /// <summary>
    /// Function that controls all physics calculations, used in the FixedUpdate event
    /// </summary>
    private void CalculatePhysics()
    {
        // Refactoring following the best practices, as seen on the movement script provided by Master D
        float horizontalSpeed = _xInput * movementSpeed;
        float verticalSpeed = _rigidBody2d.velocity.y;

        // Coyote time and jump buffer in action
        if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f)
        {
            verticalSpeed = jumpSpeed;

            _jumpBufferCounter = 0f;
            // Play the jumping sound
            AudioManager.instance.PlaySound("Jump", gameObject.transform.position);
        }

        // Reset the coyote timer when the spacebar is released
        if (Input.GetKeyUp(KeyCode.Space))
            _coyoteTimeCounter = 0f;


        // Slippery movement
        //rigidBody2d.AddForce(new Vector2(horizontalSpeed, 0), ForceMode2D.Force);

        // Movement using physics (needed to determine the character animation)
        _rigidBody2d.velocity = new Vector2(horizontalSpeed, verticalSpeed);

        /*Trying to use this in FixedUpdate caused some problems
        Not every space key press was being detected (FixedUpdate has a slower pace in comparison to Update, i think)
        Thus, making the jumping to fail sometimes
        If the character is on the ground and space is pressed
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
         Two ways for simulate a jump, i guess?
        rigidBody2d.velocity = Vector2.up * jumpSpeed;
        rigidBody2d.AddForce(new Vector2(rigidBody2d.velocity.x, jumpSpeed), ForceMode2D.Impulse);

        Play the jumping sound
        AudioManager.instance.PlayAudio(jumpSound);
        }*/

        // Set the animator yVelocity Float where:
        // > 0 = jumping
        // < 0 = falling
        _animator.SetFloat("yVelocity", _rigidBody2d.velocity.y);

        // Ternary that set the xVelocity to the desired ammount, to trigger the corresponding animation
        // 1 means moving animation
        // 0 means iddle animation
        // Using the isGrounded bool so that the moving animations is only played if the character is "not in the air", aka jumping or falling
        _animator.SetFloat("xVelocity", _rigidBody2d.velocity.x != 0 && isGrounded ? 1f : 0f);

        // Little animation to crouch, just for the giggles
        _animator.SetBool("Crouch", Input.GetKey(KeyCode.S) && _rigidBody2d.velocity.x == 0);        
}

    /// <summary>
    /// Function using the selected cast to detect if the character is in touch with the ground
    /// or if it is jumping through a platform effector with one way property enabled
    /// Has built-in debug features
    /// Using parenting (having two game objects, one at the player head, other at the foot) to check collisions or not (manually calculating the cast position / distance)
    /// </summary>
    /// <param name="WhichCastType">Cast type being used (raycast, circlecast or boxcast)</param>
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
                _raycastHitDown = Physics2D.Raycast(playerFootPosition.transform.position, Vector2.down, rayCastOffSet, _platformLayerMask);

                // Platform effector check not using parenting
                //raycastHitUp = Physics2D.Raycast(circleCollider2d.bounds.center, Vector2.up, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Platform effector check using parenting
                _raycastHitUp = Physics2D.Raycast(playerHeadPosition.transform.position, Vector2.up, rayCastOffSet, _platformLayerMask);

                if (enableDebug)
                {
                    // Ground check debug not using parenting
                    //Debug.DrawRay(circleCollider2d.bounds.center, Vector2.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitDown));

                    // Ground check debug using parenting
                    Debug.DrawRay(playerFootPosition.transform.position, Vector2.down * rayCastOffSet, CastHit(_raycastHitDown));

                    // Platform effector check debug not using parenting
                    //Debug.DrawRay(circleCollider2d.bounds.center, Vector2.up * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitUp));

                    // Platform effector check debug using parenting
                    Debug.DrawRay(playerHeadPosition.transform.position, Vector2.up * rayCastOffSet, CastHit(_raycastHitUp));
                }
                break;
            // Box cast
            case CastType.BOX:
                // Ground check not using parenting
                //raycastHitDown = Physics2D.BoxCast(circleCollider2d.bounds.center, circleCollider2d.bounds.extents, 0f, Vector2.down, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Ground check using parenting
                _raycastHitDown = Physics2D.BoxCast(playerFootPosition.transform.position, _circleCollider2d.bounds.extents, 0f, Vector2.down, rayCastOffSet, _platformLayerMask);

                // Plaftorm effector check not using parenting
                //raycastHitUp = Physics2D.BoxCast(circleCollider2d.bounds.center, circleCollider2d.bounds.extents, 0f, Vector2.up, circleCollider2d.bounds.extents.y + rayCastOffSet, platformLayerMask);

                // Platform effector check using parenting
                _raycastHitUp = Physics2D.BoxCast(playerHeadPosition.transform.position, _circleCollider2d.bounds.extents, 0f, Vector2.up, rayCastOffSet, _platformLayerMask);

                if (enableDebug)
                {
                    // Ground check debug not using parenting
                    /*Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitDown, true));
                    Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.down * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitDown, true));
                    Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, circleCollider2d.bounds.extents.y + rayCastOffSet), Vector2.right * (circleCollider2d.bounds.size), CastHit(raycastHitDown, true));*/

                    // Ground check debug using parenting
                    Debug.DrawRay(playerFootPosition.transform.position + new Vector3(_circleCollider2d.bounds.extents.x, 0), Vector3.down * rayCastOffSet, CastHit(_raycastHitDown   ));
                    Debug.DrawRay(playerFootPosition.transform.position - new Vector3(_circleCollider2d.bounds.extents.x, 0), Vector3.down * rayCastOffSet, CastHit(_raycastHitDown));
                    Debug.DrawRay(playerFootPosition.transform.position - new Vector3(_circleCollider2d.bounds.extents.x, rayCastOffSet), Vector2.right * (_circleCollider2d.bounds.size), CastHit(_raycastHitDown));

                    // Platform effector check debug not using parenting
                    /*Debug.DrawRay(circleCollider2d.bounds.center - new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.up * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitUp, false));
                    Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, 0), Vector3.up * (circleCollider2d.bounds.extents.y + rayCastOffSet), CastHit(raycastHitUp, false));
                    Debug.DrawRay(circleCollider2d.bounds.center + new Vector3(circleCollider2d.bounds.extents.x, circleCollider2d.bounds.extents.y + rayCastOffSet), Vector2.left * (circleCollider2d.bounds.size), CastHit(raycastHitUp, false));*/

                    // Platfrom effector check debug using parenting
                    Debug.DrawRay(playerHeadPosition.transform.position - new Vector3(_circleCollider2d.bounds.extents.x, 0), Vector3.up * rayCastOffSet, CastHit(_raycastHitUp));
                    Debug.DrawRay(playerHeadPosition.transform.position + new Vector3(_circleCollider2d.bounds.extents.x, 0), Vector3.up * rayCastOffSet, CastHit(_raycastHitUp));
                    Debug.DrawRay(playerHeadPosition.transform.position + new Vector3(_circleCollider2d.bounds.extents.x, rayCastOffSet), Vector2.left * (_circleCollider2d.bounds.size), CastHit(_raycastHitUp));
                }
                break;
            // Circle cast
            // The visual representation of the circle cast is being drawn on the OnDrawGizmosSelected function
            default:
                // Ground check not using parenting
                //raycastHitDown = Physics2D.CircleCast(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.min.y), rayCastOffSet, Vector2.down, rayCastOffSet, platformLayerMask);

                // Ground check using parenting
                _raycastHitDown = Physics2D.CircleCast(playerFootPosition.transform.position, rayCastOffSet, Vector2.down, rayCastOffSet, _platformLayerMask);

                // Platform effector check not using parenting
                //raycastHitUp = Physics2D.CircleCast(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.max.y), rayCastOffSet, Vector2.up, rayCastOffSet, platformLayerMask);

                // Platform effector check using parenting
                _raycastHitUp = Physics2D.CircleCast(playerHeadPosition.transform.position, rayCastOffSet, Vector2.up, rayCastOffSet, _platformLayerMask);
                break;
        }

        if(castHitLog) 
            CastHitLog();

        // If the player jumped through a platform effector, deactivate its collider to prevent jumping animation bugs
        // Since the isGrounded bool is changed below, if the rayCast projected down from the player hits a platform with the respective layer mask
        // which triggers the "on ground" animation (iddle or moving), by deactivating its collider, it is possible to prevent the bug where the
        // animation quickly transition from jumping > iddle > jumping
        // These platforms have a script on them, responsible for reseting their properties to default
        if (_raycastHitUp.collider != null && _raycastHitUp.collider.CompareTag("PlatformEffector"))
        {
            _raycastHitUp.collider.enabled = false;
        }

        // isGrounded bool receives true or false, if the cast hit any collider with the platform layer mask
        isGrounded = _raycastHitDown.collider != null;

        // If the player is grounded, cache this position (used to reset the player position later, if it falls from the level platform)
        if (isGrounded)
        {
            lastPosition = this.transform.position;
            // Work-around for a problem found: when the player is near the edge of a platform, it will slowly slip until it falls. When caching this last position
            // that causes the player to slip, this can lead to other falls for the distracted human behind the keyboard. Which can be annoying...
            // To fix this, the last position of the X axis is cached with a little calculation: it the xInput is negative (the player is moving to the left) then the
            // value cached will be a little to the right, avoiding the edge slip problem. The same goes for when the player is moving to the right (cached a little to the left)
            lastPosition.x = _xInput < 0 ? lastPosition.x + 0.5f : lastPosition.x - 0.5f;
        }

        // Set the animator Jump bool accordingly
        // Is grounded = true, and vice versa
        _animator.SetBool("Jump", !isGrounded);

    }

    /// <summary>
    /// Function used for debugging
    /// The bool is just used for the Debug.Log part, for printing into the console which collider the cast hit
    /// </summary>
    /// <param name="castHit"></param>
    /// <returns></returns>
    private Color CastHit(RaycastHit2D castHit)
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

    /// <summary>
    /// Function that check which collider is being hit and print it in the console (time based)
    /// </summary>
    private void CastHitLog()
    {
        _timer = _timer <= 0 ? 2f : _timer;
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            Debug.Log("Cast up hit " + _raycastHitUp.collider);
            Debug.Log("Cast down hit " + _raycastHitDown.collider);
        }
    }

    /// <summary>
    /// ENUM that store the cast types
    /// Can be used to implement the other ones (Line, Capsule and Overlap variants)
    /// </summary>
    public enum CastType {
        RAY,
        BOX,
        CIRCLE
    }

    /// <summary>
    /// Function that create a visual represetation of the circle cast. With or without the use of object parenting
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (enableDebug && WhichCastToUse == CastType.CIRCLE)
        {
            // Ground check color
            Gizmos.color = CastHit(_raycastHitDown);

            // Ground check debug not using parenting
            //Gizmos.DrawWireSphere(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.min.y) , rayCastOffSet);

            // Ground check debug using parenting
            Gizmos.DrawWireSphere(playerFootPosition.transform.position, rayCastOffSet);

            // Platform effector check color
            Gizmos.color = CastHit(_raycastHitUp);

            // Platform effector check debug not using parenting
            //Gizmos.DrawWireSphere(new Vector3(circleCollider2d.transform.position.x, circleCollider2d.bounds.max.y), rayCastOffSet);

            // Platform effector check debug using parenting
            Gizmos.DrawWireSphere(playerHeadPosition.transform.position, rayCastOffSet);
        }

    }
}


