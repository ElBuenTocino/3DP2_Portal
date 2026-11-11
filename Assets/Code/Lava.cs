using UnityEngine;

public class Lava : MonoBehaviour
{
    public int damage;

    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log("COllision");
    //    if (collision.gameObject.GetComponent<PlayerStats>()  != null)
    //    {
    //        PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
    //        stats.Damage(Mathf.FloorToInt(damage * Time.deltaTime));
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.GetComponent<PlayerStats>() != null)
        {
            Debug.Log("PlayerFound");
            other.gameObject.GetComponent<PlayerStats>().Die();
            //other.gameObject.GetComponent<PlayerStats>().Damage(Mathf.Max(Mathf.FloorToInt(damage * Time.deltaTime), 1));
            //PlayerStats stats = other.gameObject.GetComponent<PlayerStats>();
            //stats.Damage(Mathf.FloorToInt(damage * Time.deltaTime));
        }
    }
}
