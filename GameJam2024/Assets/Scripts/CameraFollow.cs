using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float cameraSmoothing = 0.15f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.y = 0f;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSmoothing);
    }
}
