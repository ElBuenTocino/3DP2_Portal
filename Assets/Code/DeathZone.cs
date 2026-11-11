using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public bool isZoneActive;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStats>() != null && isZoneActive)
        {
            other.gameObject.GetComponent<PlayerStats>().Die();
        }
    }
}
