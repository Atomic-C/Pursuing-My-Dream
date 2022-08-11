using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    /// <summary>
    /// Bool that initialize this script, used by the GameManager script
    /// </summary>
    public bool canStart;

    [Header("Upgrades config")]
    /// <summary>
    /// Bool used to control how the calculation is done: percentage or flat value
    /// </summary>
    public bool isPercentage;

    /// <summary>
    /// Holds a reference to the player health script
    /// </summary>
    public PlayerHealth playerHealth;

    /// <summary>
    /// Value that determine a limit in this statistic maximum upgrade
    /// </summary>
    public int strengthRankMax, speedRankMax, rangeRankMax, rateOfFireRankMax, energyRankMax, energyRegenRankMax, lifeRankMax;

    /// <summary>
    /// Indication of how many upgrades the player currently has for each statistic
    /// </summary>
    public int strengthRank, speedRank, rangeRank, rateOfFireRank, energyRank, energyRegenRank, lifeRank;

    /// <summary>
    /// The percentage increase when an upgrade is picked up
    /// </summary>
    public float strengthUpgrade, speedUpgrade, rangeUpgrade, rateOfFireUpgrade, energyUpgrade, energyRegenUpgrade;

    /// <summary>
    /// Game object that will be used as an visual indicator when the player picks an upgrade
    /// </summary>
    public GameObject strengthIndicator, speedIndicator, rateOfFireIndicator, rangeIndicator, energyIndicator, energyRegenIndicator, lifeIndicator;

    /// <summary>
    /// Array of sprites representing the upgrades that has 10 ranks
    /// </summary>
    public Sprite[] upgradeBarSprites_10;

    /// <summary>
    /// Array of sprites representing the upgrades that has 5 ranks
    /// </summary>
    public Sprite[] upgradeBarSprites_5;

    /// <summary>
    /// Cache all the upgrade bars Image component that are in the pause menu
    /// </summary>
    private Image[] _upgradeBarImages;

    /// <summary>
    /// Reference to the magical gem controller object
    /// </summary>
    private MagicalGemController _magicalGemController;

    /// <summary>
    /// Empty game object that serves as a parent for the upgrade indicator (to make the upgrade indicator animation to work)
    /// </summary>
    private GameObject indicatorParent;

    /// <summary>
    /// Cache and initialize the necessary variables
    /// </summary>
    private void Awake()
    {
        if (canStart)
        {
            // Initialize the upgrade bars Image component array (there are 7 upgrades in total)
            _upgradeBarImages = new Image[7];

            // Iterate the newly created array
            for(int i = 0; i < _upgradeBarImages.Length; i++)
            {
                // Then go get the reference for all the upgrade bars objects using their respective tag and initialize its sprite (to maintain the visual
                // representation of the player current rank in each statistics)
                switch (i)
                {
                    case 0:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("StrengthUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_10[strengthRank];
                        break;
                    case 1:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("SpeedUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_5[speedRank];
                        break;
                    case 2:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("RateOfFireUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_10[rateOfFireRank];
                        break;
                    case 3:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("RangeUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_5[rangeRank];
                        break;
                    case 4:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("EnergyUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_10[energyRank];
                        break;
                    case 5:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("EnergyRegenUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_10[energyRegenRank];
                        break;
                    case 6:
                        _upgradeBarImages[i] = GameObject.FindGameObjectWithTag("LifeUPG").GetComponent<Image>();
                        _upgradeBarImages[i].sprite = upgradeBarSprites_5[lifeRank];
                        break;
                }
            }

            // Get the pause menu object
            GameObject pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");

            // Pass its reference to the pause manager
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PauseManager>().SetPauseMenuObject(pauseMenu);

            // Deactivate the pause menu object
            pauseMenu.SetActive(false);
        }            
    }

    /// <summary>
    /// Function used by the game manager, to maintain the player shooting statistics ranks between scenes
    /// </summary>
    /// <param name="strength">Current strength rank</param>
    /// <param name="speed">Current speed rank</param>
    /// <param name="range">Current range rank</param>
    /// <param name="rateOfFire">Current rate of fire rank</param>
    /// <param name="energy">Current energy rank</param>
    /// <param name="energyRegen">Current energy regen rank</param>
    /// <param name="life">Current life rank</param>
    public void SetUpgradeRanks(int strength, int speed, int range, int rateOfFire, int energy, int energyRegen, int life)
    {
        _magicalGemController = gameObject.GetComponent<MagicalGemController>();

        // Set the current respective rank and recalculate all statistics, by using the magical gem controller upgrades function
        strengthRank = strength;
        for (int i = 0; i < strengthRank; i++)
            _magicalGemController.StrengthUpgrade(strengthUpgrade, isPercentage);

        speedRank = speed;
        for (int i = 0; i < speedRank; i++)
            _magicalGemController.SpeedUpgrade(speedUpgrade, isPercentage);

        rangeRank = range;
        for (int i = 0; i < rangeRank; i++)
            _magicalGemController.RangeUpgrade(rangeUpgrade, isPercentage);

        rateOfFireRank = rateOfFire;
        for (int i = 0; i < rateOfFireRank; i++)
            _magicalGemController.RateOfFireUpgrade(rateOfFireUpgrade, isPercentage);

        energyRank = energy;
        /*for (int i = 0; i < energyRank; i++)
            _magicalGemController.EnergyUpgrade(energyUpgrade, isPercentage);*/

        energyRegenRank = energyRegen;
        for (int i = 0; i < energyRegenRank; i++)
            _magicalGemController.EnergyRegenUpgrade(energyRegenUpgrade, isPercentage);

        // Player life ranks are calculated in the player health script, here is used for the pause menu visual representation
        lifeRank = life;
    }

    public void Initialize()
    {
        Awake();
    }

    /// <summary>
    /// Check which upgrade was picked up and apply this upgrade into the magical gem controller
    /// </summary>
    /// <param name="upgradeTag">The upgrade object tag</param>
    public void PickedUpUpgrade()
    {
        // Play the upgrade sound
        AudioManager.instance.PlaySound("PowerUp", transform.position);

        // Randomize the upgrade picked up
        int upgrade = Random.Range(1, 8);

        // Check which upgrade was picked up and verify if the player reached its maximum rank in this upgrade statistic
        // If the maximum rank was reached, exit the function
        switch (upgrade)
        {
            case 1:
                if (strengthRank == strengthRankMax)
                    return;
                break;
            case 2:
                if (speedRank == speedRankMax)
                    return;
                break;
            case 3:
                if (rateOfFireRank == rateOfFireRankMax)
                    return;
                break;
            case 4:
                if (rangeRank == rangeRankMax)
                    return;
                break;
            case 5:
                if (energyRank == energyRankMax)
                    return;
                break;
            case 6:
                if (energyRegenRank == energyRegenRankMax)
                    return;
                break;
            case 7:
                if (lifeRank == lifeRankMax)
                    return;
                break;
        }

        // Initialize the indicator parent object
        indicatorParent = new GameObject();
        // Set its position as the same as the magical gem controller position (the same as the magical gem too)
        indicatorParent.transform.position = _magicalGemController.transform.position;
        // But changes its position on the y axis, below the magical gem controller
        indicatorParent.transform.position = indicatorParent.transform.position.WithAxis(Axis.Y, indicatorParent.transform.position.y - 1f);

        // Check which upgrade kind was picked up
        switch (upgrade)
        {
            // 1 = shoot strength upgrade
            case 1:
                // Call the magical gem controller strength upgrade
                _magicalGemController.StrengthUpgrade(strengthUpgrade, isPercentage);
                // Instantiate the strength indicator object and make it a child of the indicator parent object
                Instantiate(strengthIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the strength rank
                strengthRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_10[strengthRank];
                break;
            // 2 = shoot speed upgrade
            case 2:
                // Call the magical gem controller speed upgrade
                _magicalGemController.SpeedUpgrade(speedUpgrade, isPercentage);
                // Instantiate the speed indicator object and make it a child of the indicator parent object
                Instantiate(speedIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the speed rank
                speedRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_5[speedRank];
                break;
            // 3 = rate of fire upgrade
            case 3:
                // Call the magical gem controller rate of fire upgrade
                _magicalGemController.RateOfFireUpgrade(rateOfFireUpgrade, isPercentage);
                // Instantiate the rate of fire indicator object and make it a child of the indicator parent object
                Instantiate(rateOfFireIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the rate of fire rank
                rateOfFireRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_10[rateOfFireRank];
                break;
            // 4 = shoot range upgrade
            case 4:
                // Call the magical gem controller range upgrade
                _magicalGemController.RangeUpgrade(rangeUpgrade, isPercentage);
                // Instantiate the range indicator object and make it a child of the indicator parent object
                Instantiate(rangeIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the range rank
                rangeRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_5[rangeRank];
                break;
            // 5 = maximum energy upgrade
            case 5:
                // Call the magical gem controller energy upgrade
                _magicalGemController.EnergyUpgrade(energyUpgrade, isPercentage);
                // Instantiate the energy indicator object and make it a child of the indicator parent object
                Instantiate(energyIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the energy rank
                energyRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_10[energyRank];
                break;
            // 6 = energy regen upgrade
            case 6:
                // Call the magical gem controller energy regen upgrade
                _magicalGemController.EnergyRegenUpgrade(energyRegenUpgrade, isPercentage);
                // Instantiate the energy regen indicator object and make it a child of the indicator parent object
                Instantiate(energyRegenIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the energy regen rank
                energyRegenRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_10[energyRegenRank];
                break;
            // 7 = player health upgrade
            case 7:
                // Call the player health upgrade
                playerHealth.PickedUpHealthUpgrade();
                // Instantiate the health indicator object and make it a child of the indicator parent object 
                Instantiate(lifeIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the life rank
                lifeRank++;
                // Change the respective upgrade bar to reflect the new rank attained
                _upgradeBarImages[upgrade - 1].sprite = upgradeBarSprites_5[lifeRank];
                break;
        }

        // If the upgrade wasnt the player health one, call the magical gem controller UpgradedStatistic function
        if(upgrade != 7)
            _magicalGemController.UpgradedStatistic();

        // Destroy the indicator parent object after a set ammount of time (consequently, the upgrade indicator as well
        Invoke("DestroyIndicator", 2f);
    }

    /// <summary>
    /// Destroy the instantiated indicator parent (and upgrade indicator) object
    /// </summary>
    private void DestroyIndicator()
    {
        Destroy(indicatorParent);
    }
}
