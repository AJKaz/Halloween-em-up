using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float cameraSmoothing = 0.15f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;
    [SerializeField] private float minX = 0;
    [SerializeField] private float maxX = 59.6f;

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.y = 0f;
        if (targetPosition.x < minX) targetPosition.x = minX;
        if (targetPosition.x > maxX) targetPosition.x = maxX;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSmoothing);
    }
}
