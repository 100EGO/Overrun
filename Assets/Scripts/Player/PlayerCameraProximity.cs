using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(1000)]
public class PlayerCameraProximity : MonoBehaviour
{
    [SerializeField] private Renderer[] characterRenderers;
    [SerializeField] private Transform referencePoint;
    [SerializeField, Min(0f)] private float hideDistance = 0.65f;
    [SerializeField, Min(0f)] private float showDistance = 0.85f;
    private ShadowCastingMode[] originalModes;
    private bool hidden;

    private void Awake()
    {
        if (characterRenderers == null) characterRenderers = new Renderer[0];
        originalModes = new ShadowCastingMode[characterRenderers.Length];
        for (int i = 0; i < characterRenderers.Length; i++)
            if (characterRenderers[i] != null) originalModes[i] = characterRenderers[i].shadowCastingMode;
    }

    private void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null || referencePoint == null) return;
        float distance = Vector3.Distance(cam.transform.position, referencePoint.position);
        SetHidden(hidden ? distance < Mathf.Max(showDistance, hideDistance) : distance < hideDistance);
    }

    private void SetHidden(bool value)
    {
        if (hidden == value) return;
        hidden = value;
        for (int i = 0; i < characterRenderers.Length; i++)
            if (characterRenderers[i] != null)
                characterRenderers[i].shadowCastingMode = value ? ShadowCastingMode.ShadowsOnly : originalModes[i];
    }

    private void OnDisable()
    {
        if (originalModes != null) SetHidden(false);
    }
}
