using TMPro;
using UnityEngine;

/// <summary>
/// Instantiate the damage inflicted as a number
/// </summary>
public class DamageIndicator : MonoBehaviour
{
    /// <summary>
    /// Object representing the damage
    /// </summary>
    public GameObject damageObject;

    /// <summary>
    /// Random range used as force to the instantiated game object
    /// </summary>
    public float minRandomRange, maxRandomRange;

    /// <summary>
    /// Function that instantiate the damage object, change its text value to the ammount of damage inflicted and then apply a force to the instantied object
    /// </summary>
    /// <param name="damage"></param>
    public void ShowDamage(float damage)
    {
        GameObject damageObjInstance = Instantiate(damageObject, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        damageObjInstance.GetComponent<TextMeshPro>().text = damage.ToString();
        damageObjInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(minRandomRange, maxRandomRange), Random.Range(minRandomRange, maxRandomRange)), ForceMode2D.Force);
    }
}