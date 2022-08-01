using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "Player/Stats", order = 1)]
public class PlayerStats : ScriptableObject
{
    public int coins;

    public int currentHealth;

    public int maxHealth;

    public int currentBullet;

    public float shootStrength, shootSpeed, shootRange, maxEnergy, energyRegen;

    public float[] rateOfFire;

    public void ResetStats()
    {
        coins = 0;
        currentHealth = 5;
        maxHealth = 5;
        shootRange = 1;
        shootSpeed = 0;
        shootStrength = 0;
        maxEnergy = 1;
        energyRegen = 0.1f;
    }
}
