using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class that controls all aspects of the projectiles used in the shooting mechanic
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// The position of the target of the shoot 
    /// </summary>
    private Vector2 target;

    /// <summary>
    /// Rigidbody 2D attached to this object
    /// </summary>
    private Rigidbody2D thisRigidbody2D;

    [Header("Objects Setup")]
    /// <summary>
    /// Prefab that simulates the collision effect of this object
    /// </summary>
    public BulletCollisionEffect collisionEffectPrefab;

    /// <summary>
    /// Variable that will hold the instantiated collision effect prefab
    /// </summary>
    private BulletCollisionEffect collisionEffect;

    [Header("Variables Setup")]
    /// <summary>
    /// Strenght of the bullet (ammount of damage cause to targets)
    /// </summary>
    public float strenght;

    /// <summary>
    /// Speed of the bullet (travel time speed)
    /// </summary>
    public float speed;

    /// <summary>
    /// Range of the bullet (maximum range before disappearing)
    /// </summary>
    public float range;

    /// <summary>
    /// Rate of fire of the bullet 
    /// </summary>
    public float rateOfFire;

    /// <summary>
    /// Energy cost of the bullet alternate shoot, when applicable
    /// </summary>
    public float energyCost;

    /// <summary>
    /// Float used as an spread off set as the target position, affects accuracy of the bullet
    /// </summary>
    public float spreadOffSet;

    /// <summary>
    /// Enum used to determine the behaviour of this bullet
    /// </summary>
    public ShootType shootType;

    /// <summary>
    /// Bool used to prevent trying to release an already released object (object pooling only)
    /// </summary>
    public bool isReleased;

    [Header("Alternate shoot variables")]
    /// <summary>
    /// Bool used to determine if the current bullet has an alternate shoot version
    /// </summary>
    public bool hasAlternateShoot;

    /// <summary>
    /// Sprite used in the alternate shoot
    /// </summary>
    public Sprite alternateSprite;

    /// <summary>
    /// Bool used to determine if an alternate shoot was used
    /// </summary>
    private bool alternateShoot;

    /// <summary>
    /// The game object sprite renderer
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The game object initial sprite
    /// </summary>
    private Sprite originalSprite;

    [HideInInspector]
    /// <summary>
    /// Bool that defines if this object belongs to an object pool
    /// </summary>
    public bool pooledObject;

    /// <summary>
    /// Dynamic action that is received from the MagicalGemController class. Basically, tells which behavior this will have, when colliding / reaching the range limit / expiring
    /// Using object pooling: calls the Release function from the pool, deactivating this game object to be reused later
    /// Not using object pooling: destroy this game object
    /// </summary>
    private Action<Bullet> killAction;

    [Header("Guided shoot only")]
    /// <summary>
    /// Guided shoot tracking timer (time before the bullet will begin tracking enemies in the camera view)
    /// </summary>
    public float trackingTimer;

    /// <summary>
    /// Bullet life span before disappearing (when not going pass the range limit nor colliding with anything)
    /// </summary>
    public float shootTimer;

    /// <summary>
    /// Game object used as the alternate guided bullet shoot
    /// </summary>
    public MiniGuidedBullet guidedMiniBullet;

    /// <summary>
    /// Array of mini guided bullet that will hold the ammount dictated by the miniGuidedAmmount variable
    /// Usable only if this bullet is of guided type, for its alternate shoot version
    /// </summary>
    private MiniGuidedBullet[] pooledMiniGuidedBullet;

    /// <summary>
    /// Ammount of guided mini bullet to be instantiated as the main bullet is destroyed
    /// </summary>
    public int miniGuidedAmmount;

    /// <summary>
    /// Prefab of the collision effect of the guided shoot variant
    /// </summary>
    public ExplosionEffect explosionEffectPrefab;

    /// <summary>
    /// Variable that will hold the instantiated explosion effect prefab
    /// </summary>
    private ExplosionEffect explosionEffect;

    /// <summary>
    /// The game object echo effect script (used in guided shoot type)
    /// </summary>
    private EchoEffect echoEffect;

    /// <summary>
    /// The game object trail renderer (used in guided shoot type)
    /// </summary>
    private TrailRenderer trailRenderer;

    /// <summary>
    /// The axis rotation script being used by this bullet (guided only)
    /// </summary>
    private Axis_Rotation axis_Rotation;

    [Header("Echo effect is not perfomance friendly")]
    /// <summary>
    /// Enum used to define which trail effect the guided shoot will have
    /// </summary>
    public TrailType trailType;

    /// <summary>
    /// Actual timer used in the tracking process (affected by Time.deltaTime)
    /// </summary>
    private float actualTrackingTimer;

    /// <summary>
    /// Bool used to affect the behavior of the guided shoot variant 
    /// </summary>
    private bool foundTarget;

    /// <summary>
    /// Float that will be affected by the passing of time
    /// </summary>
    private float actualShootTimer;

    [Header("Spread shoot only")]
    /// <summary>
    /// Float used in the spread shoot alternate version: faster speed
    /// </summary>
    public float chainSpeed;

    /// <summary>
    /// Float used in the spread shoot alternate version: higher rate of fire
    /// </summary>
    public float chainRateOfFire;

    /// <summary>
    /// Float used in the spread shoot alternate version: less spread
    /// </summary>
    public float chainSpread;

    /// <summary>
    /// Variables that will reflect the changes on its attributes (necessity because of the spread shoot alternate version)
    /// </summary>
    private float actualSpeed, actualSpreadOffSet;

    /// <summary>
    /// The bullet animator, used only in the spread shoot type
    /// Must be deactivated to use the alternate shoot sprite
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Call the SetupCollisionEffect function if this is an pooled object
    /// </summary>
    private void Start()
    {
        if (pooledObject)
        {
            SetupCollisionEffect();
        }
    }

    /// <summary>
    /// Cache the necessary variables
    /// </summary>
    private void Awake()
    {
        if (shootType == ShootType.GUIDED)
        {
            InitializePooledMiniGuidedBullets();

            axis_Rotation = gameObject.GetComponent<Axis_Rotation>();
            echoEffect = gameObject.GetComponent<EchoEffect>();
            echoEffect.enabled = false;
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
            trailRenderer.enabled = false;
            actualShootTimer = shootTimer;
            actualTrackingTimer = trackingTimer;
        }
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        actualSpeed = speed;
        actualSpreadOffSet = spreadOffSet;
    }

    /// <summary>
    /// Use the track target proccess / life span function of the guided type shoot
    /// </summary>
    private void Update()
    {
        if (shootType == ShootType.GUIDED)
        {
            TrackTarget();
            GuidedShootLifeSpan();
        }       
    }

    /// <summary>
    /// If the bullet enters triggers of objects that has the DestroyBullet tag, destroy the bullet after a set time (to prevent bullets to stay infinitely inside the black hole)
    /// Only the guided version uses the life span function (like a missile), so this is for the other versions
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DestroyBullet"))
        {
            Invoke("DestroyBehavior", .5f);
        }
    }

    /// <summary>
    /// Call the DestroyBehavior after it reaches its range limit (the circle collider 2D with the RangeLimit tag) only if the game object is active
    /// </summary>
    /// <param name="collision">The other object collider</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Deactivating a game object while inside a trigger, runs the OnTriggerExit, so to avoid calling the DestroyBehavior twice, must check if the game object is active
        if (collision.CompareTag("RangeLimit") && gameObject.activeSelf)
            DestroyBehavior();
    }

    /// <summary>
    /// Call the DestroyBehavior function when colliding
    /// Also call the target dummy Hit function in collision (guided is an aoe, which will damage enemies in another script)
    /// </summary>
    /// <param name="collision">The other object collider</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && shootType != ShootType.GUIDED)
            collision.collider.GetComponent<TargetDummy>().Hit(strenght, transform);

        DestroyBehavior();
    }

    /// <summary>
    /// Function that defines what happens when the bullet collides / reach its range limit / expires
    /// If it belongs to an object pool: Activates its collision / explosion effect, check for area of effect damage in the
    /// case of explosion effect, reset the bullet timers and deactivates itself afterwards    /// To avoid the problem of trying to release an already released object, the 
    /// isReleased bool is used
    /// If it dont belong: Simply call the killAction function which will, in this case, destroy the game object
    /// </summary>
    private void DestroyBehavior()
    {
        if (!isReleased)
        {
            if (pooledObject)
            {
                if (shootType == ShootType.GUIDED)
                {
                    explosionEffect.Activate(transform.position);
                    explosionEffect.AoEDamage();
                    actualShootTimer = shootTimer;
                    actualTrackingTimer = trackingTimer;
                    foundTarget = false;

                    // Spawns the guided shoot alternate version if applicable
                    SpawnMiniGuidedShoot();
                }
                else
                    collisionEffect.Activate(transform.position);
            }
            // Make sure the trail effect is disabled to prevent visual bugs when an object with its trail enabled is deactivated / activated in quick succession
            SetTrailEffect(false);
            killAction(this);
        }  
    }

    /// <summary>
    /// Setup which behavior this object will have when being destroyed (colliding or reaching its range limit)
    /// With object pooling: release the object back to the pool (deactivate it)
    /// Without object pooling: destroy the object
    /// </summary>
    /// <param name="killAction">Function received from the magical gem controller script</param>
    public void Init(Action<Bullet> killAction)
    {
        this.killAction = killAction;
    }

    /// <summary>
    /// When the bullet is destroyed, call DestroyFromPool function or NonPoolDetroy function depending if this belong or not to an object pool
    /// </summary>
    private void OnDestroy()
    {
        // Validation to avoid instantiating game objects when quitting the game
        if (gameObject.scene.isLoaded)
        {
            if (pooledObject)
            {
                DestroyFromPool();
            }
            else
                NonPoolDestroy();
        }   
    }

    /// <summary>
    /// Object belong to a pool, meaning it has been destroyed by the object pool Clear function
    /// So, its collision / explosion effect must be destroyed with it
    /// Also, destroys all the guided shoot alternate bullets, if applicable
    /// </summary>
    private void DestroyFromPool()
    {
        if (shootType == ShootType.GUIDED)
        {
            if (explosionEffect != null)
                Destroy(explosionEffect.gameObject);
            for (int i = 0; i < pooledMiniGuidedBullet.Length; i++)
            {
                Destroy(pooledMiniGuidedBullet[i].gameObject);
            }
        }
            
        else if(collisionEffect != null)
            Destroy(collisionEffect.gameObject);
    }

    /// <summary>
    /// Object dont belong to a pool, so instantiate its collision / explosion effect and if it is a guided shoot that was used with its alternate shoot
    /// instantiate at the bullet last position its alternate versions (ammount being controlled by the miniGuidedAmmount variable)
    /// </summary>
    private void NonPoolDestroy()
    {
        if(shootType == ShootType.GUIDED)
        {
            // Create the necessary reference
            ExplosionEffect explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            explosionEffect.SetupReferences(pooledObject, alternateShoot, strenght, transform);
            explosionEffect.AoEDamage();
            this.explosionEffect = explosionEffect;
        }
        else
        {
            // Create the necessary reference
            BulletCollisionEffect collisionEffect = Instantiate(collisionEffectPrefab, transform.position, Quaternion.identity);
            collisionEffect.fromPooledObject = false;
            this.collisionEffect = collisionEffect;
        }   

        SpawnMiniGuidedShoot();
    }

    /// <summary>
    /// Instantiate / activate the guided shoot alternate version projectiles
    /// </summary>
    private void SpawnMiniGuidedShoot()
    {
        // If, in the moment this function was called, the bullet was of the guided type and its alternate shoot was used
        if (shootType == ShootType.GUIDED && alternateShoot)
        {
            // In the case of object pooling being used
            if (pooledObject)
            {
                // Reset / set the necessary variables and activate each of this guided bullet alternate projectiles
                for (int i = 0; i < pooledMiniGuidedBullet.Length; i++)
                {
                    pooledMiniGuidedBullet[i].ResetMiniBullet();
                    pooledMiniGuidedBullet[i].SetupMiniBullet(target, speed, strenght, pooledObject);
                    pooledMiniGuidedBullet[i].Activate(transform.position, spreadOffSet);
                }
              // Not using object pooling
            } else
                // Instantiate several of these projectiles, ammount is dictated by the miniGuidedAmmount variable
                for (int i = 0; i < miniGuidedAmmount; i++)
                {
                    // They are instantiate in a random position near the location where the initial bullet exploded
                    MiniGuidedBullet miniBullet = Instantiate(guidedMiniBullet, new Vector2(Random.Range(transform.position.x - spreadOffSet, transform.position.x + spreadOffSet),
                                                              Random.Range(transform.position.y - spreadOffSet, transform.position.y + spreadOffSet)), Quaternion.identity);
                    // Function that setups these projectiles, by basically using the same stats as the main projectile
                    miniBullet.SetupMiniBullet(target, speed, strenght, pooledObject);
                }
        }

    }

    /// <summary>
    /// Initialize the array of mini guided bullets, used if this bullet belongs to a pool
    /// </summary>
    public void InitializePooledMiniGuidedBullets()
    {
        // Double check to avoid instantiating more than necessary
        if (pooledObject && pooledMiniGuidedBullet == null)
        {
            pooledMiniGuidedBullet = new MiniGuidedBullet[miniGuidedAmmount];

            for (int i = 0; i < pooledMiniGuidedBullet.Length; i++)
            {
                MiniGuidedBullet miniGuidedBullet = Instantiate(guidedMiniBullet);
                miniGuidedBullet.fromPooledObject = true;
                miniGuidedBullet.gameObject.SetActive(false);
                pooledMiniGuidedBullet[i] = miniGuidedBullet;
            }     
        }
    }

    /// <summary>
    /// This object belong to a pool so its collision / explosion effect must be instantiate beforehand and deactivated in order to be reused
    /// </summary>
    private void SetupCollisionEffect()
    {
        if(shootType == ShootType.GUIDED)
        {
            ExplosionEffect explosionEffect = Instantiate(explosionEffectPrefab);
            explosionEffect.SetupReferences(pooledObject, alternateShoot, strenght, transform);
            this.explosionEffect = explosionEffect;
            this.explosionEffect.gameObject.SetActive(false);
        } else
        {
            BulletCollisionEffect collisionEffect = Instantiate(collisionEffectPrefab);
            collisionEffect.fromPooledObject = true;
            this.collisionEffect = collisionEffect;
            this.collisionEffect.gameObject.SetActive(false);
        } 
    }

    /// <summary>
    /// Function that controls the bullet trajectory
    /// </summary>
    /// <param name="target">The mouse position</param>
    /// <param name="speed">The speed of the player shoot (rank at the moment of the shoot)</param>
    /// <param name="alternateShoot">Bool that dictate if an alternate shoot was used (right click)</param>
    public void Shoot(Vector3 target, float speed, bool alternateShoot)
    {
        // Cache the alternate shoot bool, since there are behaviors that happens outside this function
        this.alternateShoot = alternateShoot;

        // If the current bullet has an alternate shoot and it was used, change the sprite to have a different look
        if (this.alternateShoot && hasAlternateShoot)
        {
            spriteRenderer.sprite = alternateSprite;

            // Also changes the echo effect sprite, when applicable
            if (trailType == TrailType.ECHOEFFECT)
                echoEffect.alternateShoot = alternateShoot;
        }
        // Not using alternate shoot, so use its original sprite (in case the player already used the alternate shoot, revert the sprite back to its original)
        else
            spriteRenderer.sprite = originalSprite;

        // Cache the target to be used outside this function
        this.target = target;

        switch (shootType)
        {
            // Simple behavior shoot - goes straight to the target
            case ShootType.DEFAULT:
                thisRigidbody2D.velocity = new Vector2(target.x, target.y).normalized * (this.speed + speed);
                AudioManager.instance.PlaySound("DefaultShoot", transform.position);
                break;
            // Shoot with a spread effect - random target position, based on a radius from the original target
            // Alternate shoot: gatling like version
            case ShootType.SPREAD:
                SpreadAlternateCalculation(this.alternateShoot);
                Vector2 randomTarget = new Vector2(Random.Range(target.x - actualSpreadOffSet, target.x + actualSpreadOffSet),
                                                   Random.Range(target.y - actualSpreadOffSet, target.y + actualSpreadOffSet));
                PointAtTarget();
                thisRigidbody2D.velocity = randomTarget.normalized * (actualSpeed + speed);
                AudioManager.instance.PlaySound(alternateShoot ? "SpreadAlternateShoot" : "SpreadShoot", transform.position);
                break;
            // Shoot with a guided behavior - since its a different kind of bullet, it uses logic apart from this function.
            // Simply play the bullet sound here
            // Alternate shoot: initial bullet spawns several others when destroyed
            case ShootType.GUIDED:
                AudioManager.instance.PlaySound(alternateShoot ? "GuidedAlternateShoot" : "GuidedShoot", transform.position);
                break;
        }
    }

    /// <summary>
    /// Function used in the spread shoot version: make the sprite points towards the target direction
    /// Only necessary if the sprite used is irregular (not rounded, etc)
    /// </summary>
    private void PointAtTarget()
    {
        Vector3 normalizedTarget = target.normalized;
        float angle = Mathf.Atan2(normalizedTarget.y, normalizedTarget.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Affects the variables used in the spread shoot version
    /// Using alternate shoot: faster speed, less spread and faster rate of fire (the last affecting the MagicalGemController script, since it controls the rate of fire)
    /// </summary>
    /// <param name="activated"></param>
    public void SpreadAlternateCalculation(bool activated)
    {
        actualSpeed = activated ? chainSpeed : speed;
        actualSpreadOffSet = activated ? chainSpread : spreadOffSet;
    }

    /// <summary>
    /// Guided bullet behavior
    /// Basically, makes use of a helper class, which tracks all objects in the player camera view and directs the bullet to the closest one
    /// Makes use of a tracking timer which essentially is a small delay before the bullet will begin flying / searching for a target
    /// Special trail effect when a target is found along with double fly / rotation speed
    /// Since this needs constant tracking, it is called in the update function
    /// </summary>
    private void TrackTarget()
    {
        actualTrackingTimer -= Time.deltaTime;
        if (actualTrackingTimer <= 0)
        {
            Transform closestTarget = Radar.instance.TargetPosition();

            if (closestTarget != null)
            {
                target = closestTarget.position;
                foundTarget = true;
                // Doubles the rotation animation speed
                axis_Rotation.speed = 800;
                // Zero the rigidbody velocity. Since this dont use the built-in physics, this is to prevent the move towards function struggling with the rigidbody velocity
                thisRigidbody2D.velocity = Vector2.zero;
                transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, SpeedBoost() * Time.deltaTime);
            } 

            if (!foundTarget)
            {
                axis_Rotation.speed = 400;
                thisRigidbody2D.velocity = target.normalized * this.speed;
            }

            SetTrailEffect(foundTarget);

        }
    }

    /// <summary>
    /// Activate / deactivate the selected trail effect
    /// </summary>
    /// <param name="enable"></param>
    private void SetTrailEffect(bool enable)
    {
        switch (trailType)
        {
            case TrailType.TRAILRENDERER:
                trailRenderer.enabled = enable;
                break;
            case TrailType.ECHOEFFECT:
                echoEffect.enabled = enable;
                break;
        }
    }

    /// <summary>
    /// Function used in the guided shoot version. Since it is like a guided missile, the idea is to make it explode if it doesnt reach a target / collide with anything
    /// </summary>
    private void GuidedShootLifeSpan()
    {
        actualShootTimer -= Time.deltaTime;
        if(actualShootTimer <= 0f)
        {
            DestroyBehavior();
        }
    }

    /// <summary>
    /// Doubles the guided bullet speed when a target is found
    /// </summary>
    /// <returns>Float that is the doubled speed</returns>
    private float SpeedBoost()
    {
        return speed * 2;
    }

    /// <summary>
    /// Enum that defines the bullet behavior
    /// </summary>
    public enum ShootType
    {
        DEFAULT,
        SPREAD,
        GUIDED
    }

    /// <summary>
    /// Enum that defines the type of trail effect
    /// </summary>
    public enum TrailType
    {
        NONE,
        TRAILRENDERER,
        ECHOEFFECT
    }
}
