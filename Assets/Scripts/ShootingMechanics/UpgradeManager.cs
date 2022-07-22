using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
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
    /// The percentage increase when an upgrade is picked up
    /// </summary>
    public float strengthUpgrade, speedUpgrade, rangeUpgrade, rateOfFireUpgrade, energyUpgrade, energyRegenUpgrade;

    /// <summary>
    /// Game object that will be used as an visual indicator when the player picks an upgrade
    /// </summary>
    public GameObject strengthIndicator, speedIndicator, rateOfFireIndicator, rangeIndicator, energyIndicator, energyRegenIndicator, lifeIndicator;

    /// <summary>
    /// Reference to the magical gem controller object
    /// </summary>
    private MagicalGemController _magicalGemController;

    /// <summary>
    /// Indication of how many upgrades the player currently has for each statistic
    /// </summary>
    private int _strengthRank, _speedRank, _rangeRank, _rateOfFireRank, _energyRank, _energyRegenRank, _lifeRank;

    /// <summary>
    /// Empty game object that serves as a parent for the upgrade indicator (to make the upgrade indicator animation to work)
    /// </summary>
    private GameObject indicatorParent;

    private void Awake()
    {
        if(canStart)
            _magicalGemController = gameObject.GetComponent<MagicalGemController>();
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
                if (_strengthRank == strengthRankMax)
                    return;
                break;
            case 2:
                if (_speedRank == speedRankMax)
                    return;
                break;
            case 3:
                if (_rateOfFireRank == rateOfFireRankMax)
                    return;
                break;
            case 4:
                if (_rangeRank == rangeRankMax)
                    return;
                break;
            case 5:
                if (_energyRank == energyRankMax)
                    return;
                break;
            case 6:
                if (_energyRegenRank == energyRegenRankMax)
                    return;
                break;
            case 7:
                if (_lifeRank == lifeRankMax)
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
                _strengthRank++;
                break;
            // 2 = shoot speed upgrade
            case 2:
                // Call the magical gem controller speed upgrade
                _magicalGemController.SpeedUpgrade(speedUpgrade, isPercentage);
                // Instantiate the speed indicator object and make it a child of the indicator parent object
                Instantiate(speedIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the speed rank
                _speedRank++;
                break;
            // 3 = rate of fire upgrade
            case 3:
                // Call the magical gem controller rate of fire upgrade
                _magicalGemController.RateOfFireUpgrade(rateOfFireUpgrade, isPercentage);
                // Instantiate the rate of fire indicator object and make it a child of the indicator parent object
                Instantiate(rateOfFireIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the rate of fire rank
                _rateOfFireRank++;
                break;
            // 4 = shoot range upgrade
            case 4:
                // Call the magical gem controller range upgrade
                _magicalGemController.RangeUpgrade(rangeUpgrade, isPercentage);
                // Instantiate the range indicator object and make it a child of the indicator parent object
                Instantiate(rangeIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the range rank
                _rangeRank++;
                break;
            // 5 = maximum energy upgrade
            case 5:
                // Call the magical gem controller energy upgrade
                _magicalGemController.EnergyUpgrade(energyUpgrade, isPercentage);
                // Instantiate the energy indicator object and make it a child of the indicator parent object
                Instantiate(energyIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the energy rank
                _energyRank++;
                break;
            // 6 = energy regen upgrade
            case 6:
                // Call the magical gem controller energy regen upgrade
                _magicalGemController.EnergyRegenUpgrade(energyRegenUpgrade, isPercentage);
                // Instantiate the energy regen indicator object and make it a child of the indicator parent object
                Instantiate(energyRegenIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the energy regen rank
                _energyRegenRank++;
                break;
            // 7 = player health upgrade
            case 7:
                // Call the player health upgrade
                playerHealth.PickedUpHealthUpgrade();
                // Instantiate the health indicator object and make it a child of the indicator parent object 
                Instantiate(lifeIndicator).transform.SetParent(indicatorParent.transform);
                // Increases the life rank
                _lifeRank++;
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
