using UnityEngine;

/// <summary>
/// Class that produces a pendulum-like movement
/// </summary>
public class PendulumMotion : MonoBehaviour
{

    private float MaxAngleDeflection = 30.0f;
    public float SpeedOfPendulum = 1.0f;

    private void FixedUpdate()
    {
        float angle = MaxAngleDeflection * Mathf.Sin(Time.time * SpeedOfPendulum);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
