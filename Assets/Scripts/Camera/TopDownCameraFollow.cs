using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float followSpeed = 10f;

    private Vector3 offset;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("TopDownCameraFollow: assign the Player as Target in the Inspector.", this);
            enabled = false;
            return;
        }

        // Keep the camera's initial position relative to the player.
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
    }
}
