
using UnityEngine;

public class VictoryCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTarget;

    [Header("Camera Settings")]
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2.5f;
    [SerializeField] private float followSpeed = 5f;

    [Header("Victory Orbit")]
    [SerializeField] private float orbitSpeed = 15f;
    [SerializeField] private float startingAngle = 0f;

    private float currentAngle;
    private bool orbitEnabled = true;

    private void Start()
    {
        currentAngle = startingAngle;
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        // Rotate only while orbiting is enabled.
        if (orbitEnabled)
        {
            currentAngle += orbitSpeed * Time.deltaTime;
        }

        float radians = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Sin(radians) * cameraDistance,cameraHeight,-Mathf.Cos(radians) * cameraDistance);

        Vector3 targetPosition = player.position + offset;

        // Smoothly follow the orbit position.
        transform.position = Vector3.Lerp(transform.position,targetPosition,followSpeed * Time.deltaTime);

        // Look at the animated upper body.
        Vector3 lookPosition = cameraTarget != null
            ? cameraTarget.position
            : player.position + Vector3.up * 1.5f;

        Vector3 direction = lookPosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,followSpeed * Time.deltaTime);
        }
    }

    public void SetOrbitEnabled(bool enabled)
    {
        orbitEnabled = enabled;
    }
}
