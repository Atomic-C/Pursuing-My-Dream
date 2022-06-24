using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.U2D;

/// <summary>
/// Class responsible for controlling the shooting magical gem / shooting mechanic
/// </summary>
public class MagicalGemController : MonoBehaviour
{
    [Header("Object Pooling")]
    /// <summary>
    /// Bool used to change the behavior of the current bullet to make use of the object pooling technique
    /// </summary>
    [SerializeField] private bool usePool;

    /// <summary>
    /// Array to be used in the object pool constructor (to aid dynamicity)
    /// </summary>
    [SerializeField] private int[] bulletPoolSize, bulletPoolMaxSize;

    /// <summary>
    /// Object pool that hold all objects of the current bullet type
    /// </summary>
    private IObjectPool<Bullet> bulletPool;

    [Header("Objects Setup")]
    /// <summary>
    /// Crosshair prefab, to be instantiated in case there is no active crosshair
    /// </summary>
    public GameObject crossHairPrefab;

    /// <summary>
    /// Magical gem prefab, to be instantiated in case there is no active gem
    /// </summary>
    public GameObject magicalGemPrefab;

    /// <summary>
    /// Array of game objects that holds all the coloured bar representing each bullet type energy bar
    /// </summary>
    public GameObject[] energyBars;

    /// <summary>
    /// Original material applied to the energy bar sprites
    /// </summary>
    public Material originalBarMaterial;

    /// <summary>
    /// Material used in the not enough energy coroutine (flash the bar white)
    /// </summary>
    public Material flashBarMaterial;

    /// <summary>
    /// The types of bullets available
    /// </summary>
    public Bullet[] bullets;

    /// <summary>
    /// Array of sprites used in the crosshair object (each shoot type has an different crosshair color)
    /// </summary>
    public Sprite[] crosshairColors;

    /// <summary>
    /// Array of library assets available to the magical gem (several sprites sharing the same animation)
    /// </summary>
    public UnityEngine.U2D.Animation.SpriteLibraryAsset[] spriteLibraries;

    /// <summary>
    /// Reference to the player movement script
    /// </summary>    
    public Platform_Movement playerMovement;

    /// <summary>
    /// Magical gem object that is being used in the active scene
    /// </summary>
    private GameObject activeMagicalGem;

    /// <summary>
    /// Crosshair object that is being used in the active scene
    /// </summary>
    private GameObject activeCrossHairObject;

    /// <summary>
    /// Trigger circle collider 2D used to define the range limit of the current bullet (calculation: player range statistic plus the current bullet range statistic - this affects
    /// the radius of the circle collider 2D)
    /// </summary>
    private CircleCollider2D rangeArea;

    [Header("Variables Setup")]
    /// <summary>
    /// Float used to give the sensation of the gem going to above the player position so its not instant 
    /// </summary>
    public float smoothSpeed = 0.125f;

    /// <summary>
    /// Current bullet being used
    /// </summary>
    private int currentBullet;

    /// <summary>
    /// Statistics of the player: its shoot strenght, projectile speed, range and rate of fire, energy, maximum energy and energy regen
    /// </summary>
    public float shootStrenght, shootSpeed, shootRange, shootRateOfFire, energy, maxEnergy, energyRegen;

    /// <summary>
    /// Float that will be affected by the rate of fire calculation
    /// </summary>
    private float fireTimer;

    /// <summary>
    /// Bool used to show / hide a visual representation of the current shoot range
    /// </summary>
    public bool showRange;

    /// <summary>
    /// The magical gem animator
    /// </summary>
    private Animator gemAnimator;

    /// <summary>
    /// The sprite library being used by the active magical gem
    /// </summary>
    private UnityEngine.U2D.Animation.SpriteLibrary gemSpriteLibrary;

    // Automated Setup
    
    /// <summary>
    /// Vector3 used to hold and manipulate the energy bar sprite scale. Used in the energy consumption and regen mechanic
    /// </summary>
    private Vector3 energyBarScale;

    /// <summary>
    /// Bool used to avoid multiple energy bar flash / error sound playing, when there is no energy to use the current bullet alternate shoot
    /// </summary>
    private bool noEnergyRoutineIsRunning;

    /// <summary>
    /// Vector3 used as an offset to the following movement
    /// </summary>
    private Vector3 followOffset;

    /// <summary>
    /// Bool used to allow the next fire
    /// </summary>
    private bool canFire;

    /// <summary>
    /// The active crosshair sprite renderer
    /// </summary>
    private SpriteRenderer activeCrosshairSprite;

