using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float progress;

    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position.WithAxis(Axis.Z, -1);
    }

    // Update is called once per frame
    void Update()
    {
        progress = Time.deltaTime * speed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition.WithAxis(Axis.Z, -1);
    }
}
