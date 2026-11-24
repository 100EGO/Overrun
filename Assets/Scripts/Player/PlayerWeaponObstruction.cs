using UnityEngine;

[DefaultExecutionOrder(-50)]
public class PlayerWeaponObstruction : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float probeHeight = 1.3f;
    [SerializeField, Min(0.1f)] private float probeDistance = 1.2f;
    [SerializeField, Min(0.01f)] private float probeRadius = 0.15f;
    [SerializeField] private float raisedPitch = -65f;
    [SerializeField, Min(0.1f)] private float blendSpeed = 12f;
    public float Amount { get; private set; }
    public float RaisedPitch => raisedPitch;
    public bool BlocksFire { get; private set; }

    private void Update()
    {
        Vector3 origin = transform.position + Vector3.up * probeHeight;
        float nearest = probeDistance;
        foreach (RaycastHit hit in Physics.SphereCastAll(origin, probeRadius, transform.forward,
                     probeDistance, obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform.root == transform.root) continue;
            nearest = Mathf.Min(nearest, hit.distance);
        }
        foreach (Collider hit in Physics.OverlapSphere(origin, probeRadius, obstacleLayers, QueryTriggerInteraction.Ignore))
            if (hit.transform.root != transform.root) nearest = 0f;
        BlocksFire = nearest < probeDistance;
        float target = BlocksFire ? Mathf.Clamp01(1f - nearest / probeDistance) : 0f;
        Amount = Mathf.MoveTowards(Amount, target, blendSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        Amount = 0f;
        BlocksFire = false;
    }
}
