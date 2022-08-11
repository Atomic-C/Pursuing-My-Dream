using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

/// <summary>
/// Class responsible for controlling the shooting magical gem / shooting mechanic
/// </summary>
public class MagicalGemController : MonoBehaviour
{
    /// <summary>
    /// Bool that initialize this script, used by the GameManager script
    /// </summary>
    public bool canStart;

    [Header("Object Pooling")]
    /// <summary>
    /// Bool used to change the behavior of the current bullet to make use of the object pooling technique
    /// </summary>
    public bool usePool;

    /// <summary>
    /// Array to be used in the object pool constructor (to aid dynamicity)
    /// </summary>
    public int[] bulletPoolSize, bulletPoolMaxSize;

    /// <summary>
    /// Object pool that hold all objects of the current bullet type
    /// </summary>
    private IObjectPool<Bullet> _bulletPool;

    [Header("Objects Setup")]
    /// <summary>
    /// Crosshair prefab, to be instantiated in case there is no active crosshair
    /// </summary>
    public GameObject crossHairPrefab;

    /// <summary>
    /// Magical gem prefab, to be instantiated in case there is no active gem
    /// </summary>
    public GameObject gemPrefab;

    /// <summary>
    /// Game object that represents each bullet type energy bar
    /// </summary>
    public GameObject energyBar;

    /// <summary>
    /// Game object that represents each bullet type energy bar icon
    /// </summary>
    public GameObject energyBarIcon;

    /// <summary>
    /// Array of sprites for all bullet types energy bar sprites
    /// </summary>
    public Sprite[] energyBarSprites;

    /// <summary>
    /// Array of sprites for all bullet types energy bar icon sprites
    /// </summary>
    public Sprite[] energyIconSprites;

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
    public SpriteLibraryAsset[] spriteLibraries;

    /// <summary>
    /// Reference to the player movement script
    /// </summary>    
    public Platform_Movement playerMovement;

    [Header("Gem Visual Setup")]
    ///<summary>
    /// Make use or not of the glow effect
    /// </summary>
    public bool useEmission;

    /// <summary>
    /// Property that acts as an "on change" listener, where it automatically activate / deactivate the outline effect (through the calling of the SetOutlineEffect function)
    /// </summary>
    public bool UseOutline {
        get
        {
            return _useOutline;
        } set
        {
            _useOutline = value;
            SetOutlineEffect();
        } 
    }

    /// <summary>
    /// Make use or not of the bullet glow effect
    /// </summary>
    public bool useBulletEmission;

    /// <summary>
    /// Color array used in the gem light color
    /// </summary>
    public Color[] gemLightColors;

    /// <summary>
    /// Color array used in the gem glow color
    /// </summary>
    public Color[] gemGlowColors;

    /// <summary>
    /// Controls the intensity of the gem glow
    /// </summary>
    public float gemGlowColorIntensity;

    /// <summary>
    /// Color array used in the gem outline color
    /// </summary>
    public Color[] gemOutlineColors;

    /// <summary>
    /// Controls the intensity of the gem outline
    /// </summary>
    public float gemOutlineIntensity;

    /// <summary>
    /// Controls the thickness of the outline
    /// </summary>
    public float gemOutlineThickness;

    /// <summary>
    /// Bool used as a backing field for the UseOutline property
    /// </summary>
    [SerializeField] private bool _useOutline;

    /// <summary>
    /// Bool that controls when the OnValidate will be able to run properly (things needs to happen on Start / Awake first and since OnValidate runs before them, this is to avoid
    /// the errors that would happen in this case)
    /// </summary>
    private bool _loaded;

    /// <summary>
    /// Magical gem object that is being used in the active scene
    /// </summary>
    private GameObject _activeGem;

    /// <summary>
    /// The light2D attached to the active gem
    /// </summary>
    private Light2D _gemLight2D;

    /// <summary>
    /// The sprite renderer of the active gem
    /// </summary>
    private SpriteRenderer _gemSpriteRenderer;

    /// <summary>
    /// The sprite renderer of the energy bar
    /// </summary>
    private SpriteRenderer _energyBarRenderer;

    /// <summary>
    /// The sprite renderer of the energy bar icon
    /// </summary>
    private SpriteRenderer _energyIconRenderer;

    /// <summary>
    /// Crosshair object that is being used in the active scene
    /// </summary>
    private GameObject _activeCrossHairObject;

