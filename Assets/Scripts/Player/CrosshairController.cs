
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private GameObject crosshair;

    private void Update()
    {
        crosshair.SetActive(playerAim.IsAiming);
    }
}
