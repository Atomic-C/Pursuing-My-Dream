using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object script that holds all the player statistics, to be maintained between scenes
/// </summary>
[CreateAssetMenu(fileName = "Player Stats", menuName = "Player/Stats", order = 1)]
public class PlayerStats : ScriptableObject
{
    /// <summary>
    /// Coins counter
    /// </summary>
    public int coins;

    /// <summary>
    /// Current and max health
    /// </summary>
    public int currentHealth, maxHealth;

    /// <summary>
    /// Current and max energy
    /// </summary>
    public float currentEnergy, maxEnergy;

    /// <summary>
    /// Current bullet type
    /// </summary>
    public int currentBullet;

    /// <summary>
    /// Current statistics ranks
    /// </summary>
    public int strengthRank, speedRank, rangeRank, rateOfFireRank, energyRank, energyRegenRank, lifeRank;

    /// <summary>
    /// Bool used to determine if information about the player statistics and position / current level will be loaded from a json file
    /// </summary>
    public bool isLoad;

    /// <summary>
    /// The default initial position of the current level
    /// </summary>
    public Vector3 initialLevelPosition;

    /// <summary>
    /// Dictionary used to store the last position of each scenes, in order to make the player go back to that exact position, when exiting the shop, for example
    /// </summary>
    public Dictionary<string, Vector3> currentPosition;

    /// <summary>
    /// Function used by the game manager, when the game is closed, effectively reseting this scriptable object variables to its default state
    /// </summary>
    /// <param name="defaultMaxHealth">The default max health that is set in the PlayerHealth script attached in the player game object</param>
    /// <param name="defaultMaxEnergy">The default max energy that is set in the MagicalGemController script attached in the magical gem controller game object</param>
    public void ResetPlayerStats(int defaultMaxHealth, float defaultMaxEnergy)
    {
        coins = 0;
        maxHealth = defaultMaxHealth;
        maxEnergy = defaultMaxEnergy;
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        currentBullet = 0;
        strengthRank = 0;
        speedRank = 0;
        rangeRank = 0;
        rateOfFireRank = 0;
        energyRank = 0;
        energyRegenRank = 0;
        lifeRank = 0;
        isLoad = false;
        currentPosition["First_Level"] = Vector3.zero;
        currentPosition["Shop"] = Vector3.zero;
    }
}