    /// <summary>
    /// Guarantee there is one crosshair / magical gem in the scene and setup the necessary variable with the current bullet in use
    /// </summary> 
    private void Start()
    {
        CheckCrosshair();
        CheckGem();
        SetupCurrentBullet(currentBullet);
    }

    /// <summary>
    /// Cache the necessary variable and initialize the object pool, if applicable
    /// </summary>
    private void Awake()
    {
        rangeArea = GetComponent<CircleCollider2D>();
        canFire = true;

        InitializeBulletPool();

        // The initial idea was to try and use an array of object pools, one for each bullet type. But i found its easier to use only one pool and reset it accordingly to the current
        // bullet being used
        /*bulletPool[currentBullet] = new ObjectPool<Bullet>(() =>
        {
            Bullet bullet = Instantiate(bullets[currentBullet]);
            bullet.pooledObject = true;
            bullet.gameObject.SetActive(false);
            return bullet;
        }, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, bulletPoolSize[currentBullet], bulletPoolMaxSize[currentBullet]);*/

        //bulletPool[currentBullet] = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, bulletPoolSize[currentBullet], bulletPoolMaxSize[currentBullet]);
    }

    /// <summary>
    /// Animate the magical gem, check if the player is shooting (mouse button inputs) and use the energy regen mechanic
    /// </summary>
    private void Update()
    {
        Animate();
        FireBullet();
        EnergyRegen();
    }

    /// <summary>
    /// Follow the player with a small delay (defined in the smoothSpeed variable)
    /// </summary>
    private void FixedUpdate()
    {
        SmoothFollow.instance.FollowObject(transform,playerMovement.transform,followOffset,smoothSpeed);
    }

    /// <summary>
    /// Call the ChangeBullet function if the player pressed the designated keys
    /// </summary>
    private void OnGUI()
    {
        ChangeBullet();
    }

