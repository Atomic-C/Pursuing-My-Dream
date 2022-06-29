using UnityEngine;

/// <summary>
/// Simple script used in children objects that needs to have their scale unnafected by their parent's
/// </summary>
public class KeepChildScale : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.localScale = transform.localScale;       
    }
}
