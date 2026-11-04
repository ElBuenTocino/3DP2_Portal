using UnityEngine;

public class CrosshairPortal : MonoBehaviour
{
    public GameObject Fill;

    public void FillCrosshair(bool isFilled)
    {
        Fill.gameObject.SetActive(isFilled);
    }
}