    /// <summary>
    /// Debug feature of the current bullet range limit
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (showRange)
            Gizmos.DrawWireSphere(activeMagicalGem.transform.position, rangeArea.radius * rangeArea.transform.lossyScale.x  );
    }

    /// <summary>
    /// Animate the gem depending on the player:
    /// On the ground - keeps floating
    /// Not on the ground (jumping or falling) - keeps still on a fixed position
    /// </summary>
    // Little animation for the gem. The idea: the gem keeps floating above the player. The problem: when jumping, if the gem is still floating, can be a little confusing, aka, too
    // much movement. The solution: make the gem stay in a fixed position when jumping / falling, to avoid that sensation
    private void Animate()
    {
        // Get the isGrounded bool from the player movement script, which is used in its GroundCheck function
        bool isGrounded = playerMovement.isGrounded;
        // Changes the distance above the player, being closer when jumping (not really needed, just for the looks)
        followOffset = isGrounded ? new Vector3(0, 2, 0) : new Vector3(0, 1, 0);
        // Set the gem animator respective bool
        gemAnimator.SetBool("playerIsGrounded", isGrounded);
    }

    /// <summary>
    /// Function that controls the gem firing
    /// </summary>
    private void FireBullet()
    {
        if (canFire)
        {
            // Left mouse click
            if (Input.GetKey(KeyCode.Mouse0))
            {
                InitBullet(false);

                canFire = false;
            }
            // Right mouse click and the current bullet has an alternate shoot version
            else if (Input.GetKey(KeyCode.Mouse1) && bullets[currentBullet].hasAlternateShoot)
            {
                // If the player has the necessary energy to use the alternate shoot
                if (energy > bullets[currentBullet].energyCost)
                {
                    // If it is the spread shoot alternate version, uses a different rate of fire setting
                    fireTimer = bullets[currentBullet].shootType == Bullet.ShootType.SPREAD ? 
                                bullets[currentBullet].chainRateOfFire - shootRateOfFire : bullets[currentBullet].rateOfFire - shootRateOfFire;

                    InitBullet(true);

                    canFire = false;

                    ConsumeEnergy();
                }
                // No energy necessary
                else
                    StartCoroutine(NotEnoughEnergy());
            }
        } 
        // Deplete the current bullet cooldown before being able to shoot again
        else
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                canFire = true;
                fireTimer = bullets[currentBullet].rateOfFire - shootRateOfFire;
            }
    }   

    /// <summary>
    /// Instantiate the current bullet at the magical gem position
    /// </summary>
    /// <param name="alternateShoot">Bool that determines if an alternate shoot was fired</param>
    private void InitBullet(bool alternateShoot)
    {
        // Instantiate / get the bullet prefab (at the gem position) attached to the respective Bullet class array 
        Bullet bullet = usePool ? bulletPool.Get() : Instantiate(bullets[currentBullet], activeMagicalGem.transform.position, Quaternion.identity);

        // Spread shoot alternate version need to have its animator disabled to be able to use the alternate shoot sprite
        if(bullet.shootType == Bullet.ShootType.SPREAD && alternateShoot)
            bullet.animator.enabled = false;
        else if(bullet.shootType == Bullet.ShootType.SPREAD)
            bullet.animator.enabled = true;

        bullet.Init(KillBullet);

        // Using the guided bullet version with object pooling: instantiate all of the alternate shoot version projectiles to be reused
        if (bullet.shootType == Bullet.ShootType.GUIDED && usePool)
            bullet.InitializePooledMiniGuidedBullets();

        /*Make it ignore the player collider to avoid shooting itself
        >>> Decided to do this by changing the collision matrix instead
        Physics2D.IgnoreCollision(bullet.GetComponent<CircleCollider2D>(), playerObject.GetComponent<CircleCollider2D>(), true);
        Physics2D.IgnoreCollision(bullet.GetComponent<CircleCollider2D>(), bullets[currentBullet].gameObject.GetComponent<CircleCollider2D>(), true);*/

        // Get the crosshair position
        Vector3 direction = activeCrossHairObject.transform.position - bullet.transform.position;

        // Calls the Bullet script Shoot function, attached to this instantiated object
        bullet.Shoot(direction, shootSpeed, alternateShoot);
    }

    /// <summary>
    /// Function that will dictate which behavior the bullet will have, depending if it is using object pooling or not
    /// </summary>
    /// <param name="bullet">The current bullet</param>
    private void KillBullet(Bullet bullet)
    {
        if (usePool)
            bulletPool.Release(bullet);
        else
            Destroy(bullet.gameObject);
    }

    /// <summary>
    /// Alternate shoot used so consume the necessary energy
    /// </summary>
    private void ConsumeEnergy()
    {
        float energyConsumed = bullets[currentBullet].energyCost;
        energy -= energyConsumed;
        // Simulate the energy bar diminish effect by using its scale
        energyBars[currentBullet].transform.localScale = new Vector3(energy, energyBarScale.y, energyBarScale.z);
    }

    /// <summary>
    /// Guarantee there is one active magical gem in the current scene
    /// If there isnt, instantiate one at the position of the object this script is attached to
    /// </summary>
    private void CheckGem()
    {
        // Checking if there is a gem
        activeMagicalGem = GameObject.FindGameObjectWithTag("MagicalGem");
        // If not
        if (activeMagicalGem == null)
        {
            // Instantiate one and do the necessary setup for this script to work
            activeMagicalGem = Instantiate(magicalGemPrefab, transform.position, Quaternion.identity);
            activeMagicalGem.transform.SetParent(transform, true);
        }

        // Get the gem animator and set it to the respective bullet color
        gemSpriteLibrary = activeMagicalGem.GetComponent<UnityEngine.U2D.Animation.SpriteLibrary>();
        gemAnimator = activeMagicalGem.GetComponent<Animator>();

        rangeArea = activeMagicalGem.GetComponentInChildren<CircleCollider2D>();

    }

    /// <summary>
    /// Guarantee there is one active crosshair in the current scene
    /// If there isnt, instantiate one at the position of the object this script is attached to
    /// </summary>
    private void CheckCrosshair()
    {
        // Checking if there is a crosshair
        activeCrossHairObject = GameObject.FindGameObjectWithTag("Crosshair");
        // If not
        if (activeCrossHairObject == null)
        {
            // Instantiate one and do the necessary setup for this script to work
            activeCrossHairObject = Instantiate(crossHairPrefab, transform.position, Quaternion.identity);
        }

        activeCrosshairSprite = activeCrossHairObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Changes the current bullet type being used depending on the key pressed
    /// </summary>
    private void ChangeBullet()
    {
        // Get the current event
        Event keyPressed = Event.current;
        // Verify if a key was pressed
        bool isKey = keyPressed.isKey;

        if (isKey)
        {
            KeyCode keyCode = keyPressed.keyCode;
            switch (keyCode)
            {
                // Keypad 1 for the default bullet
                case KeyCode.Keypad1:
                    SetupCurrentBullet(0);
                    break;
                // Keypad 2 for the spread bullet
                case KeyCode.Keypad2:
                    SetupCurrentBullet(1);
                    break;
                // Keypad 3 for the guided bullet
                case KeyCode.Keypad3:
                    SetupCurrentBullet(2);
                    break;
            }
        }  
    }

    /// <summary>
    /// Changes the current bullet type being used
    /// </summary>
    /// <param name="bulletChoosen"></param>
    private void SetupCurrentBullet(int bulletChoosen)
    {
        // Do not allow changing to another bullet while the not enough energy coroutine is running. This is to avoid a bug in the bar sprite
        if (!noEnergyRoutineIsRunning)
        {
            // Deactivate the current energy bar being used
            energyBars[currentBullet].SetActive(false);
            // Set the currentBullet int by the keypad pressed by the player
            currentBullet = bulletChoosen;
            // Activate the energy bar of the new current bullet
            energyBars[currentBullet].SetActive(true);
            // Cache the scale of this energy bar
            energyBarScale = energyBars[currentBullet].transform.localScale;
            // Firing cooldown calculation: the current bullet rate of fire minus the player current rate of fire (which is upgradeable, further decreasing cooldown between shots)
            fireTimer = bullets[currentBullet].rateOfFire - shootRateOfFire;
            // Change the current crosshair by the one corresponding with the current bullet
            activeCrosshairSprite.sprite = crosshairColors[currentBullet];
            // Change the sprite library asset in use by the one correspoding with the current bullet
            gemSpriteLibrary.spriteLibraryAsset = spriteLibraries[currentBullet];
            // Set the new range limit
            rangeArea.radius = shootRange + bullets[currentBullet].range;

            if (usePool)
            {
                // Single object pool, so clear all of the previsouly used bullets
                bulletPool.Clear();
                InitializeBulletPool();
            }
        }
    }

    /// <summary>
    /// Initialize the object pool, using the current bullet type selected
    /// </summary>
    private void InitializeBulletPool()
    {
        bulletPool = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, 
                                                           true, bulletPoolSize[currentBullet], bulletPoolMaxSize[currentBullet]);
    }

    /// <summary>
    /// Creates a new bullet to be used in the object pool
    /// </summary>
    /// <returns>The newly created bullet</returns>
    private Bullet CreatePooledObject()
    {
        Bullet bullet = Instantiate(bullets[currentBullet]);
        bullet.pooledObject = true;
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    /// <summary>
    /// Get the next bullet available in the pool
    /// </summary>
    /// <param name="bullet">The next available bullet</param>
    private void OnTakeFromPool(Bullet bullet)
    {
        // Make sure the bullet will appear at the magical gem position
        bullet.transform.position = activeMagicalGem.transform.position;
        // Set the bullet isReleased bool to false, for it to be able to be released again
        bullet.isReleased = false;
        bullet.gameObject.SetActive(true);
    }

    /// <summary>
    /// Returns the selected bullet to the pool
    /// </summary>
    /// <param name="bullet">The selected bullet</param>
    private void OnReturnToPool(Bullet bullet)
    {
        if (!bullet.isReleased)
        {
            // Bullet just released, so set the corresponding variable to avoid releasing it again
            bullet.isReleased = true;
            bullet.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Destroy the object from the pool
    /// </summary>
    /// <param name="bullet"></param>
    private void OnDestroyObject(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    /// <summary>
    /// Regenerates the player energy over time, if the current bullet has an alternate shoot
    /// </summary>
    private void EnergyRegen()
    {
        if (bullets[currentBullet].hasAlternateShoot)
        {
            if (energy < maxEnergy)
            {
                energy += energyRegen * Time.deltaTime;
                foreach(GameObject bar in energyBars)
                    bar.transform.localScale = new Vector3(energy, energyBarScale.y, energyBarScale.z);
            }
        }  
    }

    /// <summary>
    /// If this coroutine is not running, play the not enough energy sound and flashes the energy bar two times
    /// </summary>
    /// <returns></returns>
    IEnumerator NotEnoughEnergy()
    {
        if (!noEnergyRoutineIsRunning)
        {
            noEnergyRoutineIsRunning = true;

            AudioManager.instance.PlaySound("NotEnoughEnergy", transform.position);

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.2f);
                energyBars[currentBullet].GetComponent<SpriteRenderer>().material = flashBarMaterial;
                yield return new WaitForSeconds(0.2f);
                energyBars[currentBullet].GetComponent<SpriteRenderer>().material = originalBarMaterial;
            }

            noEnergyRoutineIsRunning = false;
        }

        StopCoroutine("NotEnoughEnergy");
    }
}