    /// <summary>
    /// Trigger circle collider 2D used to define the range limit of the current bullet (calculation: player range statistic plus the current bullet range statistic - this affects
    /// the radius of the circle collider 2D)
    /// </summary>
    private CircleCollider2D _rangeArea;

    [Header("Variables Setup")]
    /// <summary>
    /// Float used to give the sensation of the gem going to above the player position so its not instant 
    /// </summary>
    public float smoothSpeed = 0.125f;

    /// <summary>
    /// Current bullet being used
    /// </summary>
    public int currentBullet;

    /// <summary>
    /// Statistics of the player: projectile strength, speed, range, maximum energy, current maximum energy, current energy and energy regen
    /// </summary>
    public float shootStrength, shootSpeed, shootRange, maxEnergy, currentMaxEnergy, energy, energyRegen;

    /// <summary>
    /// Statistics of the player rate of fire, one for bullet type
    /// </summary>
    public float[] rateOfFire;

    /// <summary>
    /// Bool used to show / hide a visual representation of the current shoot range
    /// </summary>
    public bool showRange;

    /// <summary>
    /// Float that will be affected by the rate of fire calculation
    /// </summary>
    private float _fireTimer;

    /// <summary>
    /// The magical gem animator
    /// </summary>
    private Animator _gemAnimator;

    /// <summary>
    /// The sprite library being used by the active magical gem
    /// </summary>
    private SpriteLibrary _gemSpriteLibrary;

    // Automated Setup

    /// <summary>
    /// Vector3 used to hold and manipulate the energy bar sprite scale. Used in the energy consumption and regen mechanic
    /// </summary>
    private Vector3 _energyBarScale;

    /// <summary>
    /// Bool used to avoid multiple energy bar flash / error sound playing, when there is no energy to use the current bullet alternate shoot
    /// </summary>
    private bool _noEnergyRoutineIsRunning;

    /// <summary>
    /// Vector3 used as an offset to the following movement
    /// </summary>
    private Vector3 _followOffset;

    /// <summary>
    /// Bool used to allow the next fire
    /// </summary>
    private bool _canFire;

    [Header("Fields change timer")]
    [Tooltip("Timer to prevent calling repeatedly functions from the OnGui function")]
    /// <summary>
    /// Timer to prevent calling repeatedly functions from the OnGui function
    /// </summary>
    public float setBoolTimer;

    /// <summary>
    /// Timer to prevent calling repeatedly functions from the OnGui function
    /// </summary>
    private float _setBoolTimer;

    /// <summary>
    /// Bool that works together with the _setBoolTimer to prevent calling repeatedly functions from the OnGui function 
    /// </summary>
    private bool _allowBoolChange;

    /// <summary>
    /// Guarantee there is one crosshair / magical gem in the scene and setup the necessary variable with the current bullet in use
    /// </summary> 
    private void Start()
    {
        if (canStart)
        {
            CheckCrosshair();
            CheckGem();
            SetupCurrentBullet(currentBullet);
            // Set the _loaded bool to true here and rerun the OnValidate
            _loaded = true;
            OnValidate();
        } 
    }

    /// <summary>
    /// Cache the necessary variable and initialize the object pool, if applicable
    /// </summary>
    private void Awake()
    {
        if (canStart)
        {
            // Begins being able to shoot
            _canFire = true;
            _allowBoolChange = true;
            _setBoolTimer = setBoolTimer;
            _energyBarRenderer = energyBar.GetComponent<SpriteRenderer>();
            _energyIconRenderer = energyBarIcon.GetComponent<SpriteRenderer>();

            InitializeBulletPool();
        }

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
        if (canStart)
        {
            Animate();
            FireBullet();
            EnergyRegen();
            AllowBoolChange();
        }
    }

    /// <summary>
    /// Follow the player with a small delay (defined in the smoothSpeed variable)
    /// </summary>
    private void FixedUpdate()
    {
        if(canStart)
            SmoothFollow.instance.FollowObject(transform, playerMovement.transform, _followOffset, smoothSpeed);
    }

    /// <summary>
    /// Check if a field has been changed in the Inspector
    /// If any field has changed in the Inspector and the _loaded is true, set the property UseOutline with the value in the _useOutline bool (essentially calling the Set function 
    /// of the property, which will call the SetOuline function). Basically, a work around to activate / deactivate the gem outline glow effect on changing the bool
    /// Although the OnValidate runs everytime a field has changed, this will only changes the outline when this field changed, was the _useOutline bool itself
    /// </summary>
    private void OnValidate()
    {
        if (_loaded)
            UseOutline = _useOutline;
    }

    /// <summary>
    /// Call the ChangeBullet function if the player pressed the designated keys
    /// </summary>
    private void OnGUI()
    {
        CheckKeyPressed();
    }

    /// <summary>
    /// Debug feature of the current bullet range limit
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (showRange)
            Gizmos.DrawWireSphere(_activeGem.transform.position, _rangeArea.radius * _rangeArea.transform.lossyScale.x);
    }

    public void Initialize()
    {
        Awake();
    }

    /// <summary>
    /// Function that controls the energy pickup mechanic (used by the CollectableManager)
    /// </summary>
    public void PickedUpEnergy(float value)
    {
        // Play the energy up sound
        AudioManager.instance.PlaySound("EnergyUp", transform.position);

        // Increase the current energy
        energy += value;

        // If the current energy goes above the maximum energy, set it to the maximum energy
        if(energy > currentMaxEnergy)
            energy = currentMaxEnergy;

        // Manipulate the energy bar local scale accordingly, if applicable (default shoot has the energy bar deactivated)
        if(energyBar.activeSelf)
            energyBar.transform.localScale = new Vector3(energy, _energyBarScale.y, _energyBarScale.z);
    }

    /// <summary>
    /// Function called when the player picks up an upgrade, to clear the object pool to include the new upgraded statistics, if applicable
    /// </summary>
    public void UpgradedStatistic()
    {
        // If using the object pool, reset it to include the new statistics value
        if (usePool)
        {
            _bulletPool.Clear();
            InitializeBulletPool();
        }
    }

    /// <summary>
    /// Upgrade the player shoot strength (if using percentage, the base value used is from the current bullet)
    /// </summary>
    /// <param name="value">The upgrade percentage</param>
    public void StrengthUpgrade(float value, bool isPercentage)
    {
        shootStrength += isPercentage ? (bullets[currentBullet].strength / 100) * value : value;
    }

    /// <summary>
    /// Upgrade the player shoot speed (if using percentage, the base value used is from the current bullet)
    /// </summary>
    /// <param name="value">The upgrade percentage</param>
    public void SpeedUpgrade(float value, bool isPercentage)
    {
        shootSpeed += isPercentage ? (bullets[currentBullet].speed / 100) * value : value;
    }

    /// <summary>
    /// Upgrade the player rate of fire for all bullets (if using percentage, the base value used is from the current bullet)
    /// </summary>
    /// <param name="value">The upgrade percentage</param>
    public void RateOfFireUpgrade(float value, bool isPercentage)
    {
        for (int i = 0; i < rateOfFire.Length; i++)
        {
            rateOfFire[i] += isPercentage ? (bullets[i].rateOfFire / 100) * value : value;
        }
        // Firing cooldown calculation: the current bullet rate of fire minus the player current rate of fire (which is upgradeable, further decreasing cooldown between shots)
        _fireTimer = bullets[currentBullet].rateOfFire - rateOfFire[currentBullet];
    }

    /// <summary>
    /// Upgrade the player shoot range (if using percentage, the base value used is from the current bullet)
    /// </summary>
    /// <param name="value">The upgrade percentage</param>
    public void RangeUpgrade(float value, bool isPercentage)
    {
        shootRange += isPercentage ? (bullets[currentBullet].range / 100) * value : value;
        // Set the new range limit
        if (_rangeArea == null || _activeGem == null)
            CheckGem();
        _rangeArea.radius = shootRange + bullets[currentBullet].range;
    }

    /// <summary>
    /// Upgrade the player max energy
    /// </summary>
    /// <param name="value">The upgrade percentage</param>
    public void EnergyUpgrade(float value, bool isPercentage)
    {
        currentMaxEnergy += isPercentage ? (currentMaxEnergy / 100) * value : value;
    }

    /// <summary>
    /// Upgrade the player energy regen
    /// </summary>
    /// <param name="value">The upgrade percentage</param>
    public void EnergyRegenUpgrade(float value, bool isPercentage)
    {
        energyRegen += isPercentage ? (energyRegen / 100) * value : value;
    }

    /// <summary>
    /// Function used by the game manager to maintain the player current energy levels between scenes
    /// </summary>
    /// <param name="currentEnergy">Previous scene current energy</param>
    public void SetEnergyBetweenScenes(float currentEnergy, float maxEnergy)
    {
        currentMaxEnergy = maxEnergy;
        energy = currentEnergy;
    }

    /// <summary>
    /// Set the shader graph _UseEmission property, activating / deactivating the effect
    /// </summary>
    private void SetOutlineEffect()
    {
        _gemSpriteRenderer.material.SetInt("_UseOutline", _useOutline ? 1 : 0);
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
        _followOffset = isGrounded ? new Vector3(0, 2, 0) : new Vector3(0, 1, 0);
        // Set the gem animator respective bool
        _gemAnimator.SetBool("playerIsGrounded", isGrounded);
    }

    /// <summary>
    /// Function that controls the gem firing
    /// </summary>
    private void FireBullet()
    {
        if (_canFire && Time.timeScale != 0)
        {
            // Left mouse click
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Shoot(InitBullet(false), false);

                _canFire = false;
            }
            // Right mouse click and the current bullet has an alternate shoot version
            else if (Input.GetKey(KeyCode.Mouse1) && bullets[currentBullet].hasAlternateShoot)
            {
                // If the player has the necessary energy to use the alternate shoot
                if (energy > bullets[currentBullet].energyCost)
                {
                    // If it is the spread shoot alternate version, uses a different rate of fire setting
                    _fireTimer = bullets[currentBullet].shootType == Bullet.ShootType.SPREAD ?
                                bullets[currentBullet].chainRateOfFire : bullets[currentBullet].rateOfFire - rateOfFire[currentBullet];

                    Shoot(InitBullet(true), true);

                    _canFire = false;

                    ConsumeEnergy();
                }
                // No energy necessary
                else
                    StartCoroutine(NotEnoughEnergy());
            }
        }
        // Deplete the current bullet cooldown before being able to shoot again
        else
            _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0f)
        {
            _canFire = true;
            _fireTimer = bullets[currentBullet].rateOfFire - rateOfFire[currentBullet];
        }

        if (useEmission)
        {
            // Make use or not of the glowing effect provided by the shader, depending on the _canFire bool
            // _canFire = true: player is not shooting, so dont use the effect
            // _canFire = false: player shoot, so use it
            _gemSpriteRenderer.material.SetInt("_UseEmission", _canFire ? 0 : 1);
        }
    }

    /// <summary>
    /// Instantiate the current bullet at the magical gem position
    /// </summary>
    /// <param name="alternateShoot">Bool that determines if an alternate shoot was fired</param>
    private Bullet InitBullet(bool alternateShoot)
    {
        // Instantiate / get the bullet prefab (at the gem position) attached to the respective Bullet class array 
        Bullet bullet = usePool ? _bulletPool.Get() : Instantiate(bullets[currentBullet], _activeGem.transform.position, Quaternion.identity);

        // Set the bullet stats
        bullet.SetBulletStats(shootStrength, shootSpeed, rateOfFire[currentBullet]);

        // Set the bullet UseEmission property that will automatically, on this bullet Bullet script, activate or deactivate its glow effect
        bullet.UseEmission = useBulletEmission;

        // Spread shoot alternate version need to have its animator disabled to be able to use the alternate shoot sprite
        if (bullet.shootType == Bullet.ShootType.SPREAD && alternateShoot)
            bullet.animator.enabled = false;
        else if (bullet.shootType == Bullet.ShootType.SPREAD)
            bullet.animator.enabled = true;

        bullet.Init(KillBullet);

        // Using the guided bullet version with object pooling: instantiate all of the alternate shoot version projectiles to be reused
        if (bullet.shootType == Bullet.ShootType.GUIDED && usePool)
            bullet.InitializePooledMiniGuidedBullets();

        return bullet;
        /*Make it ignore the player collider to avoid shooting itself
        >>> Decided to do this by changing the collision matrix instead
        Physics2D.IgnoreCollision(bullet.GetComponent<CircleCollider2D>(), playerObject.GetComponent<CircleCollider2D>(), true);
        Physics2D.IgnoreCollision(bullet.GetComponent<CircleCollider2D>(), bullets[currentBullet].gameObject.GetComponent<CircleCollider2D>(), true);*/
    }

    /// <summary>
    /// Calculate the target direction and call the Shoot function of the bullet using this direction
    /// </summary>
    /// <param name="bullet">The bullet to shoot</param>
    /// <param name="alternateShoot">Bool that determines if it is a normal shoot or an alternate version</param>
    private void Shoot(Bullet bullet, bool alternateShoot)
    {
        // Get the crosshair position
        Vector3 direction = _activeCrossHairObject.transform.position - bullet.transform.position;

        // Calls the Bullet script Shoot function, attached to this instantiated object
        bullet.Shoot(direction, alternateShoot);
    }

    /// <summary>
    /// Function that will dictate which behavior the bullet will have, depending if it is using object pooling or not
    /// </summary>
    /// <param name="bullet">The current bullet</param>
    private void KillBullet(Bullet bullet)
    {
        if (usePool)
            _bulletPool.Release(bullet);
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
        energyBar.transform.localScale = new Vector3(energy, _energyBarScale.y, _energyBarScale.z);
    }

    /// <summary>
    /// Guarantee there is one active magical gem in the current scene
    /// If there isnt, instantiate one at the position of the object this script is attached to
    /// </summary>
    private void CheckGem()
    {
        // Checking if there is a gem
        _activeGem = GameObject.FindGameObjectWithTag("MagicalGem");
        // If not
        if (_activeGem == null)
        {
            // Instantiate one and do the necessary setup for this script to work
            _activeGem = Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }
        
        // Make the gem a child of this object (necessary for the gem animation to work)
        _activeGem.transform.SetParent(transform, true);

        // Cache the necessary gem variables
        _gemSpriteLibrary = _activeGem.GetComponent<SpriteLibrary>();
        _gemAnimator = _activeGem.GetComponent<Animator>();
        _gemLight2D = _activeGem.GetComponent<Light2D>();
        _gemSpriteRenderer = _activeGem.GetComponent<SpriteRenderer>();
        _rangeArea = _activeGem.GetComponentInChildren<CircleCollider2D>();

    }

    /// <summary>
    /// Guarantee there is one active crosshair in the current scene
    /// If there isnt, instantiate one at the position of the object this script is attached to
    /// </summary>
    private void CheckCrosshair()
    {
        // Checking if there is a crosshair
        _activeCrossHairObject = GameObject.FindGameObjectWithTag("Crosshair");
        // If not
        if (_activeCrossHairObject == null)
        {
            // Instantiate one and do the necessary setup for this script to work
            _activeCrossHairObject = Instantiate(crossHairPrefab, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Calls functions when the designated key is pressed
    /// </summary>
    private void CheckKeyPressed()
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
                // Key E to activate / deactivate the visual effects
                case KeyCode.E:
                    SetVisualEffects();
                    break;
                // Key R to activate / deactivate the usage of the object pool
                case KeyCode.R:
                    ActivateObjectPooling();
                    break;
            }
        }
    }

    /// <summary>
    /// Activate / deactivate the visual effects
    /// </summary>
    private void SetVisualEffects()
    {
        // Since the OnGui is called several times per frame, this check is to prevent the quick reverting of the bool fields
        if(_allowBoolChange && Time.timeScale != 0)
        {
            useEmission = !useEmission;
            _useOutline = !_useOutline;
            useBulletEmission = !useBulletEmission;
            SetOutlineEffect();
            SetBoolTimer();
        }
    }

    /// <summary>
    /// Activate / Deactivate the use of the object pool
    /// </summary>
    private void ActivateObjectPooling()
    {
        // Since the OnGui is called several times per frame, this check is to prevent the quick reverting of the bool fields
        if (_allowBoolChange && Time.timeScale != 0)
        {
            usePool = !usePool;
            // If the object is not being used anymore, make sure to clear all game objects previously created by it
            if(!usePool)
                _bulletPool.Clear();
            SetBoolTimer();
        }
    }

    /// <summary>
    /// Function that controls the state of the bool that allows the changing of bools from the CheckKeyPressed function
    /// </summary>
    private void AllowBoolChange()
    {
        _setBoolTimer -= Time.deltaTime;
        if (_setBoolTimer < 0f)
        {
            SetBoolTimer();
        }
    }

    /// <summary>
    /// Reset the _setboolTimer and revert the _allowBoolChange state
    /// </summary>
    private void SetBoolTimer()
    {
        _setBoolTimer = setBoolTimer;
        _allowBoolChange = !_allowBoolChange;
    }

    /// <summary>
    /// Changes several aspects depending on the current bullet being used
    /// </summary>
    /// <param name="bulletChoosen">The selected bullet</param>
    private void SetupCurrentBullet(int bulletChoosen)
    {
        // Do not allow changing to another bullet while the not enough energy coroutine is running. This is to avoid a bug in the bar sprite
        if (!_noEnergyRoutineIsRunning && Time.timeScale != 0)
        {
            // Set the currentBullet int by the keypad pressed by the player
            currentBullet = bulletChoosen;
            // Activate / deactivate the energy bar of the new current bullet
            energyBar.SetActive(currentBullet == 0 ? false : true);
            // Activate / deactivate the energy bar icon of the new current bullet
            energyBarIcon.SetActive(currentBullet == 0 ? false : true);
            // Set the energy bar sprite accordingly
            _energyBarRenderer.sprite = energyBarSprites[currentBullet];
            // Set the energy bar icon sprite accordingly
            _energyIconRenderer.sprite = energyIconSprites[currentBullet];
            // Cache the scale of this energy bar
            _energyBarScale = energyBar.transform.localScale;
            // Set the bar size according to the player current energy
            energyBar.transform.localScale = new Vector3(energy, _energyBarScale.y, _energyBarScale.z);
            // Firing cooldown calculation: the current bullet rate of fire minus the player current rate of fire (which is upgradeable, further decreasing cooldown between shots)
            _fireTimer = bullets[currentBullet].rateOfFire - rateOfFire[currentBullet];
            // Change the current crosshair by the one corresponding with the current bullet
            _activeCrossHairObject.GetComponent<CrosshairController>().ChangeSprite(crosshairColors[currentBullet]);
            // Change the sprite library asset in use by the one correspoding with the current bullet
            _gemSpriteLibrary.spriteLibraryAsset = spriteLibraries[currentBullet];
            // Set the new range limit
            _rangeArea.radius = shootRange + bullets[currentBullet].range;
            // Change the gem light depending on the current bullet and using the color set in the gemLightColors array
            _gemLight2D.color = gemLightColors[currentBullet];
            /* Change the shader property with reference _GlowColor to use the corresponding color of the gemGlowColors array, depending on the current bullet and increases the color
            intensity using the gemGlowColorIntensity */
            _gemSpriteRenderer.material.SetColor("_GlowColor", gemGlowColors[currentBullet] * gemGlowColorIntensity);
            // The same as above but for the outline of the gem
            _gemSpriteRenderer.material.SetColor("_OutlineColor", gemOutlineColors[currentBullet] * gemOutlineIntensity);
            // Same idea but for controlling the outline thickness
            _gemSpriteRenderer.material.SetFloat("_OutlineThickness", gemOutlineThickness);

            if (usePool)
            {
                // Single object pool, so clear all of the previsouly used bullets
                _bulletPool.Clear();
                InitializeBulletPool();
            }
        }
    }

    /// <summary>
    /// Initialize the object pool, using the current bullet type selected
    /// </summary>
    private void InitializeBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject,
                                            true, bulletPoolSize[currentBullet], bulletPoolMaxSize[currentBullet]);
    }

    /// <summary>
    /// Creates a new bullet to be used in the object pool
    /// </summary>
    /// <returns>The newly created bullet</returns>
    private Bullet CreatePooledObject()
    {
        Bullet bullet = Instantiate(bullets[currentBullet]);
        bullet.SetBulletStats(shootStrength, shootSpeed, rateOfFire[currentBullet]);
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
        bullet.transform.position = _activeGem.transform.position;
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
    /// <param name="bullet">The bullet object</param>
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
            if (energy < currentMaxEnergy)
            {
                energy += energyRegen * Time.deltaTime;
                energyBar.transform.localScale = new Vector3(energy, _energyBarScale.y, _energyBarScale.z);
            }
        }
    }

    /// <summary>
    /// If this coroutine is not running, play the not enough energy sound and flashes the energy bar two times
    /// </summary>
    /// <returns></returns>
    IEnumerator NotEnoughEnergy()
    {
        if (!_noEnergyRoutineIsRunning)
        {
            _noEnergyRoutineIsRunning = true;

            AudioManager.instance.PlaySound("NotEnoughEnergy", transform.position);

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.2f);
                _energyBarRenderer.material = flashBarMaterial;
                yield return new WaitForSeconds(0.2f);
                _energyBarRenderer.material = originalBarMaterial;
            }

            _noEnergyRoutineIsRunning = false;
        }

        StopCoroutine("NotEnoughEnergy");
    }

}
